namespace Azoxia.AdaIsAkademi.Api.Controllers
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.Core.Api.Controllers;
    using Azoxia.Core.Wrappers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

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

        /// <summary>Adds an availability window to the authenticated worker profile.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Add worker availability")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> AddAvailability(
            [FromBody] AddWorkerAvailabilityCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Adds a certificate row to the authenticated worker profile.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Add worker certificate")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> AddCertificate(
            [FromBody] AddWorkerCertificateCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Adds an education row to the authenticated worker profile.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Add worker education")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> AddEducation(
            [FromBody] AddWorkerEducationCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Adds a work experience row to the authenticated worker profile.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Add worker experience")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> AddExperience(
            [FromBody] AddWorkerExperienceCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Adds a language proficiency row to the authenticated worker profile.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Add worker language")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> AddLanguage(
            [FromBody] AddWorkerLanguageCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Adds a reference row to the authenticated worker profile.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Add worker reference")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> AddReference(
            [FromBody] AddWorkerReferenceCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

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

        /// <summary>Removes persisted profile photo metadata (does not sweep MinIO objects).</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Clear worker profile photo")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> ClearProfilePhoto(
            [FromBody] ClearWorkerProfilePhotoCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

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

        /// <summary>Persists uploaded profile photo object key after PUT success.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Confirm worker profile photo")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> ConfirmProfilePhotoUpload(
            [FromBody] ConfirmWorkerProfilePhotoUploadCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Soft deletes worker and its linked system user.</summary>
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
        [EndpointSummary("Get worker by id (employer)")]
        [EndpointDescription("Employer-safe worker summary for a worker who shares a job application with the authenticated employer.")]
        [ProducesResponseType(typeof(ApiResponse<WorkerEmployerSafeDetailModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> GetById(
            [FromBody] GetWorkerByIdQuery query,
            CancellationToken cancellationToken)
            => ExecuteQuery(query, cancellationToken);

        /// <summary>Gets full worker detail by id.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Get worker detail (employer)")]
        [EndpointDescription("Employer-safe full worker profile for a worker tied to the employer through a job application.")]
        [ProducesResponseType(typeof(ApiResponse<WorkerEmployerSafeFullDetailModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> GetDetail(
            [FromBody] GetWorkerDetailQuery query,
            CancellationToken cancellationToken)
            => ExecuteQuery(query, cancellationToken);

        /// <summary>Records an employer-initiated worker profile view (deduped per UTC calendar day).</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Record employer worker profile view")]
        [EndpointDescription("Increments the employer/worker profile view counter when the authenticated employer shares a job application with the worker; at most one increment per UTC day.")]
        [ProducesResponseType(typeof(ApiResponse<RecordEmployerWorkerProfileViewResultModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> RecordEmployerWorkerProfileView(
            [FromBody] RecordEmployerWorkerProfileViewCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand<RecordEmployerWorkerProfileViewResultModel>(command, cancellationToken);

        /// <summary>Returns short-lived GET URL for worker profile photo.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Get worker profile photo URL")]
        [ProducesResponseType(typeof(ApiResponse<MediaBlobViewUrlModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> GetProfilePhotoViewUrl(
            [FromBody] GetWorkerProfilePhotoViewUrlQuery query,
            CancellationToken cancellationToken)
            => ExecuteQuery(query, cancellationToken);

        /// <summary>Gets authenticated worker full profile with private matching preferences.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Get worker self full detail")]
        [EndpointDescription("Returns the full worker-facing profile including salary expectations, interested job categories, and profile completion percentage.")]
        [ProducesResponseType(typeof(ApiResponse<WorkerSelfFullDetailModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> GetSelfFullDetail(
            [FromBody] GetWorkerSelfFullDetailQuery query,
            CancellationToken cancellationToken)
            => ExecuteQuery(query, cancellationToken);

        /// <summary>Gets authenticated worker summary with private matching preferences.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Get worker self summary")]
        [EndpointDescription("Returns the worker-facing profile summary including salary expectations, interested job categories, and a deterministic profile completion percentage.")]
        [ProducesResponseType(typeof(ApiResponse<WorkerSelfDetailModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> GetSelfSummary(
            [FromBody] GetWorkerSelfDetailQuery query,
            CancellationToken cancellationToken)
            => ExecuteQuery(query, cancellationToken);

        /// <summary>Begins worker profile photo upload (presigned PUT).</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Init worker profile photo upload")]
        [EndpointDescription("Returns an object key plus presigned PUT URL to stream the portrait to MinIO-compatible storage; call confirm after success.")]
        [ProducesResponseType(typeof(ApiResponse<ObjectStorageUploadInitModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> InitProfilePhotoUpload(
            [FromBody] InitWorkerProfilePhotoUploadCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand<ObjectStorageUploadInitModel>(command, cancellationToken);

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

        /// <summary>Returns live status feed for worker dashboard polling.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Get worker live status feed")]
        [EndpointDescription("Returns assignment and matching updates for the authenticated worker in near real-time polling format.")]
        [ProducesResponseType(typeof(ApiResponse<WorkerLiveStatusFeedModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> LiveStatus(
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] GetWorkerLiveStatusFeedQuery? query,
            CancellationToken cancellationToken)
            => ExecuteQuery(query ?? new GetWorkerLiveStatusFeedQuery(), cancellationToken);

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

        /// <summary>Removes an availability window from the authenticated worker profile.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Remove worker availability")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> RemoveAvailability(
            [FromBody] RemoveWorkerAvailabilityCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Removes a certificate row from the authenticated worker profile.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Remove worker certificate")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> RemoveCertificate(
            [FromBody] RemoveWorkerCertificateCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Removes an education row from the authenticated worker profile.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Remove worker education")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> RemoveEducation(
            [FromBody] RemoveWorkerEducationCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Removes a work experience row from the authenticated worker profile.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Remove worker experience")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> RemoveExperience(
            [FromBody] RemoveWorkerExperienceCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Removes a language row from the authenticated worker profile.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Remove worker language")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> RemoveLanguage(
            [FromBody] RemoveWorkerLanguageCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Removes a reference row from the authenticated worker profile.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Remove worker reference")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> RemoveReference(
            [FromBody] RemoveWorkerReferenceCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Removes a skill tag from the authenticated worker profile.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Remove worker skill")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> RemoveSkill(
            [FromBody] RemoveWorkerSkillCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Sends worker notification through push with fallback.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Send worker notification")]
        [EndpointDescription("Dispatches notification to worker using push and falls back to email/in-app when needed.")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> SendNotification(
            [FromBody] SendWorkerNotificationCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand<int>(command, cancellationToken);

        /// <summary>Updates worker-facing bio text.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Update worker bio")]
        [EndpointDescription("Replaces the authenticated worker's short about text (null/whitespace clears).")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> UpdateBio(
            [FromBody] UpdateWorkerBioCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Updates authenticated worker matching preferences.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Update worker matching preferences")]
        [EndpointDescription("Sets optional expected salary bounds as Money-compatible amount/currency pairs per bound, and interested job categories (worker scope only; omitted from employer read models).")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> UpdateMatchingPreferences(
            [FromBody] UpdateWorkerMatchingPreferencesCommand command,
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

        /// <summary>Replaces outbound social links visible on worker self endpoints.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Update worker social links")]
        [EndpointDescription("Replaces the HTTPS social profile list; empty list clears all.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> UpdateSocialLinks(
            [FromBody] UpdateWorkerSocialLinksCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        #endregion Methods
    }
}
