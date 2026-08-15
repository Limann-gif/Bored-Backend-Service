namespace BoredWeb.Models;

public class ActivityBookingOrder
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ActivityId { get; set; }
    public DateTime CreatedAt { get; set; }

    /// <summary>pending | failed | success</summary>
    public string PaymentStatus { get; set; } = "pending";

    /// <summary>approved | denied</summary>
    public string ConfirmationStatus { get; set; } = null!;

    public Guid TransactionId { get; set; }
    public bool IsGroupBooking { get; set; }
    public decimal AmountPaid { get; set; }

    public List<string> ParticipantsName { get; set; } = [];
    public List<string> ParticipantsEmail { get; set; } = [];

    // Navigation
    public User User { get; set; } = null!;
    public Activity Activity { get; set; } = null!;
}
