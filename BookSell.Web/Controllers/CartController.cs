using Microsoft.AspNetCore.Mvc;
using BookSell.Web.Services;

namespace BookSell.Web.Controllers;

public class CartController : Controller
{
    private readonly CartService _cartService;

    public CartController(CartService cartService)
    {
        _cartService = cartService;
    }

    public IActionResult Index()
    {
        var cart = _cartService.GetCartVM();
        return View(cart);
    }

    [HttpPost]
    public IActionResult Add(int bookId, string title, string? imageUrl, decimal price, int stock, int quantity = 1)
    {
        for (int i = 0; i < quantity; i++)
            _cartService.AddItem(bookId, title, imageUrl, price, stock);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Remove(int bookId)
    {
        _cartService.RemoveItem(bookId);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult UpdateQuantity(int bookId, int quantity)
    {
        _cartService.UpdateQuantity(bookId, quantity);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Clear()
    {
        _cartService.ClearCart();
        return RedirectToAction("Index");
    }
}
