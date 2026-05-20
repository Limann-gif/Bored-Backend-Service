namespace BoredWeb.Models;

public class Complaint
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;

    /// <summary>e.g. Safety | Technical</summary>
    public string Category { get; set; } = null!;

    /// <summary>open | resolved | dismissed</summary>
    public string Status { get; set; } = "open";

    public DateTime? ResolvedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public User User { get; set; } = null!;
}
