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

        #endregion Methods
    }
}
