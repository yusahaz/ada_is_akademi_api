namespace Azoxia.AdaIsAkademi.Application.Services
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;

    /// <summary>
    /// Deterministik ağırlık tablosu (toplam 100): yetenek 15 · müsaitlik 15 · portfolyo dilimi (sertifika/eğitim/deneyim/dil) 18 ·
    /// uyruk veya üniversite 8 · maaş alt+üst sınır 13 · ilgi kategorisi 13 · bio 6 · profil foto object key 5 · sosyal link≥1 için 7.
    /// Görev dökümü (<c>worker-employer-profile-enrichment.md</c>) ile senkron tutulmalıdır.
    /// </summary>
    internal sealed class WorkerProfileCompletionEvaluator :
        IWorkerProfileCompletionEvaluator
    {
        #region Fields

        private const int WeightAvailability = 15;
        private const int WeightBio = 6;
        private const int WeightInterestedCategories = 13;
        private const int WeightNationalityOrUniversity = 8;
        private const int WeightPhoto = 5;
        private const int WeightPortfolioSegment = 18;
        private const int WeightSalaryBothBounds = 13;
        private const int WeightSkills = 15;
        private const int WeightSocialLinks = 7;

        #endregion Fields

        #region Methods

        /// <inheritdoc />
        public int CompletionPercentOf(Worker worker)
        {
            worker = worker.ThrowIfNull(AzoxiaErrorCodes.ArgumentNull);

            int earned = 0;

            if (worker.Skills.Count > 0)
            {
                earned += WeightSkills;
            }

            if (worker.Availabilities.Count > 0)
            {
                earned += WeightAvailability;
            }

            bool portfolioSegment =
                worker.Certificates.Count > 0 ||
                worker.Educations.Count > 0 ||
                worker.Experiences.Count > 0 ||
                worker.Languages.Count > 0;

            if (portfolioSegment)
            {
                earned += WeightPortfolioSegment;
            }

            bool hasNationalityOrUniversity = !(worker.Nationality.IsNullOrWhiteSpace())
                || !(worker.University.IsNullOrWhiteSpace());

            if (hasNationalityOrUniversity)
            {
                earned += WeightNationalityOrUniversity;
            }

            bool salaryComplete = worker.ExpectedSalaryMinAmount.HasValue
                && !(worker.ExpectedSalaryMinCurrency.IsNullOrWhiteSpace())
                && worker.ExpectedSalaryMaxAmount.HasValue
                && !(worker.ExpectedSalaryMaxCurrency.IsNullOrWhiteSpace());

            if (salaryComplete)
            {
                earned += WeightSalaryBothBounds;
            }

            if (worker.InterestedJobCategories.Count > 0)
            {
                earned += WeightInterestedCategories;
            }

            if (!(worker.Bio.IsNullOrWhiteSpace()))
            {
                earned += WeightBio;
            }

            if (!(worker.ProfilePhotoObjectKey.IsNullOrWhiteSpace()))
            {
                earned += WeightPhoto;
            }

            if (worker.SocialLinks.Count > 0)
            {
                earned += WeightSocialLinks;
            }

            return Math.Clamp(earned, 0, 100);
        }

        #endregion Methods
    }
}
