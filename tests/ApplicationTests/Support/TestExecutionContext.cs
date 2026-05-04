namespace Azoxia.AdaIsAkademi.Application.Tests.Support
{
    using Azoxia.Core.Identity;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Test double for <see cref="IExecutionContext"/> — supports mutating claims after seed so handler assertions can mirror JWT actor ids.
    /// </summary>
    public sealed class TestExecutionContext :
        IExecutionContext
    {
        #region Fields

        private readonly Dictionary<string, List<string>> _claims;

        #endregion Fields

        #region Ctors

        public TestExecutionContext(
            bool isAuthenticated = false,
            IReadOnlyDictionary<string, IReadOnlyList<string>>? claims = null)
        {
            IsAuthenticated = isAuthenticated;
            _claims = [];
            if (claims is null)
            {
                return;
            }

            foreach (KeyValuePair<string, IReadOnlyList<string>> pair in claims)
            {
                _claims[pair.Key] = pair.Value.ToList();
            }
        }

        #endregion Ctors

        #region Properties

        /// <inheritdoc />
        public bool IsAuthenticated { get; }

        #endregion Properties

        #region Methods

        /// <inheritdoc />
        public string? GetClaim(string claimType)
            => _claims.TryGetValue(claimType, out List<string>? values)
                ? values.FirstOrDefault()
                : null;

        /// <inheritdoc />
        public IReadOnlyList<string> GetClaims(string claimType)
            => _claims.TryGetValue(claimType, out List<string>? values)
                ? values
                : Array.Empty<string>();

        /// <summary>
        /// Replaces values for one claim key (typically <c>employer_id</c> or <c>worker_id</c>).
        /// </summary>
        public void ReplaceClaim(string claimType, params string[] values)
        {
            _claims[claimType] = [.. values];
        }

        #endregion Methods
    }
}
