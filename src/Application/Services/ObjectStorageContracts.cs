namespace Azoxia.AdaIsAkademi.Application.Services
{
    /// <summary>
    /// MinIO/S3 uyumlu uç yapılandırması; eksik ise altyapı katmanı güvenli stub URL üretir.
    /// </summary>
    public sealed class ObjectStoragePresignerOptions
    {
        #region Fields

        /// <summary>
        /// <c>appsettings.json</c> bölüm adı (<c>"ObjectStorage"</c>).
        /// </summary>
        public const string ConfigurationSectionName = "ObjectStorage";

        #endregion Fields

        #region Properties

        /// <summary>
        /// API erişim anahtarı (MinIO Access Key veya IAM access key).
        /// </summary>
        public string? AccessKey { get; set; }

        /// <summary>
        /// Hedef bucket adı.
        /// </summary>
        public string? BucketName { get; set; }

        /// <summary>
        /// MinIO ve özel uçlar için RFC path-style adresleme kullanılıp kullanılmayacağı.
        /// </summary>
        public bool ForcePathStyle { get; set; } = true;

        /// <summary>
        /// Bölge (AWS SDK gereksinimi; MinIO için boş kalabilir).
        /// </summary>
        public string? RegionName { get; set; }

        /// <summary>
        /// Servis ana URL’si (<c>http://localhost:9000</c> vb.).
        /// </summary>
        public string? ServiceUrl { get; set; }

        /// <summary>
        /// İmza sırrı.
        /// </summary>
        public string? SecretKey { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Presigned PUT yanı paketi.
    /// </summary>
    /// <param name="Url">İstemci PUT adresi.</param>
    /// <param name="ExpiresAtUtc">UTC son kullanım zamanı.</param>
    public sealed record PresignedBlobUploadResult(string Url, DateTimeOffset ExpiresAtUtc);

    /// <summary>
    /// Nesne yükleme ve okuma adresleri için S3 uyumlu imzalayan (MinIO dahil).
    /// </summary>
    public interface IObjectStoragePresigner
    {
        #region Methods

        /// <summary>
        /// Kısa süreli indir/gör GET URL üretir.
        /// </summary>
        Task<string> CreatePresignedGetAsync(
            string objectKey,
            TimeSpan timeToLive,
            CancellationToken cancellationToken);

        /// <summary>
        /// Kısa süreli PUT URL üretir.
        /// </summary>
        Task<PresignedBlobUploadResult> CreatePresignedPutAsync(
            string objectKey,
            string contentType,
            TimeSpan timeToLive,
            CancellationToken cancellationToken);

        #endregion Methods
    }
}
