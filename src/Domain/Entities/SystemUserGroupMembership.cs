namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;

    /// <summary>
    /// Join between a principal user account and an authorization group, optionally constrained by scope.
    /// </summary>
    public class SystemUserGroupMembership :
        EntityBase
    {
        #region Ctors

        private SystemUserGroupMembership() { }

        protected internal SystemUserGroupMembership(
            int systemUserGroupId,
            int systemUserId,
            MembershipScopeType scopeType = MembershipScopeType.Global,
            int? scopeId = null)
        {
            SystemUserGroupId = systemUserGroupId;
            SystemUserId = systemUserId;
            ScopeType = scopeType;
            ScopeId = scopeType != MembershipScopeType.Global
                ? scopeId
                : null;
            IsActive = true;
        }

        #endregion Ctors

        #region Utils

        protected internal void SetAsActive()
            => IsActive = true;

        protected internal void SetAsPassive()
            => IsActive = false;

        #endregion Utils

        #region Properties
        /// <summary>
        /// True while membership participates in evaluations.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Optional tenant or resource discriminator when scoped away from Global.
        /// </summary>
        public int? ScopeId { get; private set; }

        /// <summary>
        /// Declares breadth of applicability for the attachment.
        /// </summary>
        public MembershipScopeType ScopeType { get; private set; }

        /// <summary>
        /// Group surrogate key referenced by membership.
        /// </summary>
        public int SystemUserGroupId { get; private set; }

        /// <summary>
        /// User surrogate key participating in group membership.
        /// </summary>
        public int SystemUserId { get; private set; }

        /// <summary>
        /// Group definition row for this junction.
        /// </summary>
        public virtual SystemUserGroup SystemUserGroup { get; private set; }

        /// <summary>
        /// User linked to group membership evaluation.
        /// </summary>
        public virtual SystemUser SystemUser { get; private set; }
        #endregion Properties
    }
}
