using Maletkunst.DAL.Interfaces;
using Maletkunst.DAL.Models;
using RestSharp;

namespace Maletkunst.DAL.RestClient;

public class OrdersRestClientDataAccess : IOrdersDataAccess
{
	private readonly string restUrl;
	public readonly RestSharp.RestClient client;

	public OrdersRestClientDataAccess()
	{
		//restUrl = "https://www.maletkunst.dk/api/v1/Orders";
		restUrl = "https://localhost:7274/v1/Orders";

        client = new RestSharp.RestClient(restUrl);
	}

	public int CreateOrder(Order order)
	{
		var request = new RestRequest("", Method.Post).AddJsonBody(order);
		var response = client.Execute<int>(request);
		return response.Data;
	}

	public IEnumerable<Order> GetAllOrders()
	{
		var request = new RestRequest("", Method.Get);
		var response = client.Execute<List<Order>>(request);
		return response.Data;
	}
}