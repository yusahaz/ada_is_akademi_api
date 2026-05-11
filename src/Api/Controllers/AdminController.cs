namespace Azoxia.AdaIsAkademi.Api.Controllers
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Api.Controllers;
    using Azoxia.Core.Wrappers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    /// <summary>
    /// Admin-only endpoints used by admin web screens.
    /// </summary>
    [Tags("Admin")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public sealed class AdminController(IServiceProvider serviceProvider) :
        ApiControllerBase(serviceProvider)
    {
        private bool IsAdminCaller =>
            HttpContext.User.FindFirst("system_user_type")?.Value == ((int)SystemUserType.Admin).ToString();

        private IActionResult? EnsureAdmin()
            => IsAdminCaller ? null : Forbid();

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Admin dashboard overview")]
        [ProducesResponseType(typeof(ApiResponse<DashboardStatisticsModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> Overview(
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] GetDashboardStatisticsQuery? query,
            CancellationToken cancellationToken)
            => EnsureAdmin() is IActionResult deny
                ? Task.FromResult(deny)
                : ExecuteQuery(query ?? new GetDashboardStatisticsQuery(), cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Admin commission revenue series")]
        [ProducesResponseType(typeof(ApiResponse<CommissionRevenueSeriesModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> CommissionRevenueSeries(
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] GetCommissionRevenueSeriesQuery? query,
            CancellationToken cancellationToken)
            => EnsureAdmin() is IActionResult deny
                ? Task.FromResult(deny)
                : ExecuteQuery(query ?? new GetCommissionRevenueSeriesQuery(), cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Admin list employers")]
        [ProducesResponseType(typeof(PageableApiResponse<EmployerListItemModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> ListEmployers(
            [FromBody] ListEmployersQuery query,
            CancellationToken cancellationToken)
            => EnsureAdmin() is IActionResult deny
                ? Task.FromResult(deny)
                : ExecutePageQuery<EmployerListItemModel, PagedQueryResultModel<EmployerListItemModel>>(
                    query,
                    result => result.Items,
                    result => result.TotalCount,
                    result => result.Limit,
                    result => result.Offset,
                    cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Admin get employer by id")]
        [ProducesResponseType(typeof(ApiResponse<EmployerDetailModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> GetEmployerById(
            [FromBody] GetEmployerByIdQuery query,
            CancellationToken cancellationToken)
            => EnsureAdmin() is IActionResult deny
                ? Task.FromResult(deny)
                : ExecuteQuery(query, cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Admin update employer profile")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> UpdateEmployerProfile(
            [FromBody] UpdateEmployerProfileCommand command,
            CancellationToken cancellationToken)
            => EnsureAdmin() is IActionResult deny
                ? Task.FromResult(deny)
                : ExecuteCommand(command, cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Admin delete employer")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> DeleteEmployer(
            [FromBody] DeleteEmployerCommand command,
            CancellationToken cancellationToken)
            => EnsureAdmin() is IActionResult deny
                ? Task.FromResult(deny)
                : ExecuteCommand(command, cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Admin list employer locations")]
        [ProducesResponseType(typeof(PageableApiResponse<EmployerLocationListItemModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> ListEmployerLocations(
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] ListEmployerLocationsQuery? query,
            CancellationToken cancellationToken)
            => EnsureAdmin() is IActionResult deny
                ? Task.FromResult(deny)
                : ExecutePageQuery<EmployerLocationListItemModel, PagedQueryResultModel<EmployerLocationListItemModel>>(
                    query ?? new ListEmployerLocationsQuery(),
                    result => result.Items,
                    result => result.TotalCount,
                    result => result.Limit,
                    result => result.Offset,
                    cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Admin list employer supervisors")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<EmployerSupervisorListItemModel>>), StatusCodes.Status200OK)]
        public Task<IActionResult> ListEmployerSupervisors(
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] ListEmployerSupervisorsQuery? query,
            CancellationToken cancellationToken)
            => EnsureAdmin() is IActionResult deny
                ? Task.FromResult(deny)
                : ExecuteQuery(query ?? new ListEmployerSupervisorsQuery(), cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Admin list workers")]
        [ProducesResponseType(typeof(PageableApiResponse<WorkerListItemModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> ListWorkers(
            [FromBody] ListWorkersQuery query,
            CancellationToken cancellationToken)
            => EnsureAdmin() is IActionResult deny
                ? Task.FromResult(deny)
                : ExecutePageQuery<WorkerListItemModel, PagedQueryResultModel<WorkerListItemModel>>(
                    query,
                    result => result.Items,
                    result => result.TotalCount,
                    result => result.Limit,
                    result => result.Offset,
                    cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Admin get worker detail")]
        [ProducesResponseType(typeof(ApiResponse<WorkerEmployerSafeFullDetailModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> GetWorkerDetail(
            [FromBody] GetWorkerDetailQuery query,
            CancellationToken cancellationToken)
            => EnsureAdmin() is IActionResult deny
                ? Task.FromResult(deny)
                : ExecuteQuery(query, cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Admin update worker profile")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> UpdateWorkerProfile(
            [FromBody] UpdateWorkerProfileCommand command,
            CancellationToken cancellationToken)
            => EnsureAdmin() is IActionResult deny
                ? Task.FromResult(deny)
                : ExecuteCommand(command, cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Admin delete worker")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> DeleteWorker(
            [FromBody] DeleteWorkerCommand command,
            CancellationToken cancellationToken)
            => EnsureAdmin() is IActionResult deny
                ? Task.FromResult(deny)
                : ExecuteCommand(command, cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Admin list system users")]
        [ProducesResponseType(typeof(PageableApiResponse<SystemUserListItemModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> ListSystemUsers(
            [FromBody] ListSystemUsersQuery query,
            CancellationToken cancellationToken)
            => EnsureAdmin() is IActionResult deny
                ? Task.FromResult(deny)
                : ExecutePageQuery<SystemUserListItemModel, PagedQueryResultModel<SystemUserListItemModel>>(
                    query,
                    result => result.Items,
                    result => result.TotalCount,
                    result => result.Limit,
                    result => result.Offset,
                    cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Admin get system user by id")]
        [ProducesResponseType(typeof(ApiResponse<SystemUserMeModel>), StatusCodes.Status200OK)]
        public Task<IActionResult> GetSystemUserById(
            [FromBody] GetSystemUserByIdQuery query,
            CancellationToken cancellationToken)
            => EnsureAdmin() is IActionResult deny
                ? Task.FromResult(deny)
                : ExecuteQuery(query, cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Admin ban system user")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> BanSystemUser(
            [FromBody] BanSystemUserCommand command,
            CancellationToken cancellationToken)
            => EnsureAdmin() is IActionResult deny
                ? Task.FromResult(deny)
                : ExecuteCommand(command, cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Admin suspend system user")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> SuspendSystemUser(
            [FromBody] SuspendSystemUserCommand command,
            CancellationToken cancellationToken)
            => EnsureAdmin() is IActionResult deny
                ? Task.FromResult(deny)
                : ExecuteCommand(command, cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Admin reactivate system user")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> ReactivateSystemUser(
            [FromBody] ReactivateSystemUserCommand command,
            CancellationToken cancellationToken)
            => EnsureAdmin() is IActionResult deny
                ? Task.FromResult(deny)
                : ExecuteCommand(command, cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Admin register employer")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> RegisterEmployer(
            [FromBody] RegisterEmployerCommand command,
            CancellationToken cancellationToken)
            => EnsureAdmin() is IActionResult deny
                ? Task.FromResult(deny)
                : ExecuteCommand(command, cancellationToken);

        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("Admin register admin user")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public Task<IActionResult> RegisterAdmin(
            [FromBody] RegisterAdminCommand command,
            CancellationToken cancellationToken)
            => EnsureAdmin() is IActionResult deny
                ? Task.FromResult(deny)
                : ExecuteCommand(command, cancellationToken);
    }
}
