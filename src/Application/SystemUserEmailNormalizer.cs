namespace Azoxia.AdaIsAkademi.Application
{
    /// <summary>
    /// Normalizes system-user login email for storage and lookup (trim + lowercase).
    /// </summary>
    public static class SystemUserEmailNormalizer
    {
        /// <summary>
        /// Trims whitespace and converts to lowercase using invariant casing rules.
        /// </summary>
        /// <param name="email">Raw email from client.</param>
        /// <returns>Normalized email suitable for persistence and equality checks.</returns>
        public static string Normalize(string email)
        {
            return email.Trim().ToLowerInvariant();
        }
    }
}
