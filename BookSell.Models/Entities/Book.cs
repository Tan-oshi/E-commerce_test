namespace BookSell.Models.Entities;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? ImageUrl { get; set; }
    public string? Isbn { get; set; }
    public int? YearPublished { get; set; }
    public int? PageCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Slug { get; set; }
    public bool IsFeatured { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public int AuthorId { get; set; }
    public Author? Author { get; set; }

    public int PublisherId { get; set; }
    public Publisher? Publisher { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
