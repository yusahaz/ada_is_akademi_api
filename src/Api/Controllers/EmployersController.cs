namespace Azoxia.AdaIsAkademi.Api.Controllers
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.Core.Api.Controllers;
    using Azoxia.Core.Wrappers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    /// <summary>
    /// Employer read and lifecycle management endpoints.
    /// </summary>
    [Tags("Employers")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public sealed class EmployersController(IServiceProvider serviceProvider) :
        ApiControllerBase(serviceProvider)
    {
        #region Methods

        /// <summary>Activates an employer.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Activate employer")]
        [EndpointDescription("Activates a non-banned employer account.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> Activate(
            [FromBody] ActivateEmployerCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Bans an employer.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Ban employer")]
        [EndpointDescription("Transitions employer account into banned state.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> Ban(
            [FromBody] BanEmployerCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Adds employer location for authenticated employer.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Add employer location")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> AddLocation(
            [FromBody] AddEmployerLocationCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand<int>(command, cancellationToken);

        /// <summary>Adds employer supervisor for authenticated employer.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Add employer supervisor")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> AddSupervisor(
            [FromBody] AddEmployerSupervisorCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand<int>(command, cancellationToken);

        /// <summary>Removes employer supervisor for authenticated employer.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Remove employer supervisor")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> RemoveSupervisor(
            [FromBody] RemoveEmployerSupervisorCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Soft deletes an employer and linked employer users.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Delete employer")]
        [EndpointDescription("Soft deletes employer and all users scoped to the employer.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> Delete(
            [FromBody] DeleteEmployerCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Gets an employer detail model by id.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Get employer by id")]
        [EndpointDescription("Returns cached employer detail read model by primary key.")]
        [ProducesResponseType(typeof(ApiResponse<EmployerDetailModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> GetById(
            [FromBody] GetEmployerByIdQuery query,
            CancellationToken cancellationToken)
            => ExecuteQuery(query, cancellationToken);

        /// <summary>Lists employers with filtering and paging support.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("List employers")]
        [EndpointDescription("Returns filtered employer rows with paging options.")]
        [ProducesResponseType(typeof(PageableApiResponse<EmployerListItemModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> List(
            [FromBody] ListEmployersQuery query,
            CancellationToken cancellationToken)
            => ExecutePageQuery<EmployerListItemModel, PagedQueryResultModel<EmployerListItemModel>>(
                query,
                result => result.Items,
                result => result.TotalCount,
                result => result.Limit,
                result => result.Offset,
                cancellationToken);

        /// <summary>Gets employer commission policy by employer id.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Get employer commission policy")]
        [EndpointDescription("Returns commission policy detail read model by employer id.")]
        [ProducesResponseType(typeof(ApiResponse<EmployerCommissionPolicyModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> GetCommissionPolicy(
            [FromBody] GetEmployerCommissionPolicyQuery query,
            CancellationToken cancellationToken)
            => ExecuteQuery(query, cancellationToken);

        /// <summary>Gets employer-specific commission estimate metrics.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Get employer commission estimate")]
        [EndpointDescription("Returns accepted volume and estimated commission amounts for the given employer.")]
        [ProducesResponseType(typeof(ApiResponse<EmployerCommissionEstimateModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> GetCommissionEstimate(
            [FromBody] GetEmployerCommissionEstimateQuery query,
            CancellationToken cancellationToken)
            => ExecuteQuery(query, cancellationToken);

        /// <summary>Lists employer commission summaries for monetization management.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("List employer commission summaries")]
        [EndpointDescription("Returns active employers ordered by monetization estimate metrics.")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<EmployerCommissionListItemModel>>), StatusCodes.Status200OK)]
        public Task<IActionResult> ListCommissionSummaries(
            [FromBody] ListEmployerCommissionSummariesQuery query,
            CancellationToken cancellationToken)
            => ExecuteQuery(query, cancellationToken);

        /// <summary>Exports employer commission policies in CSV format package.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Export employer commission policies CSV")]
        [EndpointDescription("Returns CSV package payload for employer commission policy records.")]
        [ProducesResponseType(typeof(ApiResponse<EmployerCommissionPolicyExportPackageModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> ExportCommissionPoliciesCsv(
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)]
            ExportEmployerCommissionPoliciesCsvQuery? query,
            CancellationToken cancellationToken)
            => ExecuteQuery(query ?? new ExportEmployerCommissionPoliciesCsvQuery(), cancellationToken);

        /// <summary>Generates idempotent commission receivable for employer period.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Generate commission receivable")]
        [EndpointDescription("Creates or returns existing commission receivable id for the same employer and period.")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> GenerateCommissionReceivable(
            [FromBody] GenerateCommissionReceivableCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand<int>(command, cancellationToken);

        /// <summary>Gets commission receivable detail by employer and period.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Get commission receivable by period")]
        [EndpointDescription("Returns commission receivable detail row for employer and billing period.")]
        [ProducesResponseType(typeof(ApiResponse<CommissionReceivableDetailModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> GetCommissionReceivableByPeriod(
            [FromBody] GetCommissionReceivableByPeriodQuery query,
            CancellationToken cancellationToken)
            => ExecuteQuery(query, cancellationToken);

        /// <summary>Lists commission receivable rows for an employer.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("List commission receivables by employer")]
        [EndpointDescription("Returns commission receivable list for the given employer and row limit.")]
        [ProducesResponseType(typeof(PageableApiResponse<CommissionReceivableListItemModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> ListCommissionReceivables(
            [FromBody] ListCommissionReceivablesByEmployerQuery query,
            CancellationToken cancellationToken)
            => ExecutePageQuery<CommissionReceivableListItemModel, PagedQueryResultModel<CommissionReceivableListItemModel>>(
                query,
                result => result.Items,
                result => result.TotalCount,
                result => result.Limit,
                result => result.Offset,
                cancellationToken);

        /// <summary>Suspends an employer.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Suspend employer")]
        [EndpointDescription("Suspends an employer account when policy allows.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> Suspend(
            [FromBody] SuspendEmployerCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Sets employer commission rate policy.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Set employer commission policy")]
        [EndpointDescription("Sets commission rate for monetization policy management.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> SetCommissionPolicy(
            [FromBody] SetEmployerCommissionRateCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        #endregion Methods
    }
}
