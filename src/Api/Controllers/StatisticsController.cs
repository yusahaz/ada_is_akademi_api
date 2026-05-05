namespace Azoxia.AdaIsAkademi.Api.Controllers
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.Core.Api.Controllers;
    using Azoxia.Core.Wrappers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    /// <summary>
    /// Aggregated dashboard statistics for web management cards.
    /// </summary>
    [Tags("Statistics")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public sealed class StatisticsController(IServiceProvider serviceProvider) :
        ApiControllerBase(serviceProvider)
    {
        #region Methods

        /// <summary>Returns dashboard summary counters.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Get dashboard statistics")]
        [EndpointDescription("Returns counters such as total system users, pending approvals, and users activated today.")]
        [ProducesResponseType(typeof(ApiResponse<DashboardStatisticsModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> Overview(
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)]
            GetDashboardStatisticsQuery? query,
            CancellationToken cancellationToken)
            => ExecuteQuery(query ?? new GetDashboardStatisticsQuery(), cancellationToken);

        /// <summary>Returns overdue posting and pending-application counters.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Get overdue job summary")]
        [EndpointDescription("Returns overdue job posting count and pending application count for scheduler/reporting checks.")]
        [ProducesResponseType(typeof(ApiResponse<OverdueJobSummaryModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> OverdueSummary(
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)]
            GetOverdueJobSummaryQuery? query,
            CancellationToken cancellationToken)
            => ExecuteQuery(query ?? new GetOverdueJobSummaryQuery(), cancellationToken);

        /// <summary>Returns overdue alarms CSV export package.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Export overdue alarms as CSV")]
        [EndpointDescription("Returns a CSV package payload for overdue alarms report export.")]
        [ProducesResponseType(typeof(ApiResponse<OverdueAlarmExportPackageModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> ExportOverdueAlarmsCsv(
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)]
            ExportOverdueAlarmsCsvQuery? query,
            CancellationToken cancellationToken)
            => ExecuteQuery(query ?? new ExportOverdueAlarmsCsvQuery(), cancellationToken);

        /// <summary>Returns monetization baseline summary counters.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Get monetization summary")]
        [EndpointDescription("Returns accepted application volume, active employer count, and estimated commission metrics.")]
        [ProducesResponseType(typeof(ApiResponse<MonetizationSummaryModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> MonetizationSummary(
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)]
            GetMonetizationSummaryQuery? query,
            CancellationToken cancellationToken)
            => ExecuteQuery(query ?? new GetMonetizationSummaryQuery(), cancellationToken);

        #endregion Methods
    }
}
