namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Identity;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Marks all unread notification rows as read for the authenticated system user.
    /// </summary>
    public class MarkAllNotificationsAsReadCommand :
        CommandBase;

    internal class MarkAllNotificationsAsReadCommandValidator : IRequestValidator<MarkAllNotificationsAsReadCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(MarkAllNotificationsAsReadCommand request)
            => new();

        #endregion Methods
    }

    internal class MarkAllNotificationsAsReadCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<MarkAllNotificationsAsReadCommand>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(MarkAllNotificationsAsReadCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            if (!int.TryParse(executionContext.GetClaim("system_user_id"), out int actorSystemUserId) || actorSystemUserId <= 0)
            {
                ApplicationValidationCodes.ActorSystemUserIdClaimRequired.Throw();
            }

            List<SystemUserNotificationDispatch> unreadRows = (await UnitOfWork
                    .GetRepository<SystemUserNotificationDispatch>()
                    .Filter(x => x.SystemUserId == actorSystemUserId && !x.IsRead)
                    .ToListAsync(cancellationToken))
                .ToList();

            foreach (SystemUserNotificationDispatch row in unreadRows)
            {
                row.MarkAsRead();
            }

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
