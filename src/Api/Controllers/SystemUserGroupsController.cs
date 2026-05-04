namespace Azoxia.AdaIsAkademi.Api.Controllers
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.Core.Api.Controllers;
    using Azoxia.Core.Api.Responses;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// System user group authorization management endpoints.
    /// </summary>
    [Tags("System user groups")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public sealed class SystemUserGroupsController(IServiceProvider serviceProvider) :
        ApiControllerBase(serviceProvider)
    {
        #region Methods

        /// <summary>Activates a system user group.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Activate system user group")]
        [EndpointDescription("Re-enables a system user group for authorization evaluations.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> Activate(
            [FromBody] ActivateSystemUserGroupCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Adds an allow/deny permission row.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Add system user group permission")]
        [EndpointDescription("Adds or updates permission effect row for a system user group.")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> AddPermission(
            [FromBody] AddSystemUserGroupPermissionCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Deactivates a system user group.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Deactivate system user group")]
        [EndpointDescription("Disables a system user group so it no longer contributes rules.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> Deactivate(
            [FromBody] DeactivateSystemUserGroupCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        #endregion Methods
    }
}
