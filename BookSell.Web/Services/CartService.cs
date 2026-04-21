using BookSell.Web.ViewModels;

namespace BookSell.Web.Services;

public class CartService
{
    private const string CartSessionKey = "ShoppingCart";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private List<CartItemViewModel> GetCart()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session == null) return new List<CartItemViewModel>();

        var cartJson = session.GetString(CartSessionKey);
        if (string.IsNullOrEmpty(cartJson)) return new List<CartItemViewModel>();

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<CartItemViewModel>>(cartJson)
                ?? new List<CartItemViewModel>();
        }
        catch
        {
            return new List<CartItemViewModel>();
        }
    }

    private void SaveCart(List<CartItemViewModel> cart)
    {
        _httpContextAccessor.HttpContext?.Session
            .SetString(CartSessionKey, System.Text.Json.JsonSerializer.Serialize(cart));
    }

    public CartViewModel GetCartVM()
    {
        return new CartViewModel { Items = GetCart() };
    }

    public void AddItem(int bookId, string title, string? imageUrl, decimal price, int stock)
    {
        var cart = GetCart();
        var existing = cart.FirstOrDefault(c => c.BookId == bookId);
        if (existing != null)
        {
            if (existing.Quantity < stock)
                existing.Quantity++;
        }
        else
        {
            cart.Add(new CartItemViewModel
            {
                BookId = bookId,
                Title = title,
                ImageUrl = imageUrl,
                UnitPrice = price,
                Quantity = 1,
                Stock = stock
            });
        }
        SaveCart(cart);
    }

    public void RemoveItem(int bookId)
    {
        var cart = GetCart();
        cart.RemoveAll(c => c.BookId == bookId);
        SaveCart(cart);
    }

    public void UpdateQuantity(int bookId, int quantity)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(c => c.BookId == bookId);
        if (item != null)
        {
            if (quantity <= 0)
                cart.Remove(item);
            else if (quantity <= item.Stock)
                item.Quantity = quantity;
        }
        SaveCart(cart);
    }

    public void ClearCart()
    {
        SaveCart(new List<CartItemViewModel>());
    }
}
