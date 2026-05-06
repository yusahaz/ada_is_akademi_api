namespace Azoxia.AdaIsAkademi.Infrastructure
{
    using Azoxia.AdaIsAkademi.Application.Services;
    using Azoxia.AdaIsAkademi.Domain;

    /// <summary>
    /// Deterministic placeholder extractor used until AI-backed CV extraction is wired.
    /// </summary>
    internal sealed class FakeCvExtractionService :
        ICvExtractionService
    {
        #region Methods

        /// <inheritdoc />
        public Task<CvExtractionResult> ExtractAsync(CvExtractionRequest request, CancellationToken cancellationToken)
        {
            _ = request;
            cancellationToken.ThrowIfCancellationRequested();

            const string payload = """
{
  "educations": [],
  "experiences": [],
  "certificates": [],
  "languages": [],
  "skills": []
}
""";

            return Task.FromResult(new CvExtractionResult(true, payload));
        }

        /// <inheritdoc />
        public bool Supports(CvFileFormat format)
            => format == CvFileFormat.Pdf || format == CvFileFormat.Docx;

        #endregion Methods
    }
}
