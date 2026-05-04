namespace Azoxia.AdaIsAkademi.Api.Controllers
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.Core.Api.Controllers;
    using Azoxia.Core.Api.Responses;
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

        #endregion Methods
    }
}
