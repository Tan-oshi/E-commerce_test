using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookSell.Data;
using BookSell.Web.Services;

namespace BookSell.Web.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CartService _cartService;

    public HomeController(ApplicationDbContext context, CartService cartService)
    {
        _context = context;
        _cartService = cartService;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();
        ViewBag.FeaturedBooks = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .Where(b => b.IsFeatured)
            .Take(8)
            .ToListAsync();
        ViewBag.NewestBooks = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .OrderByDescending(b => b.CreatedAt)
            .Take(8)
            .ToListAsync();
        ViewBag.CartItemCount = _cartService.GetCartVM().ItemCount;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
