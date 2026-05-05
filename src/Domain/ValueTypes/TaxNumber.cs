namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Extensions;

    /// <summary>
    /// Value object representing a normalized tax number.
    /// </summary>
    public readonly record struct TaxNumber
    {
        #region Ctors

        /// <summary>
        /// Initializes a normalized tax number value.
        /// </summary>
        /// <param name="value">Raw tax number text.</param>
        public TaxNumber(string value)
        {
            Value = value.ThrowIfNullOrWhiteSpace(DomainErrorCodes.TaxNumberInvalid).Trim();
        }

        #endregion Ctors

        #region Properties

        /// <summary>
        /// Normalized tax number text.
        /// </summary>
        public string Value { get; }

        #endregion Properties

        #region Operators

        /// <summary>
        /// Converts raw text to a normalized <see cref="TaxNumber"/>.
        /// </summary>
        /// <param name="value">Raw tax number text.</param>
        public static implicit operator TaxNumber(string value) => new(value);

        /// <summary>
        /// Returns the underlying normalized string.
        /// </summary>
        /// <param name="value">Tax number instance.</param>
        public static implicit operator string(TaxNumber value) => value.Value;

        #endregion Operators
    }
}
