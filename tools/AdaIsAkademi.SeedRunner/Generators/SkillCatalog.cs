namespace Azoxia.AdaIsAkademi.SeedRunner.Generators;

/// <summary>
/// Skill tags grouped by sector cluster for weighted worker/job matching (SkillTag value shape).
/// </summary>
internal static class SkillCatalog
{
    #region Fields

    private static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> TagsByCluster =
        new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase)
        {
            [JobCategoryCatalog.ClusterFood] =
            [
                "garson", "servis", "siparis_alma", "kahve", "espresso", "latte_art", "filtre_kahve",
                "barista", "kokteyl_bilgisi", "menu_bilgisi", "hygiene_haccp",                 "bulasik_makinesi",
                "mutfak_duzeni", "kahvalti_servisi", "sicak_icecek", "soguk_icecek",
                "pastane", "izgara_yardimcisi", "sef_yardimcisi", "temel_mutfak",
            ],
            [JobCategoryCatalog.ClusterLogistics] =
            [
                "motorlu_kurye", "e_scooter", "paket_teslim", "adres_okuma", "navigasyon",
                "google_maps", "rota_planlama", "panelvan", "b_ehliyet", "forklift",
                "transpalet", "depo_raflama", "sayim", "irsaliye_okuma", "siparis_toplama",
                "barkod_okuyucu", "kalite_kontrol", "teslimat_notu", "tahsilat_pos", "trafik_kurallari",
            ],
            [JobCategoryCatalog.ClusterRetail] =
            [
                "kasiyer", "pos_kullanimi", "para_ustu", "iade_islemi", "kampanya_okuma",
                "reyon_duzeni", "stok_cekme", "fiyat_etiketi", "raf_yukleme", "sku_kontrol",
                "envanter_destek", "soguk_dolap_duzeni", "tekstil_kat_kat", "musteri_yonlendirme",
                "magaza_acilis", "sayim_destek", "shrink_bilinci", "excel_hizli_giris", "mobil_odeme",
                "qr_odeme",
            ],
            [JobCategoryCatalog.ClusterEvent] =
            [
                "promotor", "stand_kurulum", "broşür_dagitim", "urun_tanitimi", "sampling",
                "fuar_hostesi", "davetiye_kontrol", "anket_tablet", "veri_toplama", "saha_raporu",
                "anket_kağıdı", "saha_fotografi", "sosyal_medya_destek", "sahne_arkasi", "kulis_destek",
                "guvenlik_koordinasyonu", "kalabalik_yonetimi", "stand_dekor", "ses_sistemi_tasima",
                "etkinlik_takip_excel",
            ],
            [JobCategoryCatalog.ClusterCleaning] =
            [
                "genel_temizlik", "cam_silme", "zemin_alma", "vakumlama", "wc_temizligi",
                "ofis_temizligi", "avm_temizligi", "kimyasal_etiket_okuma", "msds_bilinci",
                "endustriyel_makine", "yer_yikama_makinesi", "zemin_cilasi", "cop_ayristirma",
                "geri_donusum", "dezenfektan_uygulama", "derin_temizlik", "insaat_sonrasi",
                "temizlik_checklist", "vardiya_raporu", "merdiven_temizligi",
            ],
            [JobCategoryCatalog.ClusterOffice] =
            [
                "veri_girisi", "excel_formul", "pdf_duzenleme", "word_sablon", "e_posta_yonetimi",
                "takvim_planlama", "toplanti_notu", "crm_giris", "musteri_kaydi", "cagri_merkezi",
                "script_okuma", "sikayet_kaydi", "whatsapp_is", "canli_destek", "siparis_takip",
                "evrak_tasnif", "dosya_tarama", "arsiv_rafi_duzeni", "kvkk_bilinci", "rezervasyon_hatti",
            ],
        };

    #endregion Fields

    #region Utils

    /// <summary>
    /// Returns skill tags for a sector cluster (for worker/job seeding).
    /// </summary>
    internal static IReadOnlyList<string> GetTagsForCluster(string cluster)
        => TagsByCluster.TryGetValue(cluster, out IReadOnlyList<string>? list)
            ? list
            : Array.Empty<string>();

    /// <summary>
    /// Total distinct tags across clusters (for diagnostics).
    /// </summary>
    internal static int TotalTagCount
        => TagsByCluster.Values.Sum(x => x.Count);

    #endregion Utils
}
