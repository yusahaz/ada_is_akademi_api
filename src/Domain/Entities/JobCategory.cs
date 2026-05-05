namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;

    /// <summary>
    /// Hierarchical job classification node with an optional parent category.
    /// </summary>
    public class JobCategory :
        CodedNamedEntityBase
    {
        #region Ctors

        /// <summary>
        /// Initializes a blank instance for persistence materialization.
        /// </summary>
        protected JobCategory() { }

        /// <summary>
        /// Initializes a named category optionally carrying a descriptive text.
        /// </summary>
        /// <param name="name">Display or coded name supplied to the base entity.</param>
        /// <param name="description">Optional longer description.</param>
        protected internal JobCategory(
            string name,
            string? description = null) :
            base(name, description)
        {

        }

        #endregion Ctors

        #region Utils

        /// <summary>
        /// Soft-deletes this category through the base lifecycle API.
        /// </summary>
        protected internal virtual void DeleteCategory()
            => base.Delete();

        /// <summary>
        /// Assigns the parent category identifier.
        /// </summary>
        /// <param name="parentId">Identifier of the parent <see cref="JobCategory"/>.</param>
        protected internal void SetParent(int parentId)
        {
            ParentId = parentId;
        }

        /// <summary>
        /// Renames the category and optionally refreshes its description.
        /// </summary>
        /// <param name="name">New name value.</param>
        /// <param name="description">Optional updated description.</param>
        protected internal virtual void UpdateCategoryName(string name, string? description = null)
            => base.UpdateName(name, description);

        #endregion Utils

        #region Properties
        /// <summary>
        /// Foreign key to the parent category, when present.
        /// </summary>
        public int? ParentId { get; private set; }

        /// <summary>
        /// Parent category in the hierarchy, if any.
        /// </summary>
        public virtual JobCategory? Parent { get; private set; }
        #endregion Properties
    }
}
