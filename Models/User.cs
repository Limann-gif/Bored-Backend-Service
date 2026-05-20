namespace BoredWeb.Models;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Bio { get; set; } 
    public string? Occupation { get; set; }
    public string? LocationAddress { get; set; }

    /// <summary>user | admin</summary>
    public string Role { get; set; } = "user";

    public DateTime JoinedAt { get; set; }

    // Navigation
    public ICollection<ActivityBookingOrder> BookingOrders { get; set; } = [];
    public ICollection<Transaction> Transactions { get; set; } = [];
    public ICollection<Complaint> Complaints { get; set; } = [];
}
