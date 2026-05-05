namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;
    using Azoxia.Core.Extensions;
    using System.Security.Cryptography;

    /// <summary>
    /// Credentials and lifecycle state backing an application authentication principal.
    /// </summary>
    public class SystemUser :
        DeletableEntityBase
    {
        #region Fields

        /// <summary>
        /// PBKDF2 hash algorithm used when deriving password material.
        /// </summary>
        private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;

        /// <summary>
        /// Defines the size of the hash for password hashing.
        /// </summary>
        private const byte HashSize = 32;

        /// <summary>
        /// Defines the number of iterations for the PBKDF2 algorithm used in password hashing.
        /// </summary>
        private const int Iterations = 100000;

        /// <summary>
        /// Upper bound before the account transitions into a locked policy state.
        /// </summary>
        private const int MaxFailedLoginAttempts = 3;

        /// <summary>
        /// Defines the size of the salt for password hashing.
        /// </summary>
        private const byte SaltSize = 16;

        private readonly List<SystemUserDevice> _devices = new();
        private readonly List<SystemUserRefreshToken> _refreshTokens = new();

        #endregion Fields

        #region Ctors

        /// <summary>
        /// Blank row initializer for EF Core.
        /// </summary>
        protected SystemUser() { }

        /// <summary>
        /// Creates a pending user with hashed password material.
        /// </summary>
        /// <param name="email">Login email.</param>
        /// <param name="password">Plaintext password to hash.</param>
        /// <param name="type">User type classification.</param>
        protected internal SystemUser(
            string email,
            string password,
            SystemUserType type)
        {
            Email = email;
            Type = type;
            AccountStatus = AccountStatus.Pending;
            PasswordSalt = GeneratePasswordSalt();
            PasswordHash = GeneratePasswordHash(password);
        }

        #endregion Ctors

        #region Utils

        /// <summary>
        /// Registers or updates a device row and optionally refreshes its push token.
        /// </summary>
        protected internal SystemUserDevice AddOrUpdateDevice(
            string deviceIdentifier,
            DevicePlatform platform,
            string? deviceToken = null)
        {
            SystemUserDevice? device = Devices
                .FirstOrDefault(x => x.DeviceIdentifier == deviceIdentifier);

            if (device is null)
            {
                device = new(Id, deviceIdentifier, platform);
                _devices.Add(device);
            }

            device.RecordActivity();

            if (deviceToken is not null)
            {
                device.UpdateDeviceToken(deviceToken);
            }

            return device;
        }

        /// <summary>
        /// Bans the account and revokes all refresh tokens.
        /// </summary>
        protected internal void Ban()
        {
            AccountStatus = AccountStatus.Banned;
            RevokeAllRefreshTokens();
        }

        /// <summary>
        /// Rotates password material and revokes outstanding refresh tokens.
        /// </summary>
        protected internal void ChangePassword(string password)
        {
            PasswordSalt = GeneratePasswordSalt();
            PasswordHash = GeneratePasswordHash(password);
            LastPasswordChangeAt = DateTimeOffset.UtcNow;
            RevokeAllRefreshTokens();
        }

        /// <summary>
        /// Verifies a plaintext password against stored PBKDF2 hash material.
        /// </summary>
        protected internal Task<bool> CheckPassword(string password)
        {
            return Task.Run(() =>
            {
                byte[] saltBytes = Convert.FromBase64String(PasswordSalt);
                byte[] storedHash = Convert.FromBase64String(PasswordHash);
                byte[] computed = Rfc2898DeriveBytes.Pbkdf2(
                    password: password,
                    salt: saltBytes,
                    iterations: Iterations,
                    hashAlgorithm: HashAlgorithm,
                    outputLength: HashSize);

                return CryptographicOperations.FixedTimeEquals(storedHash, computed);
            });
        }

        /// <summary>
        /// Soft-deletes this user through the base lifecycle API.
        /// </summary>
        protected internal void DeleteSystemUser()
            => base.Delete();

        /// <summary>
        /// Derives a Base64 password hash using the current salt and iteration settings.
        /// </summary>
        private string GeneratePasswordHash(string password)
        {
            byte[] saltBytes = Convert.FromBase64String(PasswordSalt);
            byte[] hashBytes = Rfc2898DeriveBytes.Pbkdf2(
                password,
                saltBytes,
                Iterations,
                HashAlgorithm,
                HashSize);

            return hashBytes.ToBase64String();
        }

        /// <summary>
        /// Generates a new random salt encoded as Base64.
        /// </summary>
        private string GeneratePasswordSalt()
            => RandomNumberGenerator.GetBytes(SaltSize).ToBase64String();

        /// <summary>
        /// Issues or extends a refresh token for the device pairing.
        /// </summary>
        protected internal SystemUserRefreshToken IssueRefreshToken(string tokenHash, int deviceId, DateTimeOffset expiresAt)
        {
            SystemUserRefreshToken? token = RefreshTokens
                .FirstOrDefault(x => x.DeviceId == deviceId && x.TokenHash == tokenHash);

            if (token is null)
            {
                token = new(Id, tokenHash, deviceId, expiresAt);
                _refreshTokens.Add(token);
            }
            else
            {
                token.Until(expiresAt);
            }

            return token;
        }

        /// <summary>
        /// Restores an active account status when policy allows.
        /// </summary>
        protected internal void Reactivate()
        {
            (AccountStatus != AccountStatus.Banned)
                .ThrowIfFalse(DomainErrorCodes.SystemUserInvalidStatusTransition);
            AccountStatus = AccountStatus.Active;
        }

        /// <summary>
        /// Increments failed login counters after an unsuccessful attempt.
        /// </summary>
        protected internal void RecordFailedLoginAttempt()
        {
            FailedLoginAttempts++;
            LastFailedLoginAt = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Clears failed-login counters after a successful authentication.
        /// </summary>
        protected internal void RecordSuccessfulLogin()
        {
            FailedLoginAttempts = 0;
            LastSuccessfulLoginAt = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Stores email verification token hash and expiry for outbound activation flows.
        /// </summary>
        protected internal void RequestEmailVerification(string tokenHash, DateTimeOffset expiresAt)
        {
            tokenHash.ThrowIfNullOrWhiteSpace();
            (expiresAt > DateTimeOffset.UtcNow)
                .ThrowIfFalse(DomainErrorCodes.SystemUserEmailVerificationExpiresAtInvalid);
            EmailVerificationToken = tokenHash;
            EmailVerificationExpiresAt = expiresAt;
        }

        /// <summary>
        /// Revokes every active refresh token row for this user.
        /// </summary>
        protected internal void RevokeAllRefreshTokens()
        {
            foreach (SystemUserRefreshToken token in RefreshTokens.Where(x => x.IsActive))
            {
                token.Revoke();
            }
        }

        /// <summary>
        /// Revokes a single refresh token matched by hash.
        /// </summary>
        protected internal void RevokeRefreshToken(string tokenHash)
        {
            tokenHash.ThrowIfNullOrWhiteSpace();

            SystemUserRefreshToken? token = RefreshTokens
                .FirstOrDefault(x => x.TokenHash == tokenHash);
            token = token.ThrowIfNull(DomainErrorCodes.SystemUserRefreshTokenNotFound);
            token.Revoke();
        }

        /// <summary>
        /// Suspends the account when policy allows.
        /// </summary>
        protected internal void Suspend()
        {
            (AccountStatus != AccountStatus.Banned)
                .ThrowIfFalse(DomainErrorCodes.SystemUserInvalidStatusTransition);
            AccountStatus = AccountStatus.Suspended;
        }

        /// <summary>
        /// Updates profile name and optional phone fields.
        /// </summary>
        protected internal void Update(
            string firstName,
            string lastName,
            string? phone = null)
        {
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
        }

        /// <summary>
        /// Completes email verification when token and expiry are valid.
        /// </summary>
        protected internal void VerifyEmail(string tokenHash)
        {
            tokenHash.ThrowIfNullOrWhiteSpace();
            (!EmailVerifiedAt.HasValue).ThrowIfFalse(DomainErrorCodes.SystemUserEmailAlreadyVerified);
            (EmailVerificationToken == tokenHash
                && EmailVerificationExpiresAt > DateTimeOffset.UtcNow)
                .ThrowIfFalse(DomainErrorCodes.SystemUserEmailVerificationInvalid);

            EmailVerifiedAt = DateTimeOffset.UtcNow;
            AccountStatus = AccountStatus.Active;
            EmailVerificationToken = null;
            EmailVerificationExpiresAt = null;
        }

        #endregion Utils

        #region Properties

        /// <summary>
        /// Account activation status (Pending, Active, Suspended, Banned).
        /// </summary>
        public AccountStatus AccountStatus { get; private set; }

        /// <summary>
        /// Canonical login identifier (email).
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// Email verification token expiration time (UTC).
        /// </summary>
        public DateTimeOffset? EmailVerificationExpiresAt { get; private set; }

        /// <summary>
        /// Email verification token hash (SHA-256) for account activation.
        /// </summary>
        public string? EmailVerificationToken { get; private set; }

        /// <summary>
        /// Email verification completion timestamp (UTC).
        /// </summary>
        public DateTimeOffset? EmailVerifiedAt { get; private set; }

        /// <summary>
        /// Count of consecutive failed authentication attempts.
        /// </summary>
        public int FailedLoginAttempts { get; private set; }

        /// <summary>
        /// Optional given name for profile UI.
        /// </summary>
        public string? FirstName { get; private set; }

        /// <summary>
        /// Last observed failed login instant (UTC).
        /// </summary>
        public DateTimeOffset? LastFailedLoginAt { get; private set; }

        /// <summary>
        /// Optional family name for profile UI.
        /// </summary>
        public string? LastName { get; private set; }

        /// <summary>
        /// Instant of the most recent password rotation (UTC).
        /// </summary>
        public DateTimeOffset? LastPasswordChangeAt { get; private set; }

        /// <summary>
        /// Last successful authentication instant (UTC).
        /// </summary>
        public DateTimeOffset? LastSuccessfulLoginAt { get; private set; }

        /// <summary>
        /// PBKDF2-derived password hash stored as Base64.
        /// </summary>
        public string PasswordHash { get; private set; }

        /// <summary>
        /// Random salt material accompanying the password hash (Base64).
        /// </summary>
        public string PasswordSalt { get; private set; }

        /// <summary>
        /// Optional phone contact for recovery or notifications.
        /// </summary>
        public string? Phone { get; private set; }

        /// <summary>
        /// Role or audience classification for policy branching.
        /// </summary>
        public SystemUserType Type { get; private set; }


        /// <summary>
        /// True when failed login threshold matches policy lock rules.
        /// </summary>
        public bool IsLocked => FailedLoginAttempts >= MaxFailedLoginAttempts;


        /// <summary>
        /// Known devices registered for this account.
        /// </summary>
        public virtual IReadOnlyList<SystemUserDevice> Devices => _devices.AsReadOnly();

        /// <summary>
        /// Active and historical refresh token rows for session continuity.
        /// </summary>
        public virtual IReadOnlyList<SystemUserRefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

        #endregion Properties
    }
}
