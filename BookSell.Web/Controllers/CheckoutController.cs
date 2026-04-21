using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookSell.Data;
using BookSell.Models.Entities;
using BookSell.Web.Services;
using BookSell.Web.ViewModels;

namespace BookSell.Web.Controllers;

[Authorize]
public class CheckoutController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CartService _cartService;

    public CheckoutController(ApplicationDbContext context, CartService cartService)
    {
        _context = context;
        _cartService = cartService;
    }

    public IActionResult Index()
    {
        var cart = _cartService.GetCartVM();
        if (cart.IsEmpty)
            return RedirectToAction("Index", "Cart");

        ViewBag.CartItemCount = cart.ItemCount;
        return View(cart);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
    {
        var cart = _cartService.GetCartVM();
        if (cart.IsEmpty)
            return RedirectToAction("Index", "Cart");

        if (!ModelState.IsValid)
        {
            ViewBag.CartItemCount = cart.ItemCount;
            return View("Index", cart);
        }

        var userName = User.Identity?.Name;
        if (string.IsNullOrEmpty(userName))
            return RedirectToAction("Login", "Account", new { area = "Identity" });

        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        if (user == null)
            return RedirectToAction("Login", "Account", new { area = "Identity" });

        decimal shippingFee = cart.TotalAmount >= 200000 ? 0 : 30000;
        decimal totalAmount = cart.TotalAmount + shippingFee;

        var order = new Order
        {
            UserId = user.Id,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            TotalAmount = totalAmount,
            RecipientName = model.RecipientName,
            Phone = model.Phone,
            ShippingAddress = model.ShippingAddress,
            Note = model.Note
        };

        foreach (var item in cart.Items)
        {
            var book = await _context.Books.FindAsync(item.BookId);
            if (book == null || book.Stock < item.Quantity)
            {
                ModelState.AddModelError("", $"Sách '{item.Title}' không đủ số lượng trong kho.");
                ViewBag.CartItemCount = cart.ItemCount;
                return View("Index", cart);
            }

            book.Stock -= item.Quantity;

            order.Items.Add(new OrderItem
            {
                BookId = item.BookId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            });
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        _cartService.ClearCart();

        TempData["OrderId"] = order.Id;
        return RedirectToAction(nameof(OrderSuccess), new { id = order.Id });
    }

    public async Task<IActionResult> OrderSuccess(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Book)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return RedirectToAction("Index", "Home");

        var userName = User.Identity?.Name;
        if (userName == null || order.UserId != (await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName))?.Id)
            return RedirectToAction("Index", "Home");

        ViewBag.CartItemCount = 0;
        return View(order);
    }
}
