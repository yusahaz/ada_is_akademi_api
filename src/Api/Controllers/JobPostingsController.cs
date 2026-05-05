namespace Azoxia.AdaIsAkademi.Api.Controllers
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.Core.Api.Controllers;
    using Azoxia.Core.Wrappers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    /// <summary>
    /// Job-posting commands and queries; anonymous public catalog endpoints (<see cref="GetById"/>, <see cref="ListOpen"/>); management endpoints require JWT with <c>employer_id</c>.
    /// </summary>
    [Tags("Job postings")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public sealed class JobPostingsController(IServiceProvider serviceProvider) :
        ApiControllerBase(serviceProvider)
    {
        #region Methods

        /// <summary>Adds a required skill row.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Add required skill to posting")]
        [EndpointDescription("Adds a required skill row to the posting, or returns existing row when the same tag already exists.")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> AddSkill(
            [FromBody] AddJobPostingSkillCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Cancels a posting.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Cancel job posting")]
        [EndpointDescription("Transitions a cancellable posting to cancelled state.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> Cancel(
            [FromBody] CancelJobPostingCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Completes a posting.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Complete job posting")]
        [EndpointDescription("Marks an open or filled posting as completed.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> Complete(
            [FromBody] CompleteJobPostingCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Creates a draft posting.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Create job posting")]
        [EndpointDescription("Creates a draft posting for an active employer and location.")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> Create(
            [FromBody] CreateJobPostingCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Loads a posting by id.</summary>
        [AllowAnonymous]
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Get job posting by id")]
        [EndpointDescription("Returns posting detail including skill tags and application counts.")]
        [ProducesResponseType(typeof(ApiResponse<JobPostingDetailModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> GetById(
            [FromBody] GetJobPostingByIdQuery query,
            CancellationToken cancellationToken)
            => ExecuteQuery(query, cancellationToken);

        /// <summary>Lists postings owned by the authenticated employer (all statuses, soft-deleted excluded).</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("List job postings for employer")]
        [EndpointDescription("Requires Bearer JWT with a positive employer_id claim; request body may be `{}` — employer scope is taken only from the token.")]
        [ProducesResponseType(typeof(PageableApiResponse<JobPostingSummaryModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> ListByEmployer(
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)]
            ListJobPostingsByEmployerIdQuery? query,
            CancellationToken cancellationToken)
            => ExecutePageQuery<JobPostingSummaryModel, PagedQueryResultModel<JobPostingSummaryModel>>(
                query ?? new ListJobPostingsByEmployerIdQuery(),
                result => result.Items,
                result => result.TotalCount,
                result => result.Limit,
                result => result.Offset,
                cancellationToken);

        /// <summary>Lists open postings (public catalog).</summary>
        [AllowAnonymous]
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("List open job postings")]
        [EndpointDescription("Returns postings currently open for applications.")]
        [ProducesResponseType(typeof(PageableApiResponse<JobPostingSummaryModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> ListOpen(
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)]
            ListOpenJobPostingsQuery? query,
            CancellationToken cancellationToken)
            => ExecutePageQuery<JobPostingSummaryModel, PagedQueryResultModel<JobPostingSummaryModel>>(
                query ?? new ListOpenJobPostingsQuery(),
                result => result.Items,
                result => result.TotalCount,
                result => result.Limit,
                result => result.Offset,
                cancellationToken);

        /// <summary>Lists semantically matched open postings for a worker embedding.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("List semantic matched job postings")]
        [EndpointDescription("Ranks open postings by cosine similarity between authenticated worker embedding and posting description embedding.")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<SemanticMatchedJobPostingModel>>), StatusCodes.Status200OK)]
        public Task<IActionResult> ListSemanticMatched(
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] ListSemanticMatchedJobPostingsQuery? query,
            CancellationToken cancellationToken)
            => ExecuteQuery(query ?? new ListSemanticMatchedJobPostingsQuery(), cancellationToken);

        /// <summary>Publishes a draft posting.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Publish job posting")]
        [EndpointDescription("Publishes a draft posting so workers can apply.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> Publish(
            [FromBody] PublishJobPostingCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Removes a posting skill row.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Remove required skill from posting")]
        [EndpointDescription("Removes a skill row from the posting by skill id.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> RemoveSkill(
            [FromBody] RemoveJobPostingSkillCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Updates a draft posting.</summary>
        [HttpPut]
        [Consumes("application/json")]
        [EndpointSummary("Update job posting")]
        [EndpointDescription("Updates editable fields on a draft posting.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> Update(
            [FromBody] UpdateJobPostingCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        #endregion Methods
    }
}
