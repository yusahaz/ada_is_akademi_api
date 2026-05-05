namespace Azoxia.AdaIsAkademi.Application
{
    /// <summary>
    /// Presigned yükleme yanı özeti (komut çıktıları ve API modellerinde ortak kullanım).
    /// </summary>
    public sealed record ObjectStorageUploadInitModel(
        string ObjectKey,
        string UploadUrl,
        DateTimeOffset UploadExpiresAtUtc);

    /// <summary>
    /// Presigned görüntüleme/indir adresi özeti.
    /// </summary>
    public sealed record MediaBlobViewUrlModel(string Url, DateTimeOffset UrlExpiresAtUtc);
}
