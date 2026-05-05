namespace Azoxia.AdaIsAkademi.Api.Controllers
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.Core.Api.Controllers;
    using Azoxia.Core.Wrappers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    /// <summary>
    /// Job-application commands plus posting-scoped queries (Bearer JWT maps <c>employer_id</c> employer flows and <c>worker_id</c> worker apply/withdraw flows).
    /// </summary>
    [Tags("Job applications")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public sealed class JobApplicationsController(IServiceProvider serviceProvider) :
        ApiControllerBase(serviceProvider)
    {
        #region Methods

        /// <summary>Accepts an application.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Accept job application")]
        [EndpointDescription("Accepts a pending application when capacity allows.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> Accept(
            [FromBody] AcceptJobPostingApplicationCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Lists applications for a posting (employer scope comes from JWT employer_id claim).</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("List job applications")]
        [EndpointDescription("Lists applications for a posting; JWT employer_id must match posting owner.")]
        [ProducesResponseType(typeof(PageableApiResponse<JobApplicationListItemModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> List(
            [FromBody] ListJobApplicationsByJobPostingIdQuery query,
            CancellationToken cancellationToken)
            => ExecutePageQuery<JobApplicationListItemModel, PagedQueryResultModel<JobApplicationListItemModel>>(
                query,
                result => result.Items,
                result => result.TotalCount,
                result => result.Limit,
                result => result.Offset,
                cancellationToken);

        /// <summary>Lists authenticated worker's own applications.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("List my job applications")]
        [EndpointDescription("Lists applications created by the authenticated worker.")]
        [ProducesResponseType(typeof(PageableApiResponse<WorkerJobApplicationListItemModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> MyApplications(
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] ListMyJobApplicationsQuery? query,
            CancellationToken cancellationToken)
            => ExecutePageQuery<WorkerJobApplicationListItemModel, PagedQueryResultModel<WorkerJobApplicationListItemModel>>(
                query ?? new ListMyJobApplicationsQuery(),
                result => result.Items,
                result => result.TotalCount,
                result => result.Limit,
                result => result.Offset,
                cancellationToken);

        /// <summary>Rejects an application.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Reject job application")]
        [EndpointDescription("Rejects an application; optional reason on the command body.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> Reject(
            [FromBody] RejectJobPostingApplicationCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Submits a new application.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Submit job application")]
        [EndpointDescription("Submits a worker application to an open posting.")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> Submit(
            [FromBody] SubmitJobPostingApplicationCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Withdraws a pending application.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Withdraw job application")]
        [EndpointDescription("Withdraws a pending application for the posting.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> Withdraw(
            [FromBody] WithdrawJobPostingApplicationCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        #endregion Methods
    }
}
