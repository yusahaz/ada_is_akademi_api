namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;

    /// <summary>
    /// Permission node in a hierarchical access model.
    /// </summary>
    public class Permission :
        CodedNamedEntityBase
    {
        #region Ctors

        /// <summary>
        /// Blank row initializer for EF Core.
        /// </summary>
        protected Permission() { }

        /// <summary>
        /// Creates a permission node with display name.
        /// </summary>
        /// <param name="name">Permission name.</param>
        /// <param name="description">Optional description.</param>
        protected internal Permission(
            string name,
            string? description) :
            base(name, description)
        {
        }

        #endregion Ctors

        #region Utils

        /// <summary>
        /// Assigns the parent permission when it does not create a self-cycle.
        /// </summary>
        protected internal void SetParent(int parentId)
        {
            if (parentId != Id)
            {
                ParentId = parentId;
            }
        }

        #endregion Utils

        #region Properties

        /// <summary>
        /// Identifier of the parent permission in the hierarchy, when present.
        /// </summary>
        public int? ParentId { get; private set; }

        /// <summary>
        /// Parent permission, when scoped under another node.
        /// </summary>
        public virtual Permission? Parent { get; private set; }

        #endregion Properties
    }
}
