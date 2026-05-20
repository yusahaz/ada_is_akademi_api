namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Identity;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Marks one notification dispatch row as read for the authenticated system user.
    /// </summary>
    public class MarkNotificationAsReadCommand :
        CommandBase
    {
        #region Properties

        public int NotificationId { get; set; }

        #endregion Properties
    }

    internal class MarkNotificationAsReadCommandValidator : IRequestValidator<MarkNotificationAsReadCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(MarkNotificationAsReadCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.NotificationId <= 0)
            {
                failures.Add(ApplicationValidationCodes.MarkNotificationAsReadNotificationId.ForField(nameof(MarkNotificationAsReadCommand.NotificationId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class MarkNotificationAsReadCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<MarkNotificationAsReadCommand>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(MarkNotificationAsReadCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            if (!int.TryParse(executionContext.GetClaim("system_user_id"), out int actorSystemUserId) || actorSystemUserId <= 0)
            {
                ApplicationValidationCodes.ActorSystemUserIdClaimRequired.Throw();
            }

            SystemUserNotificationDispatch? notification = await UnitOfWork
                .GetRepository<SystemUserNotificationDispatch>()
                .Filter(x => x.Id == command.NotificationId)
                .FirstOrDefaultAsync(cancellationToken);
            notification = notification.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            (notification.SystemUserId == actorSystemUserId)
                .ThrowIfFalse(ApplicationValidationCodes.ActorResourceAccessDenied);

            notification.MarkAsRead();
            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await AdaIsReadModelCacheInvalidation.InvalidateSystemUserNotificationScopesAsync(
                CacheService,
                actorSystemUserId,
                workerId: null,
                cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
