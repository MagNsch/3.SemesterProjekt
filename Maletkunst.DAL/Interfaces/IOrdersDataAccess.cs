using Maletkunst.DAL.Models;

namespace Maletkunst.DAL.Interfaces;

public interface IOrdersDataAccess
{
    IEnumerable<Order> GetAllOrders();
    int CreateOrder(Order order);
}