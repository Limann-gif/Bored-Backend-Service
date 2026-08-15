namespace BoredWeb.Models;

public class PaymentDto
{
 
    public string CustomerName { get; set; } 

    // Financial Details
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "GHS";

    // Payment Status: "pending", "paid", "failed", "refunded"
    public string Status { get; set; } = "Pending";
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    
    public Activity Activity { get; set; } = null!;
}