namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;
    using System;

    /// <summary>
    /// Authentication token pair returned after login/refresh flows.
    /// </summary>
    public sealed record SystemUserTokenModel(
        int SystemUserId,
        int SystemUserType,
        string AccessToken,
        DateTime AccessTokenExpiresAt,
        string RefreshToken,
        DateTime RefreshTokenExpiresAt) :
        ModelBase;
}
