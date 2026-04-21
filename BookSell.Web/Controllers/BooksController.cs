using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookSell.Data;
using BookSell.Web.Services;

namespace BookSell.Web.Controllers;

public class BooksController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CartService _cartService;

    public BooksController(ApplicationDbContext context, CartService cartService)
    {
        _context = context;
        _cartService = cartService;
    }

    public async Task<IActionResult> Index(
        string? search,
        int? categoryId,
        string? sortBy,
        int page = 1)
    {
        var query = _context.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .Include(b => b.Publisher)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim().ToLower();
            query = query.Where(b =>
                b.Title.ToLower().Contains(search) ||
                b.Author!.Name.ToLower().Contains(search));
            ViewBag.Search = search;
        }

        if (categoryId.HasValue)
        {
            query = query.Where(b => b.CategoryId == categoryId.Value);
            ViewBag.CategoryId = categoryId;
        }

        query = sortBy switch
        {
            "price_asc" => query.OrderBy(b => b.Price),
            "price_desc" => query.OrderByDescending(b => b.Price),
            "newest" => query.OrderByDescending(b => b.CreatedAt),
            "title" => query.OrderBy(b => b.Title),
            _ => query.OrderByDescending(b => b.CreatedAt)
        };

        int pageSize = 12;
        int totalBooks = await query.CountAsync();
        var books = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
        ViewBag.SortBy = sortBy;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalBooks / (double)pageSize);
        ViewBag.CartItemCount = _cartService.GetCartVM().ItemCount;

        return View(books);
    }

    public async Task<IActionResult> Details(int id)
    {
        var book = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .Include(b => b.Publisher)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null) return NotFound();

        ViewBag.RelatedBooks = await _context.Books
            .Include(b => b.Author)
            .Where(b => b.CategoryId == book.CategoryId && b.Id != id)
            .Take(4)
            .ToListAsync();

        ViewBag.CartItemCount = _cartService.GetCartVM().ItemCount;
        return View(book);
    }
}
