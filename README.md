# Event-Driven-Hotel-Example
event-Driven Hotel Booking System using Go and C# with RabbitMQ

# Event-Driven Otel Rezervasyon Sistemi

Bu proje, Go ile geliştirilen Rezervasyon Servisi ve C# ile geliştirilen Ödeme ve Bildirim Servisleri kullanılarak oluşturulmuş Event-Driven bir otel rezervasyon sistemini göstermektedir. Servisler arası iletişimi sağlamak için RabbitMQ mesaj aracısı kullanılmıştır.

## Servisler

1. **Rezervasyon Servisi (Go)**: Rezervasyonların oluşturulmasını yönetir ve olayları yayınlar.
2. **Ödeme Servisi (C#)**: Ödemeleri işler ve ödeme olaylarını yayınlar.
3. **Bildirim Servisi (C#)**: Hem Rezervasyon hem de Ödeme servislerinden gelen olayları tüketir ve bildirimleri gönderir.

## Kullanılan Teknolojiler

- Go
- C# (.NET Core)
- RabbitMQ

### Servisleri Çalıştırma

1. RabbitMQ sunucusunu başlatın
2. Rezervasyon Servisini çalıştırın (Go):
   ```
   cd go-services/reservation-service
   go run main.go
   ```
3. Ödeme Servisini çalıştırın (C#):
   ```
   cd csharp-services/PaymentService
   dotnet run
   ```
4. Bildirim Servisini çalıştırın (C#):
   ```
   cd csharp-services/NotificationService
   dotnet run
   ```

## API Uç Noktaları

- Rezervasyon Servisi: `POST /reservations`
- Ödeme Servisi: `POST /payments`

## Olay Akışı

1. Bir rezervasyon oluşturulduğunda, Rezervasyon Servisi bir "reservation.created" olayı yayınlar.
2. Bir ödeme işlendiğinde, Ödeme Servisi bir "payment.processed" olayı yayınlar.
3. Bildirim Servisi her iki tür olayı da tüketir ve uygun bildirimleri gönderebilir.
