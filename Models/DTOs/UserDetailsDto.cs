namespace BoredWeb.Models;

public class UserDetailsDto
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public int BookingCount { get; set; }
    public int Age{get; set;}
    public string? Phone{get; set;}
    public string? LocationAddress{get; set;}
    public string? Occupation{get; set;}
    public int ActivitiesNumber{get; set;}
    public int GroupsJoinedNumber{get; set;}
    public int CompletedActivityNumber{get; set;}
    public DateTime JoinedAt {get; set;}
    
}