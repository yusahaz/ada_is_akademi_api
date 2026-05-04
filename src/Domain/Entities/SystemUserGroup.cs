namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;

    /// <summary>
    /// Logical authorization group exposing named permissions applied to memberships.
    /// </summary>
    public class SystemUserGroup :
        CodedNamedEntityBase
    {
        #region Fields

        private readonly List<SystemUserGroupPermission> _permissions = new();

        #endregion Fields

        #region Ctors

        protected SystemUserGroup() { }

        protected internal SystemUserGroup(
            string name,
            string? description,
            bool isSystem = false) :
            base(name, description)
        {
            Level = 0;
            IsSystem = isSystem;
            IsActive = true;
        }

        #endregion Ctors

        #region Utils

        protected internal void Activate()
        {
            if (!IsActive)
            {
                IsActive = true;
            }
        }

        protected internal SystemUserGroupPermission AddPermission(int permissionId, PermissionEffect effect)
        {
            SystemUserGroupPermission? permission = Permissions
                .FirstOrDefault(x => x.PermissionId == permissionId);

            if (permission is not null)
            {
                if (effect == PermissionEffect.Allow)
                {
                    permission.SetAsAllow();
                }
                else
                {
                    permission.SetAsDeny();
                }
            }
            else
            {
                permission = new(Id, permissionId, effect);
                _permissions.Add(permission);
            }

            return permission;
        }

        protected internal void Deactivate()
        {
            if (IsActive)
            {
                IsActive = false;
            }
        }

        protected override void Delete()
        {
            if (!IsSystem)
            {
                base.Delete();
            }
        }

        protected internal void RemovePermission(int permissionId)
        {
            SystemUserGroupPermission? permission = Permissions
                .FirstOrDefault(x => x.PermissionId == permissionId);

            if (permission is not null)
            {
                _permissions.Remove(permission);
                return;
            }

        }

        protected internal void SetLevel(int level)
        {
            Level = level;
        }

        protected internal void UpdateSystemUserGroupName(string name, string? description = null)
        {
            base.UpdateName(name, description);
        }

        #endregion Utils

        #region Properties
        /// <summary>
        /// True while evaluations should honor group membership-derived rules.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Indicates system-seeded groups that resist mutation or deletion.
        /// </summary>
        public bool IsSystem { get; private set; }

        /// <summary>
        /// Relative ordering weight for composing permission stacks.
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// Allow/deny permission rows linked to this group.
        /// </summary>
        public virtual IReadOnlyList<SystemUserGroupPermission> Permissions => _permissions.AsReadOnly();
        #endregion Properties
    }
}
