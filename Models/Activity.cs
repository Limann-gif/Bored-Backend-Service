namespace BoredWeb.Models;

public class Activity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Category { get; set; } = null!;
    public decimal Price { get; set; }
    public int Capacity { get; set; }
    public int GroupSizeMin { get; set; }
    public int GroupSizeMax { get; set; }
    public string Location { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    /// <summary>forming | confirmed | completed | cancelled</summary>
    public string Status { get; set; } = "forming";

    public DateTime ActivityDate { get; set; }
    public DateTime? CancellationDate { get; set; }
    public string? CancellationReason { get; set; }

    // Navigation
    public ICollection<ActivityBookingOrder> BookingOrders { get; set; } = [];
}
