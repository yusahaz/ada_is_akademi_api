namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Worker language row.
    /// </summary>
    public sealed record WorkerLanguageDetailModel(
        int Id,
        string Language,
        LanguageLevel Level) :
        ModelBase;
}
