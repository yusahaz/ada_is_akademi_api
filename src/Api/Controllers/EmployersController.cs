namespace Azoxia.AdaIsAkademi.Api.Controllers
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.Core.Api.Controllers;
    using Azoxia.Core.Api.Responses;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

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
