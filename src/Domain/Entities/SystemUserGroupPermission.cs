namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;

    /// <summary>
    /// Grants or denies an application permission relative to a user group definition.
    /// </summary>
    public class SystemUserGroupPermission :
        EntityBase
    {
        #region Ctors

        protected SystemUserGroupPermission() { }

        protected internal SystemUserGroupPermission(
            int systemUserGroupId,
            int permissionId,
            PermissionEffect effect)
        {
            SystemUserGroupId = systemUserGroupId;
            PermissionId = permissionId;
            Effect = effect;
        }

        #endregion Ctors

        #region Utils

        protected internal void SetAsAllow()
            => Effect = PermissionEffect.Allow;

        protected internal void SetAsDeny()
            => Effect = PermissionEffect.Deny;

        #endregion Utils

        #region Properties
        /// <summary>
        /// Resolved allow/deny effect for evaluations.
        /// </summary>
        public PermissionEffect Effect { get; private set; }

        /// <summary>
        /// Target permission surrogate key assigned to the collection.
        /// </summary>
        public int PermissionId { get; private set; }

        /// <summary>
        /// Group this rule row belongs to.
        /// </summary>
        public int SystemUserGroupId { get; private set; }

        /// <summary>
        /// Persisted permission master record referenced by identifier.
        /// </summary>
        public virtual Permission Permission { get; private set; }

        /// <summary>
        /// Group carrying this permission junction.
        /// </summary>
        public virtual SystemUserGroup SystemUserGroup { get; private set; }
        #endregion Properties
    }
}
