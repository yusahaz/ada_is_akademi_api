namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.ValueTypes;
    using System;

    /// <summary>
    /// Loads a single employer read model by identifier.
    /// </summary>
    public class GetEmployerByIdQuery :
        QueryBase<EmployerDetailModel>
    {
        #region Properties

        /// <summary>
        /// Employer primary key.
        /// </summary>
        public int EmployerId { get; set; }

        #endregion Properties
    }

    internal class GetEmployerByIdQueryValidator : IRequestValidator<GetEmployerByIdQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(GetEmployerByIdQuery request)
        {
            List<ValidationFailure> failures = [];

            if (request.EmployerId <= 0)
            {
                failures.Add(ApplicationValidationCodes.GetEmployerByIdEmployerId.ForField(nameof(request.EmployerId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class GetEmployerByIdQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<GetEmployerByIdQuery, EmployerDetailModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<EmployerDetailModel> HandleAsync(GetEmployerByIdQuery query, CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.EmployerDetailKey(query.EmployerId);
            EmployerDetailModel? cached = await CacheService.GetAsync<EmployerDetailModel>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            Employer? entity = await UnitOfWork
                .GetRepository<Employer>()
                .Filter(x => x.Id == query.EmployerId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            EmployerContactModel? contact = MapContact(entity);

            EmployerDetailModel model = new(
                entity.Id,
                entity.Name,
                entity.Description,
                entity.Status,
                entity.TaxNumber.Value,
                contact,
                entity.LogoObjectKey);

            await CacheService.SetAsync(
                cacheKey,
                model,
                AdaIsCacheKeys.DetailReadModelOptions(AdaIsCacheKeys.EmployerDependency(entity.Id)),
                cancellationToken);

            return model;
        }

        private static EmployerContactModel? MapContact(Employer entity)
        {
            Contact c = entity.Contact;
            if (string.IsNullOrWhiteSpace(c.Email) && string.IsNullOrWhiteSpace(c.Phone))
            {
                return null;
            }

            return new EmployerContactModel(
                c.FirstName,
                c.LastName,
                c.Email,
                c.Phone);
        }

        #endregion Utils
    }
}
