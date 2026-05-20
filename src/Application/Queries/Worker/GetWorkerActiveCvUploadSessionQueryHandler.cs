namespace Azoxia.AdaIsAkademi.Application
{
    using System.Linq;
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Identity;
    using Microsoft.Extensions.DependencyInjection;

    internal sealed class GetWorkerActiveCvUploadSessionQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<GetWorkerActiveCvUploadSessionQuery, WorkerActiveCvUploadSessionModel?>(serviceProvider)
    {
        private static readonly CvUploadSessionStatus[] OpenStatuses =
        [
            CvUploadSessionStatus.Uploaded,
            CvUploadSessionStatus.Extracting,
            CvUploadSessionStatus.AwaitingReview,
            CvUploadSessionStatus.Failed,
        ];

        protected override async Task<WorkerActiveCvUploadSessionModel?> HandleAsync(
            GetWorkerActiveCvUploadSessionQuery query,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            CvUploadSession? session = await UnitOfWork
                .GetRepository<CvUploadSession>()
                .Filter(x => x.WorkerId == workerId && OpenStatuses.Contains(x.Status))
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (session is null)
            {
                return null;
            }

            return new WorkerActiveCvUploadSessionModel(
                session.Id,
                session.Status,
                session.FileName,
                session.CreatedAt);
        }
    }
}
