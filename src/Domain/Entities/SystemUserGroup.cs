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

        /// <summary>
        /// Blank row initializer for EF Core.
        /// </summary>
        protected SystemUserGroup() { }

        /// <summary>
        /// Creates a group with optional system-seeded semantics.
        /// </summary>
        /// <param name="name">Group name.</param>
        /// <param name="description">Optional description.</param>
        /// <param name="isSystem">Whether the group is system-seeded.</param>
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

        /// <summary>
        /// Ensures the group participates in permission evaluations.
        /// </summary>
        protected internal void Activate()
        {
            if (!IsActive)
            {
                IsActive = true;
            }
        }

        /// <summary>
        /// Adds or updates a permission rule for this group.
        /// </summary>
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

        /// <summary>
        /// Excludes the group from permission evaluations.
        /// </summary>
        protected internal void Deactivate()
        {
            if (IsActive)
            {
                IsActive = false;
            }
        }

        /// <summary>
        /// Soft-deletes non-system groups through the base lifecycle API.
        /// </summary>
        protected override void Delete()
        {
            if (!IsSystem)
            {
                base.Delete();
            }
        }

        /// <summary>
        /// Removes a permission junction row when it exists.
        /// </summary>
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

        /// <summary>
        /// Updates the relative ordering weight for permission stacking.
        /// </summary>
        protected internal void SetLevel(int level)
        {
            Level = level;
        }

        /// <summary>
        /// Renames the group and optionally refreshes its description.
        /// </summary>
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
