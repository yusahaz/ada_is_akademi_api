namespace Azoxia.AdaIsAkademi.Domain
{
    /// <summary>
    /// Declares how broadly a group membership applies.
    /// </summary>
    public enum MembershipScopeType
    {
        /// <summary>
        /// Scoped to resources owned by a single employer.
        /// </summary>
        EmployerScoped = 1,

        /// <summary>
        /// Applies across the entire application boundary.
        /// </summary>
        Global = 0,

        /// <summary>
        /// Scoped to a specific employer location.
        /// </summary>
        LocationScoped = 2,
    }
}
