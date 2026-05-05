namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Identity;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Sends notification to any system user (worker, employer, admin) with channel fallback.
    /// </summary>
    public class SendSystemUserNotificationCommand :
        CommandBase<int>
    {
        #region Properties

        public string Body { get; set; }
        public int? JobPostingId { get; set; }
        public int SystemUserId { get; set; }
        public string TemplateCode { get; set; }
        public string Title { get; set; }

        #endregion Properties
    }

    internal class SendSystemUserNotificationCommandValidator : IRequestValidator<SendSystemUserNotificationCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(SendSystemUserNotificationCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.SystemUserId <= 0)
            {
                failures.Add(ApplicationValidationCodes.SendSystemUserNotificationSystemUserId.ForField(nameof(SendSystemUserNotificationCommand.SystemUserId)));
            }

            if (request.TemplateCode.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.SendWorkerNotificationTemplateCode.ForField(nameof(SendSystemUserNotificationCommand.TemplateCode)));
            }

            if (request.Title.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.SendWorkerNotificationTitle.ForField(nameof(SendSystemUserNotificationCommand.Title)));
            }

            if (request.Body.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.SendWorkerNotificationBody.ForField(nameof(SendSystemUserNotificationCommand.Body)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class SendSystemUserNotificationCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<SendSystemUserNotificationCommand, int>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<int> HandleAsync(SendSystemUserNotificationCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            _ = executionContext.GetClaim("system_user_id")
                .ThrowIfNullOrWhiteSpace(ApplicationValidationCodes.ActorSystemUserIdClaimRequired);

            SystemUser? systemUser = await UnitOfWork
                .GetRepository<SystemUser>()
                .Filter(x => x.Id == command.SystemUserId)
                .Include(x => x.Devices)
                .FirstOrDefaultAsync(cancellationToken);
            systemUser = systemUser.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            Worker? worker = await UnitOfWork
                .GetRepository<Worker>()
                .Filter(x => x.SystemUserId == command.SystemUserId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (command.JobPostingId.HasValue)
            {
                JobPosting? posting = await UnitOfWork
                    .GetRepository<JobPosting>()
                    .Filter(x => x.Id == command.JobPostingId.Value)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);
                posting.ThrowIfNull(AzoxiaErrorCodes.NotFound);
            }

            List<string> tokens = systemUser.Devices
                .Where(x => !x.DeviceToken.IsNullOrWhiteSpace())
                .Select(x => x.DeviceToken!)
                .Distinct()
                .ToList();
            bool hasVerifiedEmail = systemUser.EmailVerifiedAt.HasValue && !systemUser.Email.IsNullOrWhiteSpace();

            SystemUserNotificationDispatch dispatch = new(
                systemUser.Id,
                NotificationChannel.Push,
                command.TemplateCode,
                command.Title,
                command.Body,
                worker?.Id,
                command.JobPostingId);
            UnitOfWork.Add(dispatch);
            await UnitOfWork.SaveChangesAsync(cancellationToken);

            IPushNotificationSender sender = ServiceProvider.GetService<IPushNotificationSender>()
                ?? new NoopPushNotificationSender();

            if (tokens.Count > 0)
            {
                PushNotificationSendResult pushResult = await sender.SendAsync(tokens, command.Title, command.Body, cancellationToken);
                if (pushResult.IsSuccess)
                {
                    dispatch.MarkAsSent(NotificationChannel.Push);
                }
                else
                {
                    dispatch.MarkAsFailed(pushResult.ErrorMessage ?? pushResult.ErrorCode);
                    if (hasVerifiedEmail)
                    {
                        dispatch.MarkAsSent(NotificationChannel.Email, "push_send_failed");
                    }
                    else
                    {
                        dispatch.MarkAsSent(NotificationChannel.InApp, "push_send_failed_and_unverified_email");
                    }
                }
            }
            else if (hasVerifiedEmail)
            {
                dispatch.MarkAsSent(NotificationChannel.Email, "missing_push_token");
            }
            else
            {
                dispatch.MarkAsSent(NotificationChannel.InApp, "missing_push_token_and_unverified_email");
            }

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.SystemUserAllDependency(), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.SystemUserNotificationDispatchDependency(systemUser.Id), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.SystemUserNotificationDispatchAllDependency(), cancellationToken);
            if (worker is not null)
            {
                await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.WorkerDependency(worker.Id), cancellationToken);
            }

            return dispatch.Id;
        }

        private sealed class NoopPushNotificationSender : IPushNotificationSender
        {
            public Task<PushNotificationSendResult> SendAsync(
                IReadOnlyList<string> deviceTokens,
                string title,
                string body,
                CancellationToken cancellationToken)
                => Task.FromResult(new PushNotificationSendResult(IsSuccess: true));
        }

        #endregion Utils
    }
}
