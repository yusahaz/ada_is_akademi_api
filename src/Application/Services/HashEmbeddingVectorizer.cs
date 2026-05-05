namespace Azoxia.AdaIsAkademi.Application
{
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Lightweight deterministic vectorizer used for semantic matching baseline.
    /// </summary>
    internal class HashEmbeddingVectorizer : IEmbeddingVectorizer
    {
        #region Methods

        /// <inheritdoc />
        public float[] Vectorize(string text, int dimension)
        {
            if (dimension <= 0)
            {
                return [];
            }

            string normalized = (text ?? string.Empty).Trim().ToLowerInvariant();
            if (normalized.Length == 0)
            {
                return new float[dimension];
            }

            float[] vector = new float[dimension];
            string[] tokens = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (tokens.Length == 0)
            {
                return vector;
            }

            foreach (string token in tokens)
            {
                byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    int bucket = i % dimension;
                    vector[bucket] += hashBytes[i] / 255f;
                }
            }

            float norm = MathF.Sqrt(vector.Sum(x => x * x));
            if (norm <= 0f)
            {
                return vector;
            }

            for (int i = 0; i < vector.Length; i++)
            {
                vector[i] /= norm;
            }

            return vector;
        }

        #endregion Methods
    }
}
