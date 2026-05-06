namespace Azoxia.AdaIsAkademi.Api.Automation
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.Core.Application;

    /// <summary>
    /// Hangfire recurring job that processes queued worker CV uploads for extraction.
    /// </summary>
    public class CvExtractionRecurringJob(ISender sender)
    {
        #region Methods

        /// <summary>
        /// Executes one CV extraction sweep cycle.
        /// </summary>
        public Task ExecuteAsync()
            => sender.SendAsync(new RunCvExtractionSweepCommand());

        #endregion Methods
    }
}
