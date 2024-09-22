namespace PaymentService.Models;

public class Payment
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string ReservationId { get; set; }
    public DateTime CreatedAt { get; set; }
}