namespace BookSell.Models.Entities;

public class Publisher
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Website { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
