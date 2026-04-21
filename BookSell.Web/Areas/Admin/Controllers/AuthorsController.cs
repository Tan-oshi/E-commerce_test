using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookSell.Data;
using BookSell.Models.Entities;

namespace BookSell.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class AuthorsController : Controller
{
    private readonly ApplicationDbContext _context;

    public AuthorsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var authors = await _context.Authors
            .Include(a => a.Books)
            .OrderBy(a => a.Name)
            .ToListAsync();
        return View(authors);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Bio,ImageUrl")] Author author)
    {
        if (ModelState.IsValid)
        {
            _context.Add(author);
            await _context.SaveChangesAsync();
            TempData["success"] = "Thêm tác giả thành công!";
            return RedirectToAction(nameof(Index));
        }
        return View(author);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author == null) return NotFound();
        return View(author);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Bio,ImageUrl")] Author author)
    {
        if (id != author.Id) return NotFound();
        if (ModelState.IsValid)
        {
            _context.Update(author);
            await _context.SaveChangesAsync();
            TempData["success"] = "Cập nhật tác giả thành công!";
            return RedirectToAction(nameof(Index));
        }
        return View(author);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author == null) return NotFound();

        var hasBooks = await _context.Books.AnyAsync(b => b.AuthorId == id);
        if (hasBooks)
        {
            TempData["error"] = "Không thể xóa tác giả đang có sách!";
            return RedirectToAction(nameof(Index));
        }

        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();
        TempData["success"] = "Xóa tác giả thành công!";
        return RedirectToAction(nameof(Index));
    }
}
