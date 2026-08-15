namespace BoredWeb.Models;

public class GroupManagement
{
    public string NameOfActivity {get; set;}
    public int NumberOfParticipants {get; set;}
    public string ConfirmationStatus {get; set;}
    public DateTime CreatedAt {get; set;}
    public List<GroupMembers> Members {get; set;}
    public List<Reviews> Reviews {get; set;}
}

public class GroupMembers
{
    public string Name {get; set;}
    public bool IsPaymentCompleted {get; set;}
    
}

public class Reviews
{
    public string CommentId {get; set;}
    public string Comment {get; set;}
}