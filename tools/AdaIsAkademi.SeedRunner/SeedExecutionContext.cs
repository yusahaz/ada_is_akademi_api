namespace Azoxia.AdaIsAkademi.SeedRunner;

using Azoxia.Core.Identity;

/// <summary>
/// Execution context used by SeedRunner background process.
/// </summary>
internal sealed class SeedExecutionContext : IExecutionContext
{
    #region Methods

    /// <inheritdoc />
    public string? GetClaim(string claimType) => null;

    /// <inheritdoc />
    public IReadOnlyList<string> GetClaims(string claimType) => [];

    #endregion Methods

    #region Properties

    /// <inheritdoc />
    public bool IsAuthenticated => false;

    #endregion Properties
}
