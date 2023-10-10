using Maletkunst.DAL.Interfaces;
using Maletkunst.DAL.Models;
using System.Data.SqlClient;

namespace Maletkunst.DAL.SQL;

public class PaintingsSqlDataAccess : IPaintingsDataAccess
{
	private const string connectionString = @"Data Source=hildur.ucn.dk; Initial Catalog=DMA-CSD-V221_10434660; User ID=DMA-CSD-V221_10434660; Password=Password1!;";

	public IEnumerable<Painting> GetAllAvailablePaintings()
	{
		string queryString = @"SELECT * FROM Painting WHERE stock > 0";
		using SqlConnection connection = new SqlConnection(connectionString);
		SqlCommand command = new SqlCommand(queryString, connection);

		connection.Open();

		try { return BuildListOfPaintings(command); }

		catch (Exception ex) { throw new Exception("ERROR occurred while getting all paintings", ex); }
	}

	public IEnumerable<Painting> GetAllPaintingsByFreeSearch(string searchString)
	{
		string queryString = @"SELECT * FROM Painting WHERE Stock > 0 AND (Title LIKE '%' + @searchString + '%' OR Artist LIKE '%' + @searchString + '%' OR [Description] LIKE '%' + @searchString + '%')";
		using SqlConnection connection = new SqlConnection(connectionString);
		SqlCommand command = new SqlCommand(queryString, connection);

		command.Parameters.AddWithValue("@searchString", searchString);

		connection.Open();

		try { return BuildListOfPaintings(command); }

		catch (Exception ex) { throw new Exception("ERROR occurred while getting all paintings", ex); }
	}

	public IEnumerable<Painting> GetAllPaintingsByCategory(string category)
	{
		string queryString = @"SELECT * FROM Painting WHERE stock > 0 and category like '%' + @category + '%'";
		using SqlConnection connection = new SqlConnection(connectionString);
		SqlCommand command = new SqlCommand(queryString, connection);
		command.Parameters.AddWithValue("@category", category);

		connection.Open();
		try
		{
			return BuildListOfPaintings(command);

		}
		catch (Exception ex)
		{
			throw new Exception("ERROR occurred while getting all paintings", ex);
		}
	}

	//public IEnumerable<Painting> GetAllPaintingsByCategoryAndFreeSearch(string category, string searchString)
	//{
	//    string queryString = @"SELECT * FROM Painting WHERE Stock > 0 AND Category like '%' + @category + '%' AND (Title LIKE '%' + @searchString + '%' OR Artist LIKE '%' + @searchString + '%' OR [Description] LIKE '%' + @searchString + '%')";
	//    using SqlConnection connection = new SqlConnection(connectionString);
	//    SqlCommand command = new SqlCommand(queryString, connection);

	//    command.Parameters.AddWithValue("@category", category);
	//    command.Parameters.AddWithValue("@searchString", searchString);

	//    connection.Open();

	//    try { return BuildListOfPaintings(command); }

	//    catch (Exception ex) { throw new Exception("ERROR occurred while getting all paintings", ex); }
	//}

	public IEnumerable<Painting> GetAllPaintingsByFreeSearch(string searchString, string category)
	{
		string queryString = @"SELECT * FROM Painting WHERE Stock > 0 AND Category LIKE '%' + @category + '%' AND (Title LIKE '%' + @searchString + '%' OR Artist LIKE '%' + @searchString + '%' OR [Description] LIKE '%' + @searchString + '%')";
		using SqlConnection connection = new SqlConnection(connectionString);
		SqlCommand command = new SqlCommand(queryString, connection);

		command.Parameters.AddWithValue("@category", category);
		command.Parameters.AddWithValue("@searchString", searchString);

		connection.Open();

		try { return BuildListOfPaintings(command); }

		catch (Exception ex) { throw new Exception("ERROR occurred while getting all paintings", ex); }
	}


	private IEnumerable<Painting> BuildListOfPaintings(SqlCommand command)
	{
		SqlDataReader reader = command.ExecuteReader();
		ICollection<Painting> paintings = new List<Painting>();
		while (reader.Read())
		{
			paintings.Add(new Painting()
			{
				Id = (int)reader["ID"],
				Title = (string)reader["Title"],
				Price = (decimal)reader["Price"],
				Stock = (int)reader["Stock"],
				Artist = (string)reader["Artist"],
				Description = (string)reader["Description"],
				Category = (string)reader["Category"]
			});
		}
		return paintings;
	}

	public Painting GetPaintingById(int id)
	{
		Painting painting = new Painting();
		string queryString = @"SELECT * FROM Painting where id = @id";
		using SqlConnection connection = new SqlConnection(connectionString);
		SqlCommand command = new SqlCommand(queryString, connection);
		command.Parameters.AddWithValue("@id", id);

		connection.Open();
		try
		{
			SqlDataReader reader = command.ExecuteReader();
			while (reader.Read())
			{
				painting.Id = (int)reader["ID"];
				painting.Title = (string)reader["Title"];
				painting.Price = (decimal)reader["Price"];
				painting.Stock = (int)reader["Stock"];
				painting.Artist = (string)reader["Artist"];
				painting.Description = (string)reader["Description"];
				painting.Category = (string)reader["Category"];
			}
		}
		catch (Exception)
		{
			throw new Exception($"ERROR occurred while getting painting with {id} a painting");
		}
		return painting;
	}



}