namespace Azoxia.AdaIsAkademi.Api.Automation
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.Core.Application;

    /// <summary>
    /// Hangfire recurring job that retries failed system-user notification rows.
    /// </summary>
    public class RetryFailedSystemUserNotificationsRecurringJob(ISender sender)
    {
        #region Methods

        /// <summary>
        /// Executes one failed-notification retry sweep.
        /// </summary>
        public Task ExecuteAsync()
            => sender.SendAsync(new RetryFailedSystemUserNotificationsCommand());

        #endregion Methods
    }
}
