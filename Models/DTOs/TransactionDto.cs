namespace BoredWeb.Models;

public class TransactionDto
{
    public Guid TransactionId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; }
    public int slotsRemaining { get; set; }
}