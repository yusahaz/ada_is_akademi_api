namespace Azoxia.AdaIsAkademi.SeedRunner;

using Azoxia.AdaIsAkademi.Domain;

/// <summary>
/// Cross-stage mutable context for one seed run (shared EF tracking scope).
/// </summary>
internal sealed class SeederState
{
    #region Properties

    /// <summary>
    /// Maps catalog keys (e.g. C_GARSON) to persisted <see cref="JobCategory"/> identifiers.
    /// </summary>
    internal Dictionary<string, int> CategoryIdByKey { get; } = new(StringComparer.Ordinal);

    /// <summary>
    /// Employer aggregates created or resolved during this run.
    /// </summary>
    internal List<EmployerSeed> Employers { get; } = [];

    /// <summary>
    /// Shift assignments eligible for payout simulation (completed checkout).
    /// </summary>
    internal List<PayoutSource> PayoutSources { get; } = [];

    /// <summary>
    /// Job postings touched when generating applications (for status transitions).
    /// </summary>
    internal List<JobPosting> Postings { get; } = [];

    /// <summary>
    /// Worker profiles created or resolved during this run.
    /// </summary>
    internal List<WorkerSeed> Workers { get; } = [];

    #endregion Properties

    #region Nested types

    /// <summary>
    /// One employer with primary login user and operational locations.
    /// </summary>
    internal sealed class EmployerSeed
    {
        internal required Employer Employer { get; init; }

        internal required List<EmployerLocation> Locations { get; init; }

        internal required SystemUser PrimaryUser { get; init; }

        internal required List<SystemUser> ExtraSupervisorUsers { get; init; }
    }

    /// <summary>
    /// Assignment ready for <see cref="WorkerPayout"/> creation.
    /// </summary>
    internal sealed record PayoutSource(int AssignmentId, int EmployerId, int WorkerId, int JobPostingId);

    /// <summary>
    /// Worker profile with login user and skill cluster label.
    /// </summary>
    internal sealed class WorkerSeed
    {
        internal required string Cluster { get; init; }

        internal required IReadOnlyList<string> SkillTags { get; init; }

        internal required SystemUser User { get; init; }

        internal required Worker Worker { get; init; }
    }

    #endregion Nested types
}
