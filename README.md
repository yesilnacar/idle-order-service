# IdleOrderService

IdleOrderService, sipariş yönetimi ve kullanıcı işlemlerini yöneten, çok katmanlı mimariye sahip bir .NET projesidir. Proje, API, Application, Domain, Core ve Infrastructure katmanlarından oluşur.

## Klasör Yapısı

```
src/
  IdleOrderService.Api/           # REST API katmanı
  IdleOrderService.Application/   # Uygulama servisleri ve iş mantığı
  IdleOrderService.Core/          # Temel altyapı, mediator ve event yapıları
  IdleOrderService.Domain/        # Domain modelleri ve iş kuralları
  IdleOrderService.Infra/         # Altyapı, veri erişim ve dış servis entegrasyonları
```

## Kurulum

1. Depoyu klonlayın:
   ```sh
   git clone <repo-url>
   cd IdleOrderService
   ```
2. Gerekli NuGet paketlerini yükleyin:
   ```sh
   dotnet restore
   ```

## Çalıştırma

API projesini başlatmak için:

```sh
cd src/IdleOrderService.Api
 dotnet run
```

API varsayılan olarak `http://localhost:5000` adresinde çalışacaktır.

## Katmanlar Hakkında

- **Api**: HTTP isteklerini karşılar, controller'lar burada bulunur.
- **Application**: Komutlar, handler'lar ve DTO'lar burada yer alır.
- **Core**: Ortak altyapı, event ve mediator yapıları.
- **Domain**: Temel domain modelleri ve iş kuralları.
- **Infra**: Veri erişimi, migration ve dış servis entegrasyonları.

## Katkı Sağlama

Katkıda bulunmak için lütfen bir fork oluşturun ve pull request gönderin.

## Lisans

Bu proje MIT lisansı ile lisanslanmıştır.

