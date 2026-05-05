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
    /// Sends a worker notification through push with fallback channels.
    /// </summary>
    public class SendWorkerNotificationCommand :
        CommandBase<int>
    {
        #region Properties

        public string Body { get; set; }
        public int? JobPostingId { get; set; }
        public string TemplateCode { get; set; }
        public string Title { get; set; }
        public int WorkerId { get; set; }

        #endregion Properties
    }

    internal class SendWorkerNotificationCommandValidator : IRequestValidator<SendWorkerNotificationCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(SendWorkerNotificationCommand request)
        {
            List<ValidationFailure> failures = [];
            if (request.WorkerId <= 0)
            {
                failures.Add(ApplicationValidationCodes.SendWorkerNotificationWorkerId.ForField(nameof(SendWorkerNotificationCommand.WorkerId)));
            }

            if (request.TemplateCode.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.SendWorkerNotificationTemplateCode.ForField(nameof(SendWorkerNotificationCommand.TemplateCode)));
            }

            if (request.Title.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.SendWorkerNotificationTitle.ForField(nameof(SendWorkerNotificationCommand.Title)));
            }

            if (request.Body.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.SendWorkerNotificationBody.ForField(nameof(SendWorkerNotificationCommand.Body)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class SendWorkerNotificationCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<SendWorkerNotificationCommand, int>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<int> HandleAsync(SendWorkerNotificationCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            _ = executionContext.RequireAdaIsEmployerActorId();

            Worker? worker = await UnitOfWork
                .GetRepository<Worker>()
                .Filter(x => x.Id == command.WorkerId)
                .FirstOrDefaultAsync(cancellationToken);
            worker = worker.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            SystemUser? systemUser = await UnitOfWork
                .GetRepository<SystemUser>()
                .Filter(x => x.Id == worker.SystemUserId)
                .Include(x => x.Devices)
                .FirstOrDefaultAsync(cancellationToken);
            systemUser = systemUser.ThrowIfNull(AzoxiaErrorCodes.NotFound);

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
            bool hasVerifiedEmail = systemUser.EmailVerifiedAt.HasValue
                && !systemUser.Email.IsNullOrWhiteSpace();

            SystemUserNotificationDispatch dispatch = new(
                worker.SystemUserId,
                NotificationChannel.Push,
                command.TemplateCode,
                command.Title,
                command.Body,
                worker.Id,
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

            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.WorkerDependency(worker.Id), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.SystemUserNotificationDispatchWorkerDependency(worker.Id), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.SystemUserNotificationDispatchAllDependency(), cancellationToken);

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
