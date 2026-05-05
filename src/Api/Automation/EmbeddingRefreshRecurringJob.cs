namespace Azoxia.AdaIsAkademi.Api.Automation
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.Core.Application;

    /// <summary>
    /// Hangfire recurring job that refreshes worker and posting embeddings.
    /// </summary>
    public class EmbeddingRefreshRecurringJob(ISender sender)
    {
        #region Methods

        /// <summary>
        /// Executes one embedding refresh sweep cycle.
        /// </summary>
        public Task ExecuteAsync()
            => sender.SendAsync(new RunEmbeddingRefreshSweepCommand());

        #endregion Methods
    }
}
