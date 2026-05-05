namespace Azoxia.AdaIsAkademi.SeedRunner.Generators;

/// <summary>
/// Hierarchical demo job categories (names prefixed with [Seed] for idempotent detection).
/// </summary>
internal static class JobCategoryCatalog
{
    #region Fields

    /// <summary>
    /// Logical sector keys reused by <see cref="SkillCatalog"/>.
    /// </summary>
    internal const string ClusterFood = "Food";

    internal const string ClusterLogistics = "Logistics";
    internal const string ClusterRetail = "Retail";
    internal const string ClusterEvent = "Event";
    internal const string ClusterCleaning = "Cleaning";
    internal const string ClusterOffice = "Office";

    #endregion Fields

    #region Utils

    /// <summary>
    /// Rows in creation order: parents first, then children (ParentKey set).
    /// </summary>
    internal static IReadOnlyList<CategoryRow> Rows { get; } =
    [
        new CategoryRow("P_FOOD", null, "[Seed] Yiyecek ve İçecek", "Yiyecek içecek sektörü demo kök kategori.", ClusterFood),
        new CategoryRow("P_LOG", null, "[Seed] Lojistik ve Dağıtım", "Lojistik demo kök kategori.", ClusterLogistics),
        new CategoryRow("P_RET", null, "[Seed] Perakende ve Satış", "Perakende demo kök kategori.", ClusterRetail),
        new CategoryRow("P_EVT", null, "[Seed] Etkinlik ve Sahne", "Etkinlik demo kök kategori.", ClusterEvent),
        new CategoryRow("P_CLN", null, "[Seed] Temizlik ve Hijyen", "Temizlik demo kök kategori.", ClusterCleaning),
        new CategoryRow("P_OFF", null, "[Seed] Ofis ve Müşteri Hizmetleri", "Ofis demo kök kategori.", ClusterOffice),

        new CategoryRow("C_GARSON", "P_FOOD", "[Seed] Garson / Servis", null, ClusterFood),
        new CategoryRow("C_BARISTA", "P_FOOD", "[Seed] Barista", null, ClusterFood),
        new CategoryRow("C_BULASIK", "P_FOOD", "[Seed] Bulaşıkhane", null, ClusterFood),
        new CategoryRow("C_ASCI", "P_FOOD", "[Seed] Mutfak Yardımcısı", null, ClusterFood),

        new CategoryRow("C_KURYE", "P_LOG", "[Seed] Motorlu Kurye", null, ClusterLogistics),
        new CategoryRow("C_SOFOR", "P_LOG", "[Seed] Şoför / Panelvan", null, ClusterLogistics),
        new CategoryRow("C_DEPO", "P_LOG", "[Seed] Depo Elemanı", null, ClusterLogistics),
        new CategoryRow("C_FORKLIFT", "P_LOG", "[Seed] Forklift Operatörü", null, ClusterLogistics),

        new CategoryRow("C_KASIYER", "P_RET", "[Seed] Kasiyer", null, ClusterRetail),
        new CategoryRow("C_REYON", "P_RET", "[Seed] Reyon Görevlisi", null, ClusterRetail),
        new CategoryRow("C_HOSTES_MAG", "P_RET", "[Seed] Mağaza Hostesi", null, ClusterRetail),
        new CategoryRow("C_STOK", "P_RET", "[Seed] Stok / Sayım", null, ClusterRetail),

        new CategoryRow("C_PROMO", "P_EVT", "[Seed] Promotör", null, ClusterEvent),
        new CategoryRow("C_STAND", "P_EVT", "[Seed] Stand Hostesi", null, ClusterEvent),
        new CategoryRow("C_ANKET", "P_EVT", "[Seed] Anketör", null, ClusterEvent),
        new CategoryRow("C_SAHNE", "P_EVT", "[Seed] Sahne Yardımcısı", null, ClusterEvent),

        new CategoryRow("C_GENEL_TEM", "P_CLN", "[Seed] Genel Temizlik", null, ClusterCleaning),
        new CategoryRow("C_END_TEM", "P_CLN", "[Seed] Endüstriyel Temizlik", null, ClusterCleaning),
        new CategoryRow("C_CAM", "P_CLN", "[Seed] Cam Temizliği", null, ClusterCleaning),
        new CategoryRow("C_ILACL", "P_CLN", "[Seed] İlaçlama Yardımcısı", null, ClusterCleaning),

        new CategoryRow("C_VERI", "P_OFF", "[Seed] Veri Girişi", null, ClusterOffice),
        new CategoryRow("C_MTHS", "P_OFF", "[Seed] Müşteri Temsilcisi", null, ClusterOffice),
        new CategoryRow("C_SEKRETER", "P_OFF", "[Seed] Sekreterya", null, ClusterOffice),
        new CategoryRow("C_ARSIV", "P_OFF", "[Seed] Arşiv Elemanı", null, ClusterOffice),
    ];

    #endregion Utils

    #region Nested types

    /// <summary>
    /// One category row from the static catalog.
    /// </summary>
    /// <param name="Key">Stable catalog key.</param>
    /// <param name="ParentKey">Parent catalog key, or null for roots.</param>
    /// <param name="Name">Display name stored in <c>JobCategory.Name</c>.</param>
    /// <param name="Description">Optional description for roots.</param>
    /// <param name="Cluster">Sector cluster for skill matching.</param>
    internal sealed record CategoryRow(
        string Key,
        string? ParentKey,
        string Name,
        string? Description,
        string Cluster);

    #endregion Nested types
}
