namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Identity;
    using Microsoft.Extensions.DependencyInjection;

    internal abstract class WorkerCollectionCommandHandlerBase<TCommand>(IServiceProvider serviceProvider)
        : CommandHandlerBase<TCommand, int>(serviceProvider)
        where TCommand : ICommand<int>
    {
        protected async Task<(int workerId, Worker worker)> GetActorWorkerAsync(CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();
            Worker? worker = await UnitOfWork.GetRepository<Worker>().GetByIdAsync(workerId, cancellationToken);
            worker = worker.ThrowIfNull(AzoxiaErrorCodes.NotFound);
            return (workerId, worker);
        }

        protected async Task InvalidateWorkerAsync(int workerId, CancellationToken cancellationToken)
        {
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.WorkerDependency(workerId), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.WorkerAllDependency(), cancellationToken);
        }
    }

    internal abstract class WorkerCollectionUnitCommandHandlerBase<TCommand>(IServiceProvider serviceProvider)
        : CommandHandlerBase<TCommand>(serviceProvider)
        where TCommand : ICommand
    {
        protected async Task<(int workerId, Worker worker)> GetActorWorkerAsync(CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();
            Worker? worker = await UnitOfWork.GetRepository<Worker>().GetByIdAsync(workerId, cancellationToken);
            worker = worker.ThrowIfNull(AzoxiaErrorCodes.NotFound);
            return (workerId, worker);
        }

        protected async Task InvalidateWorkerAsync(int workerId, CancellationToken cancellationToken)
        {
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.WorkerDependency(workerId), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.WorkerAllDependency(), cancellationToken);
        }
    }
}
