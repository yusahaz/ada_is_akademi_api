namespace Azoxia.AdaIsAkademi.Api.Automation
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.Core.Application;

    /// <summary>
    /// Hangfire recurring job that triggers overdue alarm sweep command.
    /// </summary>
    public class OverdueAlarmRecurringJob(ISender sender)
    {
        #region Methods

        /// <summary>
        /// Executes one overdue-alarm sweep cycle.
        /// </summary>
        public Task ExecuteAsync()
            => sender.SendAsync(new RunOverdueAlarmSweepCommand());

        #endregion Methods
    }
}
