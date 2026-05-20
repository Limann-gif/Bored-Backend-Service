namespace BoredWeb.Models;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    /// <summary>booking</summary>
    public string Type { get; set; } = "booking";

    public decimal Amount { get; set; }

    /// <summary>paid | refunded | failed</summary>
    public string Status { get; set; } = null!;

    public string? Description { get; set; }
    public Guid ReferenceId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public ActivityBookingOrder? BookingOrder { get; set; }
}
