namespace BoredWeb.Models;

public class BookingDto
{
    public string UserId { get; set; }
    public string ActivityId { get; set; } = null!;
    public List<string> ParticipantsName { get; set; } = [];
    public List<string> ParticipantsEmail { get; set; } = [];
    public bool IsGroup {get; set;}

}