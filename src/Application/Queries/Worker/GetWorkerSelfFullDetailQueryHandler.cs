namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Azoxia.AdaIsAkademi.Application.Services;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Identity;
    using Azoxia.Core.ValueTypes;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    internal class GetWorkerSelfFullDetailQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<GetWorkerSelfFullDetailQuery, WorkerSelfFullDetailModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<WorkerSelfFullDetailModel> HandleAsync(
            GetWorkerSelfFullDetailQuery query,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            CacheKey cacheKey = AdaIsCacheKeys.WorkerSelfFullDetailKey(workerId);
            WorkerSelfFullDetailModel? cached =
                await CacheService.GetAsync<WorkerSelfFullDetailModel>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            Worker? entity = await UnitOfWork
                .GetRepository<Worker>()
                .Filter(x => x.Id == workerId)
                .AsNoTracking()
                .Include(x => x.SystemUser)
                .Include(x => x.Skills)
                .Include(x => x.Availabilities)
                .Include(x => x.Certificates)
                .Include(x => x.Educations)
                .Include(x => x.Experiences)
                .Include(x => x.Languages)
                .Include(x => x.References)
                .Include(x => x.SocialLinks)
                .Include(x => x.InterestedJobCategories)
                .FirstOrDefaultAsync(cancellationToken);

            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            var categoryItemsReader = new WorkerInterestedJobCategoryItemsReader(UnitOfWork);
            IReadOnlyList<WorkerInterestedJobCategoryItemModel> categories =
                await categoryItemsReader.ListForWorkerAsync(entity, cancellationToken);

            IWorkerProfileCompletionEvaluator completionEvaluator =
                ServiceProvider.GetRequiredService<IWorkerProfileCompletionEvaluator>();
            int profileCompletionPercent = completionEvaluator.CompletionPercentOf(entity);

            IReadOnlyList<WorkerSocialLinkItemModel> socialLinks = entity.SocialLinks
                .OrderBy(x => x.Platform)
                .ThenBy(x => x.Url, StringComparer.OrdinalIgnoreCase)
                .Select(x => new WorkerSocialLinkItemModel(x.Platform, x.Url))
                .ToList();

            WorkerSelfFullDetailModel model = new(
                entity.Id,
                entity.SystemUserId,
                entity.Nationality,
                entity.University,
                entity.Bio,
                entity.ProfilePhotoObjectKey,
                entity.EmbeddingUpdatedAt,
                new WorkerSystemUserSummaryModel(
                    entity.SystemUser.Id,
                    entity.SystemUser.Email,
                    entity.SystemUser.FirstName,
                    entity.SystemUser.LastName,
                    entity.SystemUser.Phone,
                    entity.SystemUser.AccountStatus),
                entity.Skills
                    .OrderBy(x => x.Tag.Value, StringComparer.OrdinalIgnoreCase)
                    .Select(x => new WorkerSkillDetailModel(x.Id, x.Tag.Value, x.CreatedAt))
                    .ToList(),
                entity.Availabilities
                    .OrderBy(x => x.DayOfWeek)
                    .ThenBy(x => x.TimeFrom)
                    .Select(x => new WorkerAvailabilityDetailModel(x.Id, x.DayOfWeek, x.TimeFrom, x.TimeTo))
                    .ToList(),
                entity.Certificates
                    .OrderByDescending(x => x.IssuedAt)
                    .ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
                    .Select(x => new WorkerCertificateDetailModel(
                        x.Id,
                        x.Name,
                        x.IssuingOrganization,
                        x.IssuedAt,
                        x.ExpiresAt,
                        x.DocumentUrl,
                        x.CreatedAt))
                    .ToList(),
                entity.Educations
                    .OrderByDescending(x => x.StartYear)
                    .ThenBy(x => x.School, StringComparer.OrdinalIgnoreCase)
                    .Select(x => new WorkerEducationDetailModel(
                        x.Id,
                        x.School,
                        x.Department,
                        x.EducationType,
                        x.StartYear,
                        x.EndYear,
                        x.IsOngoing))
                    .ToList(),
                entity.Experiences
                    .OrderByDescending(x => x.StartDate)
                    .ThenBy(x => x.CompanyName, StringComparer.OrdinalIgnoreCase)
                    .Select(x => new WorkerExperienceDetailModel(
                        x.Id,
                        x.CompanyName,
                        x.Position,
                        x.StartDate,
                        x.EndDate,
                        x.IsCurrent,
                        x.Description))
                    .ToList(),
                entity.Languages
                    .OrderBy(x => x.Language, StringComparer.OrdinalIgnoreCase)
                    .Select(x => new WorkerLanguageDetailModel(x.Id, x.Language, x.Level))
                    .ToList(),
                entity.References
                    .OrderBy(x => x.Company, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(x => x.Position, StringComparer.OrdinalIgnoreCase)
                    .Select(x => new WorkerReferenceDetailModel(
                        x.Id,
                        x.Company,
                        x.Position,
                        x.Contact.FirstName,
                        x.Contact.LastName,
                        x.Contact.Email,
                        x.Contact.Phone))
                    .ToList(),
                socialLinks,
                MapWorkerExpectedSalary(entity.ExpectedSalaryMinAmount, entity.ExpectedSalaryMinCurrency),
                MapWorkerExpectedSalary(entity.ExpectedSalaryMaxAmount, entity.ExpectedSalaryMaxCurrency),
                profileCompletionPercent,
                categories);

            await CacheService.SetAsync(
                cacheKey,
                model,
                AdaIsCacheKeys.DetailReadModelOptions(AdaIsCacheKeys.WorkerDependency(entity.Id)),
                cancellationToken);

            return model;
        }

        /// <summary>
        /// Builds <see cref="Money"/> from persisted salary columns when both amount and currency exist (EF cannot materialize nullable composites).
        /// </summary>
        private Money? MapWorkerExpectedSalary(decimal? amount, string? currency)
        {
            if (!amount.HasValue || currency.IsNullOrWhiteSpace())
            {
                return null;
            }

            return new Money(amount.Value, currency.Trim().ToUpperInvariant());
        }

        #endregion Utils
    }
}
