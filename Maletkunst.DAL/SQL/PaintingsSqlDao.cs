using Maletkunst.DAL.Interfaces;
using Maletkunst.DAL.Models;
using System.Data.SqlClient;

namespace Maletkunst.DAL.SQL;

public class PaintingsSqlDao : IPaintingsDao
{
    private string connectionString = @"Data Source=hildur.ucn.dk; Initial Catalog=DMA-CSD-V221_10434660; User ID=DMA-CSD-V221_10434660; Password=Password1!;";

    public PaintingsSqlDao(String connectionString) { this.connectionString = connectionString; }

    public PaintingsSqlDao() { }

    public async Task<IEnumerable<Painting>> GetAllPaintingsAsync()
    {
        string queryString = @"SELECT * FROM Painting";
        using SqlConnection connection = new SqlConnection(connectionString);
        SqlCommand command = new SqlCommand(queryString, connection);

        await connection.OpenAsync();

        try { return await BuildListOfPaintingsAsync(command); }

        catch (Exception ex) { throw new Exception("ERROR occurred while getting all paintings", ex); }
    }

    private async Task<IEnumerable<Painting>> BuildListOfPaintingsAsync(SqlCommand command)
    {
        SqlDataReader reader = await command.ExecuteReaderAsync();
        List<Painting> paintings = new List<Painting>();
        while (await reader.ReadAsync())
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

    public int CreatePainting(Painting painting)
    {
        string queryString = @"INSERT INTO Painting values(@Title,@Price,@Stock,@Artist,@Description,@Category); SELECT CAST(scope_identity() AS int)";
        using SqlConnection connection = new SqlConnection(connectionString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand(queryString, connection, transaction);

        command.Parameters.AddWithValue("@Title", painting.Title);
        command.Parameters.AddWithValue("@Price", painting.Price);
        command.Parameters.AddWithValue("@Stock", painting.Stock);
        command.Parameters.AddWithValue("@Artist", painting.Artist);
        command.Parameters.AddWithValue("@Description", painting.Description);
        command.Parameters.AddWithValue("@Category", painting.Category);

        int NewGeneratedPaintingId = 0;
        try
        {
            NewGeneratedPaintingId = (int)command.ExecuteScalar();
            transaction.Commit();
        }

        catch (Exception e)
        {
            try
            {
                transaction.Rollback();
                throw new Exception("ERROR occurred while rolling back", e);
            }
            catch (Exception ex)
            {
                throw new Exception("ERROR occurred while creating a painting", ex);
            }
        }
        return NewGeneratedPaintingId;
    }

    public bool DeletePaintingById(int id)
    {
        string queryString = @"DELETE FROM Painting WHERE id = @id";
        using SqlConnection connection = new SqlConnection(connectionString);
        SqlCommand command = new SqlCommand(queryString, connection);
        command.Parameters.AddWithValue("@id", id);
        connection.Open();

        try
        {
            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected <= 0)
            {
                return false;
            }

            return true;
        }
        catch (SqlException ex)
        {
            //HACK: we're using ex no as id
            if (ex.Number == 547) // Foreign key constraint violation number
            {
                //Run it in swagger and try the delete method
                throw new Exception("Painting could not be deleted as it is associated with an orderline in an order.", ex);
            }
            else
            {
                throw new Exception("An error occurred while deleting the painting.", ex);
            }
        }
    }


    public bool UpdatePainting(Painting painting)
    {
        string queryString = @"UPDATE Painting set Title =@title, Price = @price, Stock = @stock, Artist = @artist, Description = @description, Category = @category WHERE id = @id";
        using SqlConnection connection = new SqlConnection(connectionString);
        SqlCommand command = new SqlCommand(queryString, connection);
        command.Parameters.AddWithValue("@title", painting.Title);
        command.Parameters.AddWithValue("@price", painting.Price);
        command.Parameters.AddWithValue("@stock", painting.Stock);
        command.Parameters.AddWithValue("@artist", painting.Artist);
        command.Parameters.AddWithValue("@description", painting.Description);
        command.Parameters.AddWithValue("@category", painting.Category);
        command.Parameters.AddWithValue("@id", painting.Id);
        connection.Open();

        try
        {
            command.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("ERROR occurred while updating painting", ex);
        }
    }

    public Painting GetPaintingbyId(int id)
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
