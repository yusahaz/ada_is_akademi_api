namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using System.Linq;

    /// <summary>
    /// Soft deletes employer and all employer-scoped users.
    /// </summary>
    public class DeleteEmployerCommand : CommandBase
    {
        /// <summary>
        /// Identifier of the employer to delete.
        /// </summary>
        public int EmployerId { get; set; }
    }

    internal class DeleteEmployerCommandValidator : IRequestValidator<DeleteEmployerCommand>
    {
        /// <inheritdoc />
        public ValidationResult Validate(DeleteEmployerCommand request)
        {
            List<ValidationFailure> failures = [];
            if (request.EmployerId <= 0)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.EmployerId)));
            }

            return new ValidationResult(failures);
        }
    }

    internal class DeleteEmployerCommandHandler(IServiceProvider serviceProvider)
        : CommandHandlerBase<DeleteEmployerCommand>(serviceProvider)
    {
        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(DeleteEmployerCommand command, CancellationToken cancellationToken)
        {
            Employer? employer = await UnitOfWork.GetRepository<Employer>().GetByIdAsync(command.EmployerId, cancellationToken);
            employer = employer.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            IEnumerable<int> locationIdsRaw = await UnitOfWork.GetRepository<EmployerLocation>()
                .Filter(x => x.EmployerId == employer.Id)
                .AsNoTracking()
                .ToListAsync(x => x.Id, cancellationToken);
            List<int> locationIds = locationIdsRaw.ToList();

            IEnumerable<int> supervisorUserIdsRaw = await UnitOfWork.GetRepository<ShiftSupervisor>()
                .Filter(x => x.EmployerId == employer.Id)
                .AsNoTracking()
                .ToListAsync(x => x.SystemUserId, cancellationToken);
            List<int> supervisorUserIds = supervisorUserIdsRaw.ToList();

            IEnumerable<int> employerScopedMembershipUserIdsRaw = await UnitOfWork.GetRepository<SystemUserGroupMembership>()
                .Filter(x => x.ScopeType == MembershipScopeType.EmployerScoped && x.ScopeId == employer.Id)
                .AsNoTracking()
                .ToListAsync(x => x.SystemUserId, cancellationToken);
            List<int> employerScopedMembershipUserIds = employerScopedMembershipUserIdsRaw.ToList();

            IEnumerable<int> locationScopedMembershipUserIdsRaw = locationIds.Count == 0
                ? []
                : await UnitOfWork.GetRepository<SystemUserGroupMembership>()
                    .Filter(x =>
                        x.ScopeType == MembershipScopeType.LocationScoped
                        && x.ScopeId.HasValue
                        && locationIds.Contains(x.ScopeId.Value))
                    .AsNoTracking()
                    .ToListAsync(x => x.SystemUserId, cancellationToken);
            List<int> locationScopedMembershipUserIds = locationScopedMembershipUserIdsRaw.ToList();

            IReadOnlyList<int> systemUserIds = supervisorUserIds
                .Concat(employerScopedMembershipUserIds)
                .Concat(locationScopedMembershipUserIds)
                .Distinct()
                .ToList();

            if (systemUserIds.Count > 0)
            {
                IEnumerable<SystemUser> systemUsersRaw = await UnitOfWork.GetRepository<SystemUser>()
                    .Filter(x => systemUserIds.Contains(x.Id))
                    .ToListAsync(cancellationToken);
                List<SystemUser> systemUsers = systemUsersRaw.ToList();

                foreach (SystemUser systemUser in systemUsers)
                {
                    systemUser.DeleteSystemUser();
                }
            }

            employer.DeleteEmployer();

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.EmployerDependency(employer.Id), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.EmployerAllDependency(), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.SystemUserAllDependency(), cancellationToken);

            return Unit.Value;
        }
    }
}
