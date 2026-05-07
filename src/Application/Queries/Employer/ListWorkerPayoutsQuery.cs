namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Identity;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Lists worker payouts for authenticated employer.
    /// </summary>
    public class ListWorkerPayoutsQuery :
        QueryBase<PagedQueryResultModel<WorkerPayoutListItemModel>>
    {
        #region Properties

        public int Limit { get; set; } = 20;
        public int Offset { get; set; }

        #endregion Properties
    }

    internal class ListWorkerPayoutsQueryValidator : IRequestValidator<ListWorkerPayoutsQuery>
    {
        /// <inheritdoc />
        public ValidationResult Validate(ListWorkerPayoutsQuery request)
        {
            List<ValidationFailure> failures = [];
            if (request.Limit is < 1 or > 200)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(ListWorkerPayoutsQuery.Limit)));
            }

            if (request.Offset < 0)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(ListWorkerPayoutsQuery.Offset)));
            }

            return new ValidationResult(failures);
        }
    }

    internal class ListWorkerPayoutsQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListWorkerPayoutsQuery, PagedQueryResultModel<WorkerPayoutListItemModel>>(serviceProvider)
    {
        /// <inheritdoc />
        protected override async Task<PagedQueryResultModel<WorkerPayoutListItemModel>> HandleAsync(
            ListWorkerPayoutsQuery query,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            CacheKey cacheKey = new("query", "EmployerWorkerPayouts", $"{employerId}:{query.Limit}:{query.Offset}");
            PagedQueryResultModel<WorkerPayoutListItemModel>? cached =
                await CacheService.GetAsync<PagedQueryResultModel<WorkerPayoutListItemModel>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            var filter = UnitOfWork
                .GetRepository<WorkerPayout>()
                .Filter(x => x.EmployerId == employerId)
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.Worker)
                .Include(x => x.Worker.SystemUser);

            int totalCount = checked((int)await filter.CountAsync(cancellationToken));

            List<WorkerPayoutListItemModel> rows = (await filter
                .OrderByDescending(x => x.CreatedAt)
                .Skip(query.Offset)
                .Take(query.Limit)
                .ToListAsync(
                    x => new WorkerPayoutListItemModel(
                        x.Id,
                        x.AssignmentId,
                        x.WorkerId,
                        $"{x.Worker.SystemUser.FirstName ?? string.Empty} {x.Worker.SystemUser.LastName ?? string.Empty}".Trim(),
                        x.NetAmount.Amount,
                        x.NetAmount.Currency,
                        x.Status,
                        x.Status == WorkerPayoutStatus.Processing,
                        x.LastFailureReason,
                        x.Status == WorkerPayoutStatus.Processing ? "system" : null,
                        x.ConfirmationDueAt,
                        x.CreatedAt,
                        x.PaidAt ?? x.FailedAt ?? x.ProcessingMarkedAt ?? x.CreatedAt),
                    cancellationToken))
                .ToList();

            PagedQueryResultModel<WorkerPayoutListItemModel> result = new(rows, totalCount, query.Limit, query.Offset);
            await CacheService.SetAsync(
                cacheKey,
                result,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.WorkerPayoutEmployerDependency(employerId),
                    AdaIsCacheKeys.WorkerPayoutAllDependency()),
                cancellationToken);

            return result;
        }
    }
}
