namespace Azoxia.AdaIsAkademi.Application.Services
{
    using Azoxia.AdaIsAkademi.Domain;

    /// <summary>
    /// Extracts structured candidate profile hints from uploaded CV documents.
    /// </summary>
    public interface ICvExtractionService
    {
        /// <summary>
        /// Executes extraction for one uploaded CV object.
        /// </summary>
        /// <param name="request">Upload metadata and storage reference.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Extraction result payload and status detail.</returns>
        Task<CvExtractionResult> ExtractAsync(CvExtractionRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Returns whether the extractor supports the provided file format.
        /// </summary>
        /// <param name="format">Candidate file format.</param>
        /// <returns><c>true</c> when extraction is supported.</returns>
        bool Supports(CvFileFormat format);
    }

    /// <summary>
    /// Immutable extraction request shape for one CV upload session.
    /// </summary>
    public sealed record CvExtractionRequest(
        int CvUploadSessionId,
        int WorkerId,
        string ObjectKey,
        string FileName,
        string ContentType,
        CvFileFormat FileFormat);

    /// <summary>
    /// Immutable extraction response with serialized preview payload and optional failure reason.
    /// </summary>
    public sealed record CvExtractionResult(
        bool IsSuccess,
        string ExtractedJson,
        string? FailureReason = null);
}
