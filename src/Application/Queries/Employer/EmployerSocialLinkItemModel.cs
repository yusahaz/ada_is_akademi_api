namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Employer profile slice carrying one outbound social/web URL per platform.
    /// </summary>
    public sealed record EmployerSocialLinkItemModel(SocialMediaPlatform Platform, string Url) :
        ModelBase;
}
