namespace BoredWeb.Models.DTOs;

public class BookingDto
{
    public Guid UserId { get; set; }
    public Guid ActivityId { get; set; }
    public List<string> ParticipantsName { get; set; } = [];
    public List<string> ParticipantsPhoneNumber { get; set; } = [];
    public bool IsGroup {get; set;}

}