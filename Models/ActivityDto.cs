namespace BoredWeb.Models;

public class ActivityDto
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
}