using System.Runtime.InteropServices.JavaScript;

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
    public DateOnly DateOfBirth { get; set; }
    public string Gender { get; set; }

    /// <summary>user | admin</summary>
    public string Role { get; set; } = "user";
    public DateTime JoinedAt { get; set; }
    
    // Verification fields
    public bool IsEmailVerified { get; set; } = false;
    public string? VerificationCode { get; set; }
    public DateTime? VerificationCodeExpiry { get; set; }

    // Navigation
    public ICollection<ActivityBookingOrder> BookingOrders { get; set; } = [];
    public ICollection<Complaint> Complaints { get; set; } = [];
    
    // --- NEW PASSWORD VERIFICATION FIELDS ---
    public string? PasswordVerificationCode { get; set; }
    public DateTime? PasswordVerificationCodeExpiry { get; set; }
}
