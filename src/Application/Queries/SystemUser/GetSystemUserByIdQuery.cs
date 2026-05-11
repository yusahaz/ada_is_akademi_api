namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;

    /// <summary>
    /// Loads a system user detail by identifier for admin screens.
    /// </summary>
    public sealed class GetSystemUserByIdQuery :
        QueryBase<SystemUserMeModel>
    {
        public int SystemUserId { get; set; }
    }

    internal sealed class GetSystemUserByIdQueryValidator : IRequestValidator<GetSystemUserByIdQuery>
    {
        public ValidationResult Validate(GetSystemUserByIdQuery request)
        {
            List<ValidationFailure> failures = [];
            if (request.SystemUserId <= 0)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(GetSystemUserByIdQuery.SystemUserId)));
            }

            return new ValidationResult(failures);
        }
    }

    internal sealed class GetSystemUserByIdQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<GetSystemUserByIdQuery, SystemUserMeModel>(serviceProvider)
    {
        protected override async Task<SystemUserMeModel> HandleAsync(
            GetSystemUserByIdQuery query,
            CancellationToken cancellationToken)
        {
            SystemUser? user = await UnitOfWork
                .GetRepository<SystemUser>()
                .Filter(x => x.Id == query.SystemUserId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            user = user.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            return new SystemUserMeModel(
                user.Id,
                (int)user.Type,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Phone,
                user.AccountStatus,
                user.IsLocked,
                user.EmployerId,
                null);
        }
    }
}
