namespace Azoxia.AdaIsAkademi.Domain
{
    /// <summary>
    /// Lifecycle state of a CV upload session.
    /// </summary>
    public enum CvUploadSessionStatus
    {
        /// <summary>
        /// Worker explicitly discarded extracted results.
        /// </summary>
        Discarded = 50,

        /// <summary>
        /// Extraction failed and captured failure metadata.
        /// </summary>
        Failed = 60,

        /// <summary>
        /// Worker accepted extracted fields and closed the session.
        /// </summary>
        Confirmed = 40,

        /// <summary>
        /// Upload is persisted and ready for extraction.
        /// </summary>
        Uploaded = 10,

        /// <summary>
        /// Background extraction is in progress.
        /// </summary>
        Extracting = 20,

        /// <summary>
        /// Extraction completed; waiting worker review.
        /// </summary>
        AwaitingReview = 30,
    }
}
