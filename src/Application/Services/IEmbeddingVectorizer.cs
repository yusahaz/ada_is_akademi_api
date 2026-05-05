namespace Azoxia.AdaIsAkademi.Application
{
    /// <summary>
    /// Produces deterministic numeric vectors from textual content.
    /// </summary>
    internal interface IEmbeddingVectorizer
    {
        /// <summary>
        /// Builds an embedding vector for the provided text.
        /// </summary>
        /// <param name="text">Source text used for vectorization.</param>
        /// <param name="dimension">Requested embedding dimension.</param>
        /// <returns>Embedding vector with requested dimension.</returns>
        float[] Vectorize(string text, int dimension);
    }
}
