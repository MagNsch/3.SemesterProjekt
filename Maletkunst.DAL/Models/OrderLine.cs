namespace Maletkunst.DAL.Models;

public class OrderLine
{
    public int OrderLineId { get; set; }
    public int Quantity { get; set; }
    public decimal SubTotal { get; set; }
    public Painting Painting { get; set; }
}