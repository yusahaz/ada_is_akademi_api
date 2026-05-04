namespace Azoxia.AdaIsAkademi.Api.Controllers
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.Core.Api.Controllers;
    using Azoxia.Core.Api.Responses;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    /// <summary>
    /// System user lifecycle helpers (email verification).
    /// </summary>
    [Tags("System users")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public sealed class SystemUsersController(IServiceProvider serviceProvider) :
        ApiControllerBase(serviceProvider)
    {
        #region Methods

        /// <summary>Bans a system user.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Ban system user")]
        [EndpointDescription("Bans the target account and revokes active refresh tokens.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> Ban(
            [FromBody] BanSystemUserCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Changes a system user's password.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Change system user password")]
        [EndpointDescription("Rotates password hash and salt for the target account.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> ChangePassword(
            [FromBody] ChangeSystemUserPasswordCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Reactivates a banned system user.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Reactivate system user")]
        [EndpointDescription("Reactivates a previously banned system user account.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> Reactivate(
            [FromBody] ReactivateSystemUserCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Registers a new admin account.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Register admin")]
        [EndpointDescription("Creates a new admin system user and attaches to default admin group when present.")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> RegisterAdmin(
            [FromBody] RegisterAdminCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Registers a new employer account and organization.</summary>
        [AllowAnonymous]
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Register employer")]
        [EndpointDescription("Creates employer system user, employer aggregate, and default active supervisor link.")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> RegisterEmployer(
            [FromBody] RegisterEmployerCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Registers a new worker account and worker profile.</summary>
        [AllowAnonymous]
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Register worker")]
        [EndpointDescription("Creates worker system user and linked worker profile row.")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> RegisterWorker(
            [FromBody] RegisterWorkerCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Authenticates the user and returns an access/refresh token pair.</summary>
        [AllowAnonymous]
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Login system user")]
        [EndpointDescription("Authenticates credentials and returns JWT access token with refresh token.")]
        [ProducesResponseType(typeof(ApiResponse<SystemUserTokenModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> Login(
            [FromBody] LoginSystemUserCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Returns the authenticated user's profile summary.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Get current system user")]
        [EndpointDescription("Returns profile information for the currently authenticated user derived from JWT claims.")]
        [ProducesResponseType(typeof(ApiResponse<SystemUserMeModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> Me(
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)]
            GetSystemUserMeQuery? query,
            CancellationToken cancellationToken)
            => ExecuteQuery(query ?? new GetSystemUserMeQuery(), cancellationToken);

        /// <summary>Revokes a device-bound refresh token (logout).</summary>
        [AllowAnonymous]
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Logout system user")]
        [EndpointDescription("Revokes the provided active refresh token for the given user and device pair.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> Logout(
            [FromBody] LogoutSystemUserCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Rotates access and refresh tokens for a valid refresh token.</summary>
        [AllowAnonymous]
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Refresh system user token")]
        [EndpointDescription("Validates refresh token for user + device and issues a fresh token pair.")]
        [ProducesResponseType(typeof(ApiResponse<SystemUserTokenModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> RefreshToken(
            [FromBody] RefreshSystemUserTokenCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Stores a new email verification token for the user.</summary>
        [AllowAnonymous]
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Request email verification")]
        [EndpointDescription("Stores token hash and expiry for the user identified in the command body.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> RequestEmailVerification(
            [FromBody] RequestSystemUserEmailVerificationCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Suspends a system user account.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Suspend system user")]
        [EndpointDescription("Suspends a non-banned system user account.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> Suspend(
            [FromBody] SuspendSystemUserCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        /// <summary>Marks email verified when the token hash matches.</summary>
        [AllowAnonymous]
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Verify email")]
        [EndpointDescription("Activates account when token hash and expiry are valid for the user in the command body.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> VerifyEmail(
            [FromBody] VerifySystemUserEmailCommand command,
            CancellationToken cancellationToken)
            => ExecuteCommand(command, cancellationToken);

        #endregion Methods
    }
}
