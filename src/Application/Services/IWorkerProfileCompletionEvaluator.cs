namespace Azoxia.AdaIsAkademi.Application.Services
{
    using Azoxia.AdaIsAkademi.Domain;

    /// <summary>
    /// Computes deterministic profile completion scores for worker aggregates (see docs/tasks/worker-employer-profile-enrichment.md weights).
    /// </summary>
    public interface IWorkerProfileCompletionEvaluator
    {
        #region Methods

        /// <summary>
        /// Calculates completion as an integer percentage in the inclusive range 0–100.
        /// </summary>
        /// <param name="worker">Hydrated aggregate including collections consulted by the scorer.</param>
        int CompletionPercentOf(Worker worker);

        #endregion Methods
    }
}
