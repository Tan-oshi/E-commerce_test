using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookSell.Data;
using BookSell.Models.Entities;

namespace BookSell.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class PublishersController : Controller
{
    private readonly ApplicationDbContext _context;

    public PublishersController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var publishers = await _context.Publishers
            .Include(p => p.Books)
            .OrderBy(p => p.Name)
            .ToListAsync();
        return View(publishers);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description,Website")] Publisher publisher)
    {
        if (ModelState.IsValid)
        {
            _context.Add(publisher);
            await _context.SaveChangesAsync();
            TempData["success"] = "Thêm nhà xuất bản thành công!";
            return RedirectToAction(nameof(Index));
        }
        return View(publisher);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var publisher = await _context.Publishers.FindAsync(id);
        if (publisher == null) return NotFound();
        return View(publisher);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Website")] Publisher publisher)
    {
        if (id != publisher.Id) return NotFound();
        if (ModelState.IsValid)
        {
            _context.Update(publisher);
            await _context.SaveChangesAsync();
            TempData["success"] = "Cập nhật nhà xuất bản thành công!";
            return RedirectToAction(nameof(Index));
        }
        return View(publisher);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var publisher = await _context.Publishers.FindAsync(id);
        if (publisher == null) return NotFound();

        var hasBooks = await _context.Books.AnyAsync(b => b.PublisherId == id);
        if (hasBooks)
        {
            TempData["error"] = "Không thể xóa nhà xuất bản đang có sách!";
            return RedirectToAction(nameof(Index));
        }

        _context.Publishers.Remove(publisher);
        await _context.SaveChangesAsync();
        TempData["success"] = "Xóa nhà xuất bản thành công!";
        return RedirectToAction(nameof(Index));
    }
}
