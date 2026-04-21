namespace BookSell.Models.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Slug { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
