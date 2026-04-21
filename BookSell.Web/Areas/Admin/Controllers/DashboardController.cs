using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookSell.Data;
using BookSell.Models.Entities;

namespace BookSell.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var totalOrders = await _context.Orders.CountAsync();
        var pendingOrders = await _context.Orders
            .Where(o => o.Status == OrderStatus.Pending)
            .CountAsync();
        var totalBooks = await _context.Books.CountAsync();
        var totalUsers = await _context.Users.CountAsync();
        var recentOrders = await _context.Orders
            .Include(o => o.User)
            .OrderByDescending(o => o.OrderDate)
            .Take(5)
            .ToListAsync();
        var totalRevenue = await _context.Orders
            .Where(o => o.Status != OrderStatus.Cancelled)
            .SumAsync(o => o.TotalAmount);

        ViewBag.TotalOrders = totalOrders;
        ViewBag.PendingOrders = pendingOrders;
        ViewBag.TotalBooks = totalBooks;
        ViewBag.TotalUsers = totalUsers;
        ViewBag.RecentOrders = recentOrders;
        ViewBag.TotalRevenue = totalRevenue;

        return View();
    }
}
