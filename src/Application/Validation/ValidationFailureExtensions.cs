namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;

    /// <summary>
    /// Maps <see cref="ErrorCode"/> catalog entries to <see cref="ValidationFailure"/> for field-level validators.
    /// </summary>
    internal static class ValidationFailureExtensions
    {
        #region Methods

        /// <summary>
        /// Builds a validation failure for the given model field using the error code and its catalog message.
        /// </summary>
        /// <param name="error">Catalogued application validation error.</param>
        /// <param name="fieldName">Name of the invalid field (typically <c>nameof</c>).</param>
        public static ValidationFailure ForField(this ErrorCode error, string fieldName) =>
            new(Field: fieldName, Code: error.Code, Message: error.ErrorMessage);

        #endregion Methods
    }
}
