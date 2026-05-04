# Cursor: kurallar ve skill’ler (Ada İş Akademi)

Kısa rehber: **ne zaman** devreye girerler, **sen ne yaparsın**.

---

## Rules (`.cursor/rules/*.mdc`)

Cursor, bu dosyaları **bağlama göre** modele ekler.

| Tür | Bu repoda | Ne zaman etkili |
|-----|-------------|------------------|
| **Her zaman** | `ada-is-akademi-agent-workflow.mdc` | Bu workspace’te açılan neredeyse her sohbet: önce analiz, `docs/tasks/` ile izlenebilir iş, mevcut CQRS yapısını bozmama. |
| **Dosya desenine göre** | `application-layer`, `domain-layer`, `api-layer`, `persistence-layer`, `infrastructure-layer`, `tests-layer`, `static-members` | Sohbet veya açık dosya yolu ilgili `globs` ile eşleşince (ör. `src/Application/**/*.cs` açıkken Application kuralları). |

**Senin yapman gereken:** Çoğu zaman **hiçbir şey**; kurallar sessizce uygulanır. Özellik bir kuralı zorlamak istersen sohbette ilgili dosyayı aç veya “`application-layer` kurallarına uy” diye yaz.

---

## Skills (`.cursor/skills/<isim>/SKILL.md`)

Skill’ler **talimat paketidir**; model onları genelde **sen tetikleyince** okur (sohbete **ekleme / @ ile ad**).

| Skill (`name`) | Ne zaman kullan |
|------------------|------------------|
| **ada-is-akademi-plan** | Plan, sprint parçalama, `docs/tasks/` altında checklist, Domain+Application+Api’yi birlikte etkileyen iş. Sohbette skill’i **ekle** veya `/ada-is-akademi-plan` benzeri ile çağır. |
| **code-format** | Belirtilen proje kökünde C# için XML doc, `#region`, üye sırası istendiğinde. Skill içindeki **ProjectRoot** sınırına uy. |
| **application-doctor** / **ddd-doctor** / **doctor** | Application veya DDD tarafında denetim / “doktor” raporu istediğinde ilgili skill’i ekle. |

**Nasıl:** Chat’te **Skills / Attach skill** ile seç veya modelin skill listesinden **ada-is-akademi-plan** vb. adıyla bağla. İsim, her `SKILL.md` dosyasındaki YAML `name:` alanıdır.

---

## Pratik akış (öneri)

1. **Büyük veya çok dosyalı iş** → önce **ada-is-akademi-plan** (gerekirse `docs/tasks/...md` güncellenir).  
2. **Kod yazarken** → ilgili katman dosyasını aç; **rules** otomatik devreye girer.  
3. **Biçim / region / sıra** → **code-format** skill’ini ekle ve kapsamı net söyle.

---

## İş listesi dosyaları

Plan skill’i ile uyumlu kalıcı checklist: **`docs/tasks/`** (ör. `phase1-identity-jobposting-application.md`). Repoda kalır; oturumlar arası “ne kaldı” için buraya bak.

---

*Kişisel skill’ler (`~/.cursor/skills/`) bu README’nin dışındadır; proje skill’leri yalnızca `.cursor/skills/` altındadır.*
