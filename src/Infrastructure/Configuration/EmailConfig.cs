namespace Azoxia.AdaIsAkademi.Infrastructure.Configuration
{
    using Azoxia.Core.Configuration;

    /// <summary>
    /// SMTP e-mail settings bound from the <c>EmailConfig</c> section.
    /// </summary>
    public sealed record EmailConfig : IConfig
    {
        /// <summary>
        /// Enables real SMTP delivery when true.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// SMTP server host.
        /// </summary>
        public string? Host { get; set; }

        /// <summary>
        /// SMTP server port.
        /// </summary>
        public int Port { get; set; } = 587;

        /// <summary>
        /// Sender display name.
        /// </summary>
        public string? FromName { get; set; }

        /// <summary>
        /// Sender e-mail address.
        /// </summary>
        public string? FromEmail { get; set; }

        /// <summary>
        /// SMTP auth username.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// SMTP auth password.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Enables SSL/TLS immediately when connecting.
        /// </summary>
        public bool UseSsl { get; set; } = false;

        /// <summary>
        /// Enables SMTP authentication.
        /// </summary>
        public bool UseAuthentication { get; set; } = true;
    }
}
