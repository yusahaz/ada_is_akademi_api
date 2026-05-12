namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.AdaIsAkademi.Domain.Events;
    using Azoxia.Core.Domain;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;

    /// <summary>
    /// Tracks worker CV upload lifecycle from object-storage upload through extraction and worker review.
    /// </summary>
    public class CvUploadSession :
        EntityAggregateRoot
    {
        #region Fields

        private const long MaxFileSizeBytes = 10L * 1024 * 1024;

        #endregion Fields

        #region Ctors

        /// <summary>
        /// Blank row initializer for EF Core.
        /// </summary>
        protected CvUploadSession() { }

        /// <summary>
        /// Creates a new uploaded session scoped to a worker and object-storage key.
        /// </summary>
        protected internal CvUploadSession(
            int workerId,
            string objectKey,
            string fileName,
            string contentType,
            long fileSizeBytes,
            CvFileFormat fileFormat)
        {
            WorkerId = workerId;
            ObjectKey = objectKey.ThrowIfNullOrWhiteSpace(AzoxiaErrorCodes.StringNullOrWhiteSpace).Trim();
            FileName = fileName.ThrowIfNullOrWhiteSpace(AzoxiaErrorCodes.StringNullOrWhiteSpace).Trim();
            ContentType = contentType.ThrowIfNullOrWhiteSpace(AzoxiaErrorCodes.StringNullOrWhiteSpace).Trim();
            (fileSizeBytes > 0 && fileSizeBytes <= MaxFileSizeBytes)
                .ThrowIfFalse(DomainErrorCodes.CvUploadSessionFileSizeOutOfRange);
            FileSizeBytes = fileSizeBytes;
            FileFormat = fileFormat;
            Status = CvUploadSessionStatus.Uploaded;
            CreatedAt = DateTimeOffset.UtcNow;
            RaiseDomainEvent(() => new CvUploadedEvent(Id, WorkerId));
        }

        #endregion Ctors

        #region Utils

        /// <summary>
        /// Marks the upload session as extraction in progress.
        /// </summary>
        protected internal void MarkAsExtracting()
        {
            (Status == CvUploadSessionStatus.Uploaded || Status == CvUploadSessionStatus.Failed)
                .ThrowIfFalse(DomainErrorCodes.CvUploadSessionInvalidStatusTransition);

            Status = CvUploadSessionStatus.Extracting;
            ExtractionRequestedAt = DateTimeOffset.UtcNow;
            FailureReason = null;
        }

        /// <summary>
        /// Marks extraction as completed and persists extracted preview payload for worker review.
        /// </summary>
        protected internal void MarkAsAwaitingReview(string extractedJson)
        {
            (Status == CvUploadSessionStatus.Extracting)
                .ThrowIfFalse(DomainErrorCodes.CvUploadSessionInvalidStatusTransition);

            ExtractedJson = extractedJson.ThrowIfNullOrWhiteSpace(DomainErrorCodes.CvUploadSessionExtractedPayloadRequired).Trim();
            Status = CvUploadSessionStatus.AwaitingReview;
            ExtractionCompletedAt = DateTimeOffset.UtcNow;
            FailureReason = null;
            RaiseDomainEvent(() => new CvExtractionCompletedEvent(Id, WorkerId));
        }

        /// <summary>
        /// Marks extraction as failed with optional reason.
        /// </summary>
        protected internal void MarkAsFailed(string? reason = null)
        {
            (Status == CvUploadSessionStatus.Extracting)
                .ThrowIfFalse(DomainErrorCodes.CvUploadSessionInvalidStatusTransition);

            Status = CvUploadSessionStatus.Failed;
            FailureReason = reason.IsNullOrWhiteSpace() ? null : reason.Trim();
            ExtractionCompletedAt = DateTimeOffset.UtcNow;
            RaiseDomainEvent(() => new CvExtractionFailedEvent(Id, WorkerId, FailureReason));
        }

        /// <summary>
        /// Confirms worker approval for extracted fields.
        /// Application flow must call this only from <see cref="CvUploadSessionStatus.AwaitingReview"/> after applying payload writes.
        /// Repeated confirmation retries are handled idempotently in Application when status is already <see cref="CvUploadSessionStatus.Confirmed"/>.
        /// </summary>
        protected internal void Confirm()
        {
            (Status == CvUploadSessionStatus.AwaitingReview)
                .ThrowIfFalse(DomainErrorCodes.CvUploadSessionInvalidStatusTransition);

            Status = CvUploadSessionStatus.Confirmed;
            ReviewedAt = DateTimeOffset.UtcNow;
            RaiseDomainEvent(() => new CvImportConfirmedEvent(Id, WorkerId));
        }

        /// <summary>
        /// Discards extracted fields after worker review.
        /// </summary>
        protected internal void Discard()
        {
            (Status == CvUploadSessionStatus.AwaitingReview)
                .ThrowIfFalse(DomainErrorCodes.CvUploadSessionInvalidStatusTransition);

            Status = CvUploadSessionStatus.Discarded;
            ReviewedAt = DateTimeOffset.UtcNow;
            RaiseDomainEvent(() => new CvImportDiscardedEvent(Id, WorkerId));
        }

        #endregion Utils

        #region Properties

        /// <summary>
        /// Stored MIME type provided at upload-confirm time.
        /// </summary>
        public string ContentType { get; private set; }

        /// <summary>
        /// UTC timestamp when session row was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; private set; }

        /// <summary>
        /// Serialized extraction preview payload for worker confirmation.
        /// </summary>
        public string? ExtractedJson { get; private set; }

        /// <summary>
        /// UTC timestamp when extraction completed (success or failure).
        /// </summary>
        public DateTimeOffset? ExtractionCompletedAt { get; private set; }

        /// <summary>
        /// UTC timestamp when extraction was requested by background processing.
        /// </summary>
        public DateTimeOffset? ExtractionRequestedAt { get; private set; }

        /// <summary>
        /// Optional extraction failure detail from background processing.
        /// </summary>
        public string? FailureReason { get; private set; }

        /// <summary>
        /// Original client-provided file name.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Parsed file format for extraction provider routing.
        /// </summary>
        public CvFileFormat FileFormat { get; private set; }

        /// <summary>
        /// Size of uploaded object in bytes.
        /// </summary>
        public long FileSizeBytes { get; private set; }

        /// <summary>
        /// Object-storage key bound to this upload session.
        /// </summary>
        public string ObjectKey { get; private set; }

        /// <summary>
        /// UTC timestamp when worker finalized review (confirm/discard).
        /// </summary>
        public DateTimeOffset? ReviewedAt { get; private set; }

        /// <summary>
        /// Current upload/extraction lifecycle status.
        /// </summary>
        public CvUploadSessionStatus Status { get; private set; }

        /// <summary>
        /// Worker owner identifier.
        /// </summary>
        public int WorkerId { get; private set; }

        /// <summary>
        /// Linked worker navigation.
        /// </summary>
        public virtual Worker Worker { get; private set; }

        #endregion Properties
    }
}
