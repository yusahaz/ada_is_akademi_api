namespace Azoxia.AdaIsAkademi.SeedRunner;

using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Downloads demo images and uploads them to MinIO/S3 so presigned GET URLs work in the UI.
/// </summary>
internal sealed class ObjectStorageMediaUploader : IAsyncDisposable
{
    private const int MaxWorkersForBinaryMedia = 2_500;
    private const int MaxEmployersForBinaryMedia = 500;

    private readonly AmazonS3Client _client;
    private readonly string _bucket;
    private readonly HttpClient _http;

    private ObjectStorageMediaUploader(AmazonS3Client client, string bucket, HttpClient http)
    {
        _client = client;
        _bucket = bucket;
        _http = http;
    }

    /// <summary>
    /// Builds an uploader when <c>ObjectStorage:ServiceUrl</c> (or env <c>ObjectStorage__ServiceUrl</c>) and credentials are set.
    /// </summary>
    public static ObjectStorageMediaUploader? TryCreate(IConfiguration configuration)
    {
        string? serviceUrl = configuration["ObjectStorage:ServiceUrl"];
        string? accessKey = configuration["ObjectStorage:AccessKey"];
        string? secretKey = configuration["ObjectStorage:SecretKey"];
        string? bucket = configuration["ObjectStorage:BucketName"];
        bool forcePathStyle = configuration.GetValue("ObjectStorage:ForcePathStyle", true);
        string authenticationRegion = configuration["ObjectStorage:RegionName"] ?? "us-east-1";

        if (string.IsNullOrWhiteSpace(serviceUrl)
            || string.IsNullOrWhiteSpace(accessKey)
            || string.IsNullOrWhiteSpace(secretKey)
            || string.IsNullOrWhiteSpace(bucket))
        {
            return null;
        }

        string trimmedUrl = serviceUrl.TrimEnd('/');
        AmazonS3Config config =
            new()
            {
                ServiceURL = trimmedUrl,
                ForcePathStyle = forcePathStyle,
                UseHttp = trimmedUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase),
            };

        // RegionEndpoint + ServiceURL makes the client talk to real AWS S3; MinIO needs only ServiceURL + signing region.
        if (!string.IsNullOrWhiteSpace(authenticationRegion))
        {
            config.AuthenticationRegion = authenticationRegion;
        }

        AWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);
        AmazonS3Client client = new(credentials, config);
        HttpClient http =
            new()
            {
                Timeout = TimeSpan.FromSeconds(45),
            };
        http.DefaultRequestHeaders.UserAgent.ParseAdd("AdaIsAkademi-SeedRunner/1.0 (+https://adaisakademi.local)");
        return new ObjectStorageMediaUploader(client, bucket, http);
    }

    public bool CanUploadForScale(SeedOptions options)
        => !options.SkipMediaUpload
            && options.Workers <= MaxWorkersForBinaryMedia
            && options.Employers <= MaxEmployersForBinaryMedia;

    public async Task SeedWorkerAvatarsAsync(SeederState state, CancellationToken cancellationToken)
    {
        for (int i = 0; i < state.Workers.Count; i++)
        {
            int n = i + 1;
            string objectKey = $"seed/demo/workers/worker-{n:D3}/profile.jpg";
            int img = 1 + (n % 70);
            string url = $"https://i.pravatar.cc/256?img={img}";
            await UploadFromUrlAsync(objectKey, url, cancellationToken).ConfigureAwait(false);
            if (n % 100 == 0 || n == state.Workers.Count)
            {
                Console.WriteLine($"[MinIO] Worker avatar ilerleme: {n}/{state.Workers.Count}");
            }
        }
    }

    public async Task SeedEmployerLogosAsync(SeederState state, CancellationToken cancellationToken)
    {
        for (int i = 0; i < state.Employers.Count; i++)
        {
            int n = i + 1;
            string objectKey = $"seed/demo/employers/employer-{n:D2}/logo.png";
            SeederState.EmployerSeed row = state.Employers[i];
            string shortName = row.Employer.Name.Length == 0
                ? $"E{n}"
                : row.Employer.Name[..Math.Min(24, row.Employer.Name.Length)];
            string label = Uri.EscapeDataString($"{shortName} {n}");
            string url =
                $"https://ui-avatars.com/api/?size=512&background=1a237e&color=fff&bold=true&format=png&name={label}";
            await UploadFromUrlAsync(objectKey, url, cancellationToken).ConfigureAwait(false);
            if (n % 20 == 0 || n == state.Employers.Count)
            {
                Console.WriteLine($"[MinIO] Employer logo ilerleme: {n}/{state.Employers.Count}");
            }
        }
    }

    private async Task UploadFromUrlAsync(string objectKey, string sourceUrl, CancellationToken cancellationToken)
    {
        try
        {
            using HttpResponseMessage response =
                await _http.GetAsync(sourceUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            byte[] bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
            string contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";

            using MemoryStream body = new(bytes);
            PutObjectRequest put =
                new()
                {
                    BucketName = _bucket,
                    Key = objectKey,
                    InputStream = body,
                    ContentType = contentType,
                };

            await _client.PutObjectAsync(put, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MinIO] Uyarı: '{objectKey}' yüklenemedi ({sourceUrl}): {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        _http.Dispose();
        await Task.CompletedTask.ConfigureAwait(false);
    }
}
