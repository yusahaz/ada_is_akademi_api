namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Identity;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Returns the authenticated user's profile summary by resolving identity from bearer claims.
    /// </summary>
    public class GetSystemUserMeQuery :
        QueryBase<SystemUserMeModel>;

    internal class GetSystemUserMeQueryValidator : IRequestValidator<GetSystemUserMeQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(GetSystemUserMeQuery request)
            => new();

        #endregion Methods
    }

    internal class GetSystemUserMeQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<GetSystemUserMeQuery, SystemUserMeModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<SystemUserMeModel> HandleAsync(GetSystemUserMeQuery query, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();

            if (!int.TryParse(executionContext.GetClaim("system_user_id"), out int systemUserId) || systemUserId <= 0)
            {
                ApplicationValidationCodes.ActorSystemUserIdClaimRequired.Throw();
            }

            SystemUser? user = await UnitOfWork
                .GetRepository<SystemUser>()
                .Filter(x => x.Id == systemUserId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            user = user.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            int? workerId = int.TryParse(executionContext.GetClaim("worker_id"), out int parsedWorkerId) && parsedWorkerId > 0
                ? parsedWorkerId
                : null;
            int? employerId = int.TryParse(executionContext.GetClaim("employer_id"), out int parsedEmployerId) && parsedEmployerId > 0
                ? parsedEmployerId
                : null;

            return new SystemUserMeModel(
                user.Id,
                (int)user.Type,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Phone,
                user.AccountStatus,
                user.IsLocked,
                employerId,
                workerId);
        }

        #endregion Utils
    }
}
