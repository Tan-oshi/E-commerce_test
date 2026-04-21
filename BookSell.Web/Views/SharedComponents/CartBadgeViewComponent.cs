using Microsoft.AspNetCore.Mvc;
using BookSell.Web.Services;

namespace BookSell.Web.Views.SharedComponents;

public class CartBadgeViewComponent : ViewComponent
{
    private readonly CartService _cartService;

    public CartBadgeViewComponent(CartService cartService)
    {
        _cartService = cartService;
    }

    public IViewComponentResult Invoke()
    {
        var count = _cartService.GetCartVM().ItemCount;
        return View(count);
    }
}
