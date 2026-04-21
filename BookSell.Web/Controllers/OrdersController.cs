using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookSell.Data;
using BookSell.Models.Entities;
using BookSell.Web.Services;

namespace BookSell.Web.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CartService _cartService;

    public OrdersController(ApplicationDbContext context, CartService cartService)
    {
        _context = context;
        _cartService = cartService;
    }

    public async Task<IActionResult> Index()
    {
        var userName = User.Identity?.Name;
        if (string.IsNullOrEmpty(userName))
            return RedirectToAction("Index", "Home");

        var userId = await _context.Users
            .Where(u => u.UserName == userName)
            .Select(u => u.Id)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Index", "Home");

        var orders = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Book)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        ViewBag.CartItemCount = _cartService.GetCartVM().ItemCount;
        return View(orders);
    }
}
