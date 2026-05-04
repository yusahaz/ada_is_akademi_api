namespace Azoxia.AdaIsAkademi.Api.Controllers
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.Core.Api.Controllers;
    using Azoxia.Core.Api.Responses;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Shift assignment endpoints for assignment creation and QR check-in.
    /// </summary>
    [Tags("Shift assignments")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public sealed class ShiftAssignmentsController(IServiceProvider serviceProvider) :
        ApiControllerBase(serviceProvider)
    {
        #region Methods

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

        #endregion Methods
    }
}
