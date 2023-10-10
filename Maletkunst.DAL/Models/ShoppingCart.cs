namespace Maletkunst.DAL.Models;

public class ShoppingCart
{
    public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();

    public decimal Subtotal => Items.Sum(item => item.Price * item.Quantity);

    public decimal Total => Subtotal;
}