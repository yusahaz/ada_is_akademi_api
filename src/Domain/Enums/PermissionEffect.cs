namespace Azoxia.AdaIsAkademi.Domain
{
    /// <summary>
    /// Allow or deny contribution when evaluating a permission rule.
    /// </summary>
    public enum PermissionEffect
    {
        /// <summary>
        /// Grants the permission when this rule applies.
        /// </summary>
        Allow = 10,

        /// <summary>
        /// Withholds the permission when this rule applies.
        /// </summary>
        Deny = 20,
    }
}
