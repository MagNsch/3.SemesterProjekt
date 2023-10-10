using Maletkunst.DAL.Interfaces;
using Maletkunst.DAL.Models;
using RestSharp;

namespace Maletkunst.DAL.RestClient;

public class PaintingsRestClientDao : IPaintingsDao
{

	private readonly string restUrl;
	public readonly RestSharp.RestClient client;
    public PaintingsRestClientDao()
    {
        restUrl = "https://www.maletkunst.dk/api/v1/Paintings";
        client = new RestSharp.RestClient(restUrl);
	}

	public async Task<IEnumerable<Painting>> GetAllPaintingsAsync()
	{
		var request = new RestRequest("/all", Method.Get);
		var response = await client.ExecuteAsync<List<Painting>>(request);
		return response.Data;
	}

	public int CreatePainting(Painting painting)
    {
        var request = new RestRequest("", Method.Post).AddJsonBody(painting);
        var response = client.Execute<int>(request);
        return response.Data;
    }

    public bool DeletePaintingById(int paintingId)
    {
        var request = new RestRequest($"/delete/{paintingId}", Method.Delete);
        var response = client.Execute<bool>(request);

        if (!response.IsSuccessful) { return false; }
        return true;
    }

    public bool UpdatePainting(Painting painting)
    {
        var request = new RestRequest("", Method.Put).AddJsonBody(painting);
        var response = client.ExecuteAsync<bool>(request);

        response.Wait();

        if (!response.IsCompleted) { return false; }
        return true;
    }
}