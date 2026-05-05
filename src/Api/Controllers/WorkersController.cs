namespace Azoxia.AdaIsAkademi.Api.Controllers
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.Core.Api.Controllers;
    using Azoxia.Core.Wrappers;
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

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Add worker availability")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> AddAvailability(
            [FromBody] AddWorkerAvailabilityCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Remove worker availability")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> RemoveAvailability(
            [FromBody] RemoveWorkerAvailabilityCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Add worker certificate")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> AddCertificate(
            [FromBody] AddWorkerCertificateCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Remove worker certificate")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> RemoveCertificate(
            [FromBody] RemoveWorkerCertificateCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Add worker education")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> AddEducation(
            [FromBody] AddWorkerEducationCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Remove worker education")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> RemoveEducation(
            [FromBody] RemoveWorkerEducationCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Add worker experience")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> AddExperience(
            [FromBody] AddWorkerExperienceCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Remove worker experience")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> RemoveExperience(
            [FromBody] RemoveWorkerExperienceCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Add worker language")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> AddLanguage(
            [FromBody] AddWorkerLanguageCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Remove worker language")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> RemoveLanguage(
            [FromBody] RemoveWorkerLanguageCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Add worker reference")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> AddReference(
            [FromBody] AddWorkerReferenceCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Remove worker reference")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> RemoveReference(
            [FromBody] RemoveWorkerReferenceCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Remove worker skill")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> RemoveSkill(
            [FromBody] RemoveWorkerSkillCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Delete worker")]
        [EndpointDescription("Soft deletes worker and its linked system user.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> Delete(
            [FromBody] DeleteWorkerCommand command,
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

        /// <summary>Gets full worker detail by id.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Get worker detail")]
        [EndpointDescription("Returns full worker profile detail with account summary and all profile collections.")]
        [ProducesResponseType(typeof(ApiResponse<WorkerFullDetailModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> GetDetail(
            [FromBody] GetWorkerDetailQuery query,
            CancellationToken cancellationToken)
            => ExecuteQuery(query, cancellationToken);

        /// <summary>Lists workers with filtering and paging support.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("List workers")]
        [EndpointDescription("Returns filtered worker rows with account status and email filters.")]
        [ProducesResponseType(typeof(PageableApiResponse<WorkerListItemModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> List(
            [FromBody] ListWorkersQuery query,
            CancellationToken cancellationToken)
            => ExecutePageQuery<WorkerListItemModel, PagedQueryResultModel<WorkerListItemModel>>(
                query,
                result => result.Items,
                result => result.TotalCount,
                result => result.Limit,
                result => result.Offset,
                cancellationToken);

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

        /// <summary>Confirms processing payout for authenticated worker.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Confirm worker payout")]
        [EndpointDescription("Worker confirms payout transfer and closes payout lifecycle.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> ConfirmPayout(
            [FromBody] ConfirmWorkerPayoutCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Updates authenticated worker profile basics.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Update worker profile")]
        [EndpointDescription("Updates nationality and university fields for the authenticated worker.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> UpdateProfile(
            [FromBody] UpdateWorkerProfileCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        #endregion Methods
    }
}
