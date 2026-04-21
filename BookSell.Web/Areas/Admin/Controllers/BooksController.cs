using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookSell.Data;
using BookSell.Models.Entities;

namespace BookSell.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class BooksController : Controller
{
    private readonly ApplicationDbContext _context;

    public BooksController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? search, int? categoryId)
    {
        var query = _context.Books
            .Include(b => b.Category)
            .Include(b => b.Author)
            .Include(b => b.Publisher)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(b => b.Title.ToLower().Contains(search)
                || b.Author!.Name.ToLower().Contains(search));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(b => b.CategoryId == categoryId.Value);
        }

        ViewBag.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
        ViewBag.Search = search;
        ViewBag.CategoryId = categoryId;

        var books = await query.OrderByDescending(b => b.CreatedAt).ToListAsync();
        return View(books);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
        ViewBag.Authors = await _context.Authors.OrderBy(a => a.Name).ToListAsync();
        ViewBag.Publishers = await _context.Publishers.OrderBy(p => p.Name).ToListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Description,Price,Stock,ImageUrl,Isbn,YearPublished,PageCount,IsFeatured,CategoryId,AuthorId,PublisherId")] Book book)
    {
        if (ModelState.IsValid)
        {
            book.Slug = book.Title.ToLower().Replace(" ", "-");
            book.CreatedAt = DateTime.UtcNow;
            _context.Add(book);
            await _context.SaveChangesAsync();
            TempData["success"] = "Thêm sách thành công!";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
        ViewBag.Authors = await _context.Authors.OrderBy(a => a.Name).ToListAsync();
        ViewBag.Publishers = await _context.Publishers.OrderBy(p => p.Name).ToListAsync();
        return View(book);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound();

        ViewBag.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
        ViewBag.Authors = await _context.Authors.OrderBy(a => a.Name).ToListAsync();
        ViewBag.Publishers = await _context.Publishers.OrderBy(p => p.Name).ToListAsync();
        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Price,Stock,ImageUrl,Isbn,YearPublished,PageCount,IsFeatured,CategoryId,AuthorId,PublisherId")] Book book)
    {
        if (id != book.Id) return NotFound();

        var existing = await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
        if (existing == null) return NotFound();

        if (ModelState.IsValid)
        {
            book.Slug = book.Title.ToLower().Replace(" ", "-");
            book.CreatedAt = existing.CreatedAt;
            _context.Update(book);
            await _context.SaveChangesAsync();
            TempData["success"] = "Cập nhật sách thành công!";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
        ViewBag.Authors = await _context.Authors.OrderBy(a => a.Name).ToListAsync();
        ViewBag.Publishers = await _context.Publishers.OrderBy(p => p.Name).ToListAsync();
        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound();

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        TempData["success"] = "Xóa sách thành công!";
        return RedirectToAction(nameof(Index));
    }
}
