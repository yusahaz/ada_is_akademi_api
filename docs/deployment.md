# Ada Is Akademi - Coolify Docker Deployment Dokumani

Bu dokuman, `AdaIsAkademi` projesini Coolify uzerinden Docker Compose ile production ortamina guvenli ve tekrarlanabilir sekilde almak icin uc asamali bir yol sunar:

1. Hazirlik
2. Kurulum ve ilk deploy
3. Operasyon (izleme, backup, rollback, update)

## 1) Mimari Ozeti

`docker/docker-compose.yml` dosyasinda asagidaki servisler tanimlidir:

- `ada-is-akademi-api` (.NET API)
- `ada-is-akademi-db` (PostgreSQL)
- `ada-is-akademi-redis` (Redis)
- `ada-is-akademi-minio` (MinIO object storage)
- `ada-is-akademi-dbview` (pgAdmin)

Kalici volume'lar:

- `db_data`
- `redis_data`
- `minio_data`
- `dbview_data`
- `api_dataprotection_keys`

## 2) On Kosullar

### 2.1 Sunucu ve altyapi

- Coolify kurulu Linux host
- Yeterli kaynak (baslangic icin en az):
  - 4 vCPU
  - 8 GB RAM
  - 80+ GB disk (log + image + volume buyumesi icin bos alan birak)
- Docker engine stabil durumda
- Saat senkronizasyonu aktif (NTP)

### 2.2 Alan adi ve ag

- API icin domain/subdomain (ornek: `api.alanadiniz.com`)
- DNS kaydi Coolify host IP'sine yonlu
- 80/443 dis erisim acik
- Gerekmiyorsa DB/Redis/MinIO/pgAdmin portlarini public acma

### 2.3 Kaynak kod

- Coolify'nin ulasabilecegi Git repo
- Deploy branch belirli (`main`, `master` veya `release/*`)
- Branch protection ve en azindan temel code review akisi tavsiye edilir

## 3) Bu Repo Icin Kritik Build Bilgisi

Bu repoda API image buildi standart tek-dizin degil, compose ile kok dizinden alinacak sekilde ayarlidir:

- Compose dosyasi: `docker/docker-compose.yml`
- API Dockerfile: `src/Api/Dockerfile`
- Compose icindeki API build:
  - `context: ../..`
  - `dockerfile: AdaIsAkademi/src/Api/Dockerfile`

Bu nedenle Coolify ayarlari dogru olmazsa build patlar.

**Zorunlu ayar:**

- Base Directory: `docker`
- Compose File: `docker-compose.yml`

## 4) Coolify Uzerinde Uygulama Olusturma (Alan Alan)

1. `New Resource -> Application`
2. Source: ilgili Git provider/repo
3. Type/Build Pack: `Docker Compose`
4. Application ayarlari:
   - Base Directory: `docker`
   - Compose File: `docker-compose.yml`
   - Branch: production branch
5. Save

## 5) Environment Variables ve Secret Yonetimi

`docker/.env` dosyasindaki degerler lokal gelistirme icin referanstir. Production'da ayni degerleri kullanma.

### 5.1 Zorunlu degiskenler

Asagidaki anahtarlar Coolify Environment Variables alanina girilmeli:

- `DB_NAME`
- `DB_USER`
- `DB_PASS`
- `DB_PORT`
- `REDIS_PORT`
- `REDIS_PASSWORD`
- `MINIO_PORT`
- `MINIO_PORT_SECURE`
- `MINIO_ROOT_USER`
- `MINIO_ROOT_PASSWORD`
- `DBVIEW_USER`
- `DBVIEW_PASSWORD`
- `DBVIEW_PORT`
- `API_PORT`
- `ASPNETCORE_URLS`
- `ASPNETCORE_ENVIRONMENT`
- `JWT_ISSUER`
- `JWT_AUDIENCE`
- `JWT_SIGNING_KEY`
- `JWT_EXPIRE_MINUTES`
- `JWT_REFRESH_TOKEN_EXPIRE_DAYS`
- `REDIS_CONFIG_ENABLED`
- `REDIS_CONFIG_CONNECTION`
- `REDIS_CONFIG_INSTANCE_NAME`

### 5.2 Production deger onerileri

- `ASPNETCORE_ENVIRONMENT=Production`
- `ASPNETCORE_URLS=http://+:8080`
- `JWT_SIGNING_KEY`: en az 64+ karakter rastgele
- `REDIS_CONFIG_CONNECTION`: `ada-is-akademi-redis:6379,password=<REDIS_PASSWORD>`

### 5.3 Guvenlik kurallari

- Secret degerleri repoya yazma
- Coolify secret/env alaninda tut
- Periyodik secret rotation uygula
- Ayricalikli erisimleri sinirla (kim secret gorebilir/degistirebilir)

## 6) Domain, Reverse Proxy ve SSL

Public'e acilmasi gereken birincil servis `ada-is-akademi-api` olmalidir.

### 6.1 Onerilen ayar

- Domain: `api.alanadiniz.com`
- Target service: `ada-is-akademi-api`
- Internal port: `8080`
- HTTPS: Let's Encrypt aktif
- HTTP -> HTTPS redirect aktif

### 6.2 Servis bazli erisim modeli

- `api`: public
- `db`, `redis`, `minio`, `dbview`: internal

`dbview` ve `minio console` gerekiyorsa VPN veya IP allowlist ile kisitla.

## 7) Ilk Deploy Runbook

1. Tanimlari kaydet
2. `Deploy` tetikle
3. Build loglarini kontrol et
4. Runtime loglarini kontrol et
5. Domain uzerinden smoke test yap

### 7.1 Build log kontrolu

- Dockerfile path dogru mu?
- `dotnet restore/build/publish` adimlari hatasiz mi?
- Image olustu mu?

### 7.2 Runtime log kontrolu

- API ayaaga kalkti mi?
- `depends_on` healthcheck nedeniyle beklemede kalma var mi?
- DB baglanti/acilis hatasi var mi?
- Redis baglanti hatasi var mi?

### 7.3 Ilk smoke test

- Yetki gerektirmeyen bir endpoint `200` donuyor mu?
- Yetki gerektiren endpoint dogru auth davranisi veriyor mu (`401/403`)
- Temel CRUD senaryosu calisiyor mu?

## 8) Veritabani Migration Stratejisi

Migration dosyalari repoda var; production'a cikmadan once migration'in nasil ve ne zaman kosacagi net olarak secilmelidir.

Bu projede en guvenli yaklasim:

1. Staging'de migration test et
2. Production deploy oncesi migration'i kontrollu kos
3. Migration basariliysa API release'ini ac

### 8.1 Migration oncesi kontrol listesi

- [ ] Migration dosyalari review edildi
- [ ] Geriye donus plani (rollback) yazildi
- [ ] Production DB backup alindi
- [ ] Staging'de migration suresi olculdu
- [ ] Uygulama ile schema versiyonu uyumu dogrulandi

### 8.2 Manuel migration calistirma (onerilen baseline)

En dusuk riskli model, migration'i release adimindan ayri ve manuel/denetimli kosmaktir.

Adimlar:

1. Production backup al
2. Coolify deploy'u durdurmak yerine read-only/kisa maintenance penceresi planla
3. Migration komutunu bir kerelik calistir
4. Basarili oldugunu DB uzerinde dogrula
5. API deploy'u tamamla
6. Smoke test yap

Not:

- Migration ve API deploy'unu tek adima sikistirmak yerine ayri adimda yapmak, hata durumunda geri donusu kolaylastirir.

### 8.3 Coolify uzerinden one-off migration job modeli

Coolify'de API'den ayri bir "migration job" tanimlayabilirsin. Mantik:

- Ayni kodu kullanir
- Sadece migration kosar
- Basarili olursa API deploy edilir

Ornek akis:

1. `migration` adli ayri bir resource/job olustur
2. Ayni branch'ten build et
3. Start command'i migration komutu olsun
4. Job `0` kodu ile biterse API deploy tetikle

Bu model, "once schema sonra app" prensibini net uygular.

### 8.4 Uygulama acilisinda otomatik migration (yapilabilir, dikkatli kullan)

Evet, otomatik migration yapilabilir; ancak production'da kontrolsuz kullanmak risklidir.

Artisi:

- Operasyon adimi azalir
- Kucuk ekiplerde hizli teslimat saglar

Eksisi:

- Ayni anda birden fazla API replica aciliyorsa race condition olabilir
- Uzun migration app startup'i geciktirir
- Basarisiz migration tum deployment'i bloke eder

Oneri:

- Sadece tek replica + kucuk migration + dusuk trafik ortamlarda dusun
- Buyuk ve kritik ortamlarda ayrik migration job tercih et

### 8.5 Dockerfile icinde migration calistirma (onerilmez)

Soruya net cevap: teorik olarak denenebilir, pratikte production icin **onerilmez**.

Neden:

- Dockerfile build asamasi image olusturur; bu asamada production DB'ye baglanmak dogru degil
- Build ortami ile runtime ortami ayridir
- Tekrarlanabilirlik ve guvenlik bozulur

Kural:

- Dockerfile sadece build/publish/isletim icin kullanilsin
- Migration runtime'da (job ya da kontrollu startup) kosulsun

### 8.6 Docker Compose ile otomatik migration (mumkun)

Compose tarafinda otomatiklestirme icin iki model vardir:

1. `migration` adli ayri servis (onerilen)
2. API `command` icinde once migration sonra app baslatma (ikincil tercih)

#### Model A - Ayri migration servisi (onerilen)

Yapi:

- `ada-is-akademi-migration` servisi tek sefer calisir
- DB healthy olduktan sonra migration kosar
- API servisi migration basarili olduktan sonra baslar

Avantaj:

- Davranis acik ve denetlenebilir
- Hata API'yi ayri etkiler

#### Model B - API baslangic komutuna migration eklemek

Yapi:

- API container start command: "migration calistir -> app'i baslat"

Risk:

- Her restart'ta migration adimi devreye girer
- Scale-out senaryosunda esit zamanli migration riski olur

### 8.7 Teknik uygulama secenekleri

Pratikte migration calistirmak icin 3 teknik secenek vardir:

1. `dotnet ef database update`
2. EF migration bundle (`dotnet ef migrations bundle`)
3. Kod icinden `Database.Migrate()`

#### Secenek 1 - dotnet ef

- SDK ve kaynak kod gerektirir
- Runtime image genelde SDK icermez
- Daha cok CI/CD veya ayri migration image icin uygundur

#### Secenek 2 - Migration bundle (production-friendly)

- Build asamasinda migration executable uretilir
- Runtime'da sadece bu binary calistirilir
- CI/CD ve Coolify job modeline iyi uyar

#### Secenek 3 - Database.Migrate() startup

- Uygulama koduna migration adimi eklenir
- En kolay kurulum ama kontrol seviyesi dusuk
- Kucuk ortamlarda kabul edilebilir

### 8.8 Hangi modeli secmeliyim?

Production ve buyume hedefi olan sistemler icin sirali oneri:

1. **En iyi:** Ayri migration job + API deploy
2. **Iyi:** Migration bundle + controlled run
3. **Kabul edilebilir (kucuk sistem):** Startup `Database.Migrate()`
4. **Kacin:** Dockerfile build icinde migration

### 8.9 Ornek release sirası (guvenli)

1. Backup al
2. Migration job kos
3. Basariliysa API deploy et
4. Smoke test + log kontrolu
5. Problem varsa rollback runbook uygula

## 9) Kalicilik ve Veri Koruma

Compose volume'larin host tarafinda kalici oldugunu dogrula.

Ozellikle kritik:

- `db_data`: tum is verisi
- `minio_data`: dosyalar/object'ler
- `api_dataprotection_keys`: token/cookie sifreleme anahtarlari

`api_dataprotection_keys` silinirse daha once uretilen bazi token/cookie dogrulamalari etkilenebilir.

**Opsiyonel anahtar XML sifrelemesi:** API icinde `DataProtection:ProtectKeysCertificatePath` gecerli bir dosya yoluna ayarlanirsa (ve dosya container icinde okunabilirse), kalici anahtar dosyalari ek olarak X.509 ile `ProtectKeysWithCertificate` ile korunur. Yukleme `X509CertificateLoader.LoadPkcs12FromFile` ile yapilir — dosya tipik olarak **PKCS#12 (.pfx/.p12)** olmalidir; parola zorunluysa `DataProtection:ProtectKeysCertificatePassword` kullanilir. Uygulama adi `SetApplicationName("Azoxia.AdaIsAkademi.Api")` ile sabittir; birden fazla API ornegi ayni anahtar hacmini paylasirken bu adin eslesmesi gerekir.

## 10) Backup ve Restore

### 10.1 Backup kapsami

- PostgreSQL dump/snapshot
- MinIO object verisi
- Kritk env/secret envanteri (degerin kendisi degil, hangi key'lerin oldugu)

### 10.2 Backup sikligi

- En az gunluk DB backup
- Is kritigine gore saatlik artimli strateji
- Backup retention: 7/14/30 gun politikasi

### 10.3 Restore tatbikati

- Aylik en az bir kez staging uzerinde restore test yap
- RPO/RTO hedefini dokumante et

## 11) Guncelleme ve Release Akisi

### 11.1 Tavsiye edilen release modeli

1. Feature branch -> PR
2. CI test/build
3. `main` merge
4. Coolify auto deploy veya manuel deploy
5. Smoke test

### 11.2 Deploy tipi

- Manuel deploy: kontrollu release
- Auto deploy: hizli teslimat (main push'ta)

Kritik sistemlerde mesai disi deploy pencere politikasi belirle.

## 12) Rollback Runbook

Basarisiz release durumunda:

1. Son stabil commit/tag sec
2. Coolify'de bu commit'e don ve tekrar deploy et
3. Sistem dogrulama yap:
   - API health
   - DB baglanti
   - kritik endpointler
4. Olay kaydini ac ve kok neden analizi planla

Not: DB migration geri alinmazsa, kod rollback tek basina yetmeyebilir. Bu nedenle migration rollback plani ayrica zorunludur.

## 13) Go-Live Oncesi Checklist

- [ ] Base Directory `docker`, Compose file `docker-compose.yml`
- [ ] `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Tum secret degerleri production'a ozel
- [ ] API domain/SSL aktif
- [ ] Publice acik sadece API
- [ ] Backup plani aktif ve ilk backup alindi
- [ ] Rollback adimlari yazili ve test edildi
- [ ] Temel smoke test senaryolari basarili
- [ ] Hangfire dashboard (`/automation/jobs`) erisim kontrolu yapildi

## 14) Operasyonel Izleme Onerileri

- Uygulama loglarinda minimum:
  - baslangic event'leri
  - exception stack trace
  - DB/Redis baglanti hatalari
- Alarmlar:
  - API restart dongusu
  - 5xx oran artisi
  - disk doluluk
  - DB baglanti hatalari

## 15) Sik Karsilasilan Sorunlar

### 15.1 Build path hatasi

Belirti:

- Dockerfile bulunamiyor
- COPY path hatasi

Kontrol:

- Base Directory `docker` mu?
- Compose file `docker-compose.yml` mi?

### 15.2 API DB baglanamiyor

Kontrol:

- `DbConfig__Host=ada-is-akademi-db`
- `DbConfig__Port=5432`
- `DB_USER/DB_PASS/DB_NAME` dogru mu?
- DB container healthy mi?

### 15.3 Redis baglanti hatasi

Kontrol:

- `REDIS_CONFIG_CONNECTION` formati dogru mu?
- Sifre dogru mu?
- Redis healthcheck geciyor mu?

### 15.4 Auth/token sorunlari

Kontrol:

- `JWT_*` degerleri tutarli mi?
- `api_dataprotection_keys` volume kalici mi?
- Deploy sonrasi key kaybi oldu mu?

## 16) Ek Guvenlik Notlari

- pgAdmin'i production'da surekli acik tutma; gerekince gecici ac
- MinIO console erisimini sinirla
- Coolify kullanici rollerini least privilege ile yonet
- Host seviyesinde firewall kurallariyla yalnizca gerekli portlari ac

---

Bu dokuman canli ortama gecis icin operasyonel temel sunar. Proje buyudukce staging ortami, otomatik smoke test ve merkezi log/metric stack (or. Loki/Prometheus/Grafana benzeri) eklenmesi tavsiye edilir.

