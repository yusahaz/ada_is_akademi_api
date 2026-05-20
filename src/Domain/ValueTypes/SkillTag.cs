namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Extensions;

    /// <summary>
    /// Value object representing a normalized skill tag.
    /// </summary>
    public readonly record struct SkillTag
    {
        #region Ctors

        /// <summary>
        /// Initializes a normalized skill tag value.
        /// </summary>
        /// <param name="value">Raw skill tag text.</param>
        public SkillTag(string value)
        {
            string normalized = SkillLabelNormalizer.ToDisplayPascalCase(
                value.ThrowIfNullOrWhiteSpace(DomainErrorCodes.SkillTagInvalid));

            if (normalized.Length == 0)
            {
                DomainErrorCodes.SkillTagInvalid.Throw();
            }

            Value = normalized;
        }

        #endregion Ctors

        #region Properties

        /// <summary>
        /// Normalized tag value.
        /// </summary>
        public string Value { get; }

        #endregion Properties

        #region Methods

        /// <inheritdoc />
        public bool Equals(SkillTag other) =>
            string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

        /// <inheritdoc />
        public override readonly int GetHashCode() =>
            StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

        #endregion Methods

        #region Operators

        /// <summary>
        /// Converts raw text to a normalized <see cref="SkillTag"/>.
        /// </summary>
        /// <param name="value">Raw skill tag text.</param>
        public static implicit operator SkillTag(string value) => new(value);

        /// <summary>
        /// Returns the underlying normalized string.
        /// </summary>
        /// <param name="tag">Skill tag instance.</param>
        public static implicit operator string(SkillTag tag) => tag.Value;

        #endregion Operators
    }
}
