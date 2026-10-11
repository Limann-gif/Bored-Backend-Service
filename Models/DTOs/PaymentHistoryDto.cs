namespace BoredWeb.Models;

public class PaymentHistoryDto
{
    public Guid TransactionId  { get; set; } 
    public string UserName  { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } 
}