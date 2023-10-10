using Maletkunst.DAL.Interfaces;
using Maletkunst.DAL.Models;
using RestSharp;

namespace Maletkunst.DAL.RestClient;

public class PaintingsRestClientDataAccess : IPaintingsDataAccess
{
	private readonly string restUrl;
	public readonly RestSharp.RestClient client;

	public PaintingsRestClientDataAccess()
	{
		restUrl = "https://www.maletkunst.dk/api/v1/Paintings";
		client = new RestSharp.RestClient(restUrl);
	}

	public IEnumerable<Painting> GetAllAvailablePaintings()
	{
		var request = new RestRequest();
		var response = client.Execute<IEnumerable<Painting>>(request);
		return response.Data;
	}


	public IEnumerable<Painting> GetAllPaintingsByFreeSearch(string searchString, string category)
	{
		// Prepare the base request
		var request = new RestRequest();

		// If searchString is not empty and category is not empty, add both to the request
		if (!string.IsNullOrEmpty(searchString) && !string.IsNullOrEmpty(category))
		{
			request.Resource = $"search/{searchString}/category/{category}";
		}
		// If searchString is not empty and category is empty, add searchString to the request
		else if (!string.IsNullOrEmpty(searchString))
		{
			request.Resource = $"search/{searchString}";
		}
		// If searchString is empty and category is not empty, add category to the request
		else if (!string.IsNullOrEmpty(category))
		{
			request.Resource = $"category/{category}";
		}

		request.Method = Method.Get;

		var response = client.Execute<IEnumerable<Painting>>(request);
		return response.Data;
	}

	public IEnumerable<Painting> GetAllPaintingsByFreeSearch(string searchString)
	{
		var request = new RestRequest($"search/{searchString}", Method.Get);
		var response = client.Execute<IEnumerable<Painting>>(request);
		return response.Data;
	}

	public IEnumerable<Painting> GetAllPaintingsByCategory(string category)
	{
		var request = new RestRequest($"category/{category}", Method.Get);
		var response = client.Execute<IEnumerable<Painting>>(request);
		return response.Data;
	}

	public Painting GetPaintingById(int id)
	{
		var request = new RestRequest("{id}");
		request.AddUrlSegment("id", id);
		var response = client.Execute<Painting>(request);
		return response.Data;
	}
}