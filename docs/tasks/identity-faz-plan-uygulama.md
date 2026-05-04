# Ada İş Akademi Identity Faz Planı — Uygulama

## Hedef
- Identity akışlarında (kayıt, login, refresh, email verification, me) contract/validation/güvenlik tutarlılığını mevcut CQRS yapısını bozmadan güçlendirmek.

## Kapsam
- `SystemUser` command/query modelleri, validatorlar, handler güvenlik kontrolleri.
- API controller sözleşme metaverisi ve özet tutarlılığı.
- Identity tarafı uygulama testleri.

## Kapsam Dışı
- MFA / sosyal login.
- Harici kimlik sağlayıcı entegrasyonu.
- Yeni authorization matrisi.

## Görevler
- [x] Identity command/validator/handler + API endpoint envanterini çıkar ve gap listesini belirle.
- [x] Query read model sözleşmesini `ModelBase` standardına hizala.
- [x] Email verification request validatorına `ExpiresAt` gelecekte olmalı kuralını ekle.
- [x] Refresh token handlerında aktif/lock kontrolünü login akışıyla hizala.
- [x] Identity validator ve handler davranışları için kritik testleri ekle.
- [x] API action özet metnindeki yanlış açıklamayı düzelt.

## Touch List
- `src/Application/Queries/SystemUser/SystemUserMeModel.cs`
- `src/Application/ApplicationValidationCodes.cs`
- `src/Application/Commands/SystemUser/RequestSystemUserEmailVerificationCommand.cs`
- `src/Application/Commands/SystemUser/RefreshSystemUserTokenCommand.cs`
- `src/Api/Controllers/SystemUsersController.cs`
- `tests/ApplicationTests/SystemUserIdentityValidatorsTests.cs`
- `tests/ApplicationTests/Phase1CriticalCommandHandlersTests.cs`

## Done / Follow-ups
- Done: Identity refresh akışında pasif/lock hesapların token yenilemesi engellendi.
- Done: Email verification request için geçmiş tarih doğrulaması pipeline seviyesine taşındı.
- Follow-up: Login/Refresh claim üretimindeki tekrar eden kod, ayrı instance tabanlı bir claim builder servisine taşınabilir.
