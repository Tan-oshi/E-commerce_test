namespace BookSell.Web.ViewModels;

public class CartItemViewModel
{
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal => UnitPrice * Quantity;
    public int Stock { get; set; }
}

public class CartViewModel
{
    public List<CartItemViewModel> Items { get; set; } = new();
    public decimal TotalAmount => Items.Sum(i => i.Subtotal);
    public int ItemCount => Items.Sum(i => i.Quantity);
    public bool IsEmpty => !Items.Any();
}

public class AddToCartViewModel
{
    public int BookId { get; set; }
    public int Quantity { get; set; } = 1;
}
