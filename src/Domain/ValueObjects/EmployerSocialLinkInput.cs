namespace Azoxia.AdaIsAkademi.Domain
{
    /// <summary>
    /// Incoming social link tuple used when replacing employer social links inside the aggregate.
    /// </summary>
    /// <param name="Platform">Logical platform grouping.</param>
    /// <param name="Url">Absolute HTTPS URL trimmed by the caller.</param>
    public sealed record EmployerSocialLinkInput(SocialMediaPlatform Platform, string Url);
}
