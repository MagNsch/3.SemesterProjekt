namespace Maletkunst.DAL.Models;

public class ShoppingCartItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}