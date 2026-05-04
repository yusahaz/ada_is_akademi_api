namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;
    using System;

    /// <summary>
    /// Semantic matching row for worker to open job posting recommendation.
    /// </summary>
    public sealed record SemanticMatchedJobPostingModel(
        int JobPostingId,
        string Title,
        DateOnly ShiftDate,
        TimeOnly ShiftStartTime,
        TimeOnly ShiftEndTime,
        double SimilarityScore) :
        ModelBase;
}
