using System.ComponentModel.DataAnnotations;

namespace BookSell.Web.ViewModels;

public class BookViewModel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    public string? ImageUrl { get; set; }
    public string? Isbn { get; set; }
    public int? YearPublished { get; set; }
    public int? PageCount { get; set; }
    public string? Slug { get; set; }
    public bool IsFeatured { get; set; }

    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }

    public int AuthorId { get; set; }
    public string? AuthorName { get; set; }

    public int PublisherId { get; set; }
    public string? PublisherName { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class BookListViewModel
{
    public List<BookViewModel> Books { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public int? CategoryId { get; set; }
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
}

public class BookCreateViewModel
{
    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    public string? ImageUrl { get; set; }
    public string? Isbn { get; set; }
    public int? YearPublished { get; set; }
    public int? PageCount { get; set; }
    public bool IsFeatured { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int AuthorId { get; set; }

    [Required]
    public int PublisherId { get; set; }
}
