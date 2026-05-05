namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Worker self read model bileşeni: seçilen platform için tam URL (işverene kapalı yüzelerde kullanılmaz).
    /// </summary>
    public sealed record WorkerSocialLinkItemModel(SocialMediaPlatform Platform, string Url) :
        ModelBase;

}
