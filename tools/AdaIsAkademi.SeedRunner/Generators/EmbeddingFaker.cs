namespace Azoxia.AdaIsAkademi.SeedRunner.Generators;

using System.Globalization;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Deterministic pseudo-embeddings for pgvector semantic-match smoke tests (not OpenAI vectors).
/// </summary>
internal static class EmbeddingFaker
{
    #region Fields

    /// <summary>
    /// OpenAI text-embedding-3-small dimension used by the domain model.
    /// </summary>
    internal const int Dimensions = 1536;

    #endregion Fields

    #region Utils

    /// <summary>
    /// Builds an L2-normalized vector of length <see cref="Dimensions"/> from arbitrary seed text.
    /// </summary>
    /// <param name="seedText">Stable seed string (skill join, description, etc.).</param>
    /// <returns>Normalized float array suitable for cosine similarity.</returns>
    internal static float[] GenerateDeterministic(string seedText)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(seedText));
        uint seed = BitConverter.ToUInt32(hash, 0);
        var rng = new Random((int)seed);

        var v = new float[Dimensions];
        double sumSq = 0;
        for (int i = 0; i < Dimensions; i++)
        {
            double x = rng.NextDouble() * 2d - 1d;
            v[i] = (float)x;
            sumSq += x * x;
        }

        float norm = (float)Math.Sqrt(sumSq);
        if (norm > 1e-12f)
        {
            for (int i = 0; i < Dimensions; i++)
            {
                v[i] /= norm;
            }
        }

        return v;
    }

    /// <summary>
    /// Friendly preview for logs (first few dimensions).
    /// </summary>
    internal static string Preview(float[] v)
        => string.Join(
            ", ",
            v.Take(4).Select(x => x.ToString("F4", CultureInfo.InvariantCulture)));

    #endregion Utils
}
