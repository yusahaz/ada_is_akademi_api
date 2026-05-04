namespace Azoxia.AdaIsAkademi.Api.Controllers
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.Core.Api.Controllers;
    using Azoxia.Core.Api.Responses;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Worker read and profile skill endpoints.
    /// </summary>
    [Tags("Workers")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public sealed class WorkersController(IServiceProvider serviceProvider) :
        ApiControllerBase(serviceProvider)
    {
        #region Methods

        /// <summary>Adds a skill to worker profile.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Add worker skill")]
        [EndpointDescription("Adds a normalized skill tag to the worker profile.")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> AddSkill(
            [FromBody] AddWorkerSkillCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Gets worker detail by id.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Get worker by id")]
        [EndpointDescription("Returns worker detail model including normalized skill tags.")]
        [ProducesResponseType(typeof(ApiResponse<WorkerDetailModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> GetById(
            [FromBody] GetWorkerByIdQuery query,
            CancellationToken cancellationToken)
            => ExecuteQuery(query, cancellationToken);

        /// <summary>Gets personalized notification preview with push/email fallback.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Get worker notification preview")]
        [EndpointDescription("Builds notification preview for the authenticated worker and falls back to email when push token is missing.")]
        [ProducesResponseType(typeof(ApiResponse<WorkerNotificationPreviewModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> NotificationPreview(
            [FromBody] GetWorkerPersonalizedNotificationPreviewQuery query,
            CancellationToken cancellationToken)
            => ExecuteQuery(query, cancellationToken);

        #endregion Methods
    }
}
