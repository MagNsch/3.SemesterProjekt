using Maletkunst.DAL.Interfaces;
using Maletkunst.DAL.Models;

using Maletkunst.DAL.RestClient;
using Maletkunst.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Maletkunst.MVC.Controllers;

public class OrdersController : Controller
{
	IOrdersDataAccess _client;

	public OrdersController(IOrdersDataAccess client) { _client = client; }

	public ActionResult Index(CustomerInformationViewModel customerInfo)
	{
		Customer customer = new Customer
		{
			FirstName = customerInfo.FirstName,
			LastName = customerInfo.LastName,
			Address = customerInfo.Address,
			City = customerInfo.City,
			PostalCode = customerInfo.PostalCode,
			Email = customerInfo.Email,
			Phone = customerInfo.Phone
		};

		Order order = CreateOrderWithShoppingCart(customerInfo.ShoppingCart, customer);
		EmptyCart();

		return View(order);
	}

	private Order CreateOrderWithShoppingCart(string shoppingCart, Customer customer)
	{
		ShoppingCart cart = JsonConvert.DeserializeObject<ShoppingCart>(shoppingCart);

		Order order = new Order();
		order.OrderDate = DateTime.Now;
		order.Status = "Status";
		List<OrderLine> orderlines = new List<OrderLine>();

		foreach (var item in cart.Items)
		{
			Painting painting = new Painting();
			OrderLine orderline = new();
			orderline.Quantity = item.Quantity;

			painting.Title = item.Name;
			painting.Price = item.Price;
			painting.Id = item.Id;
			painting.Stock = item.Quantity;
			orderline.Painting = painting;

			orderlines.Add(orderline);
		}

		order.Total = cart.Total;
		order.OrderLines = orderlines;
		order.OrdersCustomer = customer; 
		int orderNumber = _client.CreateOrder(order);
		order.OrderNumber = orderNumber;

		return order;
	}


	// POST: OrdersController/Create
	//[HttpPost]
	//[ValidateAntiForgeryToken]
	//public ActionResult Create(Order order, Customer customer)
	//{
	//	try
	//	{
	//		order.OrdersCustomer = customer;
	//		int orderId = _client.CreateOrder(order);
	//		EmptyCart();
	//		return RedirectToAction(nameof(Index));
	//	}
	//	catch
	//	{
	//		return View();
	//	}
	//}

	private void EmptyCart()
	{
		Response.Cookies.Delete("ShoppingCart");
	}
}