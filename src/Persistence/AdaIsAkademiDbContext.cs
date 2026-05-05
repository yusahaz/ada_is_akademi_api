namespace Azoxia.AdaIsAkademi.Persistence
{
    using Azoxia.Core.Persistence;

    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Entity Framework Core database context for Ada Is Akademi persistence.
    /// </summary>
    internal class AdaIsAkademiDbContext(DbContextOptions<AdaIsAkademiDbContext> options) :
        DbContextBase<AdaIsAkademiDbContext>(options);
}
