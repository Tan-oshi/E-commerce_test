using System.ComponentModel.DataAnnotations;

namespace BookSell.Web.ViewModels;

public class CheckoutViewModel
{
    public CartViewModel Cart { get; set; } = new();

    [Required]
    [MaxLength(200)]
    public string RecipientName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string ShippingAddress { get; set; } = string.Empty;

    public string? Note { get; set; }

    public string PaymentMethod { get; set; } = "COD";
}
