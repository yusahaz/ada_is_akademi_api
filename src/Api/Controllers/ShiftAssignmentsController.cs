namespace Azoxia.AdaIsAkademi.Api.Controllers
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.Core.Api.Controllers;
    using Azoxia.Core.Wrappers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    /// <summary>
    /// Shift assignment endpoints for assignment creation and mutual QR lifecycle.
    /// </summary>
    [Tags("Shift assignments")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public sealed class ShiftAssignmentsController(IServiceProvider serviceProvider) :
        ApiControllerBase(serviceProvider)
    {
        #region Methods

        /// <summary>Checks in worker by QR token hash.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Check in shift assignment")]
        [EndpointDescription("Marks assignment as checked-in when token hash matches and worker owns assignment.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> CheckIn(
            [FromBody] CheckInShiftAssignmentCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Checks out worker assignment.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Check out shift assignment")]
        [EndpointDescription("Marks assignment as checked-out when authenticated worker owns the assignment.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> CheckOut(
            [FromBody] CheckOutShiftAssignmentCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Creates assignment from accepted application.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Create shift assignment")]
        [EndpointDescription("Creates (or returns existing) assignment row for an accepted job application.")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> Create(
            [FromBody] CreateShiftAssignmentCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Lists authenticated worker's assignments.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("List my shift assignments")]
        [EndpointDescription("Lists assignments created for the authenticated worker.")]
        [ProducesResponseType(typeof(PageableApiResponse<WorkerShiftAssignmentListItemModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> MyAssignments(
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] ListMyShiftAssignmentsQuery? query,
            CancellationToken cancellationToken)
            => ExecutePageQuery<WorkerShiftAssignmentListItemModel, PagedQueryResultModel<WorkerShiftAssignmentListItemModel>>(
                query ?? new ListMyShiftAssignmentsQuery(),
                result => result.Items,
                result => result.TotalCount,
                result => result.Limit,
                result => result.Offset,
                cancellationToken);

        /// <summary>Supervisor confirms check-in by QR token hash.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Supervisor check in shift assignment")]
        [EndpointDescription("Completes mutual QR check-in when employer/supervisor confirms assignment check-in token.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> SupervisorCheckIn(
            [FromBody] SupervisorCheckInShiftAssignmentCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        #endregion Methods
    }
}
