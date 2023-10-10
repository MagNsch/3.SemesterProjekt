using System.Data.SqlClient;
using Maletkunst.DAL.Interfaces;
using Maletkunst.DAL.Models;
using Maletkunst.DAL.SQL;

namespace Maletkunst.Tests
{
    [TestFixture]
    public class PaintingsSqlDaoTests
    {
        #region Fields
        private string _developer;
        private IPaintingsDao _paintingWinAppSqlDao;
        private List<int> paintingIds;

        // ConnectionString for the test database
        private string _connectionString = "server=hildur.ucn.dk; Database=DMA-CSD-V221_10434668;User Id=DMA-CSD-V221_10434668; Password=Password1!;";

        #endregion Fields

        [SetUp]
        public void Setup()
        {
            // Guid
            string developerGuid = Guid.NewGuid().ToString("N");

            // Write your name
            _developer = developerGuid + "Thomas";

            // Access to database
            _paintingWinAppSqlDao = new PaintingsSqlDao(
               _connectionString            
                );

            // Set up list for painting ids
            paintingIds = new List<int>();

            // Setup test data
            insertTestData();
        }

        [Category("Database")]
        [Description("Integration test for creating a painting in the Database")]
        [Test]
        public void CreatePainting_CreationOfPaintingIsSuccesful_ReturnsPaintingID()
        {
            // Arrange
            Painting painting = new Painting
            {
                Title = _developer + "Starry Night",
                Price = 5000,
                Stock = 3,
                Artist = "Van Gogh",
                Description = "A masterpiece",
                Category = "Post-Impressionism"
            };

            // Act
            int actualID = _paintingWinAppSqlDao.CreatePainting(painting);

            // Assert
            int expectedID = GetPaintingIdById(actualID);
            string errorMessage = $"Test Failed: actualID should equal to expectedID: actualID: {actualID}, expectedID{expectedID}";
            Assert.That(actualID, Is.EqualTo(expectedID), errorMessage);
        }


        [Category("Database")]
        [Description("Integration test for getting all paintings from the Database")]
        [Test]
        public async Task GetAllPaintings_GetsAllPaintings_ReturnsListOfPaintings()
        {

			//Arrange

			// Act
			List<Painting> paintings = (await _paintingWinAppSqlDao.GetAllPaintingsAsync()).ToList();

			// Assert
			int expectedCount = paintingIds.Count;
            Assert.That(paintings.Count, Is.EqualTo(expectedCount), $"Test Failed: expected painting count {expectedCount} but got {paintings.Count}");
        }


        [Category("Database")]
        [Description("Integration test for deleting a painting when id is valid in Database")]
        [Test]
        public void DeletePainting_WhenPaintingIDIsValid_ReturnsTrue()
        {
            // Arrange
            int paintingIdToDelete = paintingIds[0];

            // Act
            bool expectedTrue = _paintingWinAppSqlDao.DeletePaintingById(paintingIdToDelete);

            // Assert
            string errorMessage = $"Test Failed: Should have returned true: Expected: {true}, Actual: {expectedTrue}";
            Assert.IsTrue(expectedTrue, errorMessage);
        }

        [Category("Database")]
        [Description("Integration test for deleting a painting when id is invalid in Database")]
        [TestCase(-1)]
        [TestCase(0)]
        public void DeletePainting_WhenPaintingIDIsInvalid_ReturnsFalse(int paintingIdToDelete)
        {
            // Act
            bool expectedFalse = _paintingWinAppSqlDao.DeletePaintingById(paintingIdToDelete);

            // Assert
            string errorMessage = $"Test Failed: Should not delete anything and return false: Expected: {false}, Actual: {expectedFalse}";
            Assert.IsFalse(expectedFalse, errorMessage);
        }


        // Tilføj flere tests. Også for invalide inputs

        [TearDown]
        public void TearDown()
        {
            int rows = 0;
            string delete = $"DELETE FROM Painting WHERE Title LIKE @title";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(delete, connection);

                command.Parameters.AddWithValue("@title", _developer + "%");

                rows = command.ExecuteNonQuery();
                Console.WriteLine($"TearDown method has been activated: {rows} rows has been deleted from Painting table");
            }
        }

        #region helper_methods
        public void insertTestData()
        {
            List<Painting> testData = CreatePaintingTestData();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string insertTestData = "INSERT INTO Painting (Title, Price, Stock, Artist, Description, Category) OUTPUT INSERTED.Id " +
                "VALUES (@Title, @Price, @Stock, @Artist, @Description, @Category)";
                SqlCommand command = new SqlCommand(@insertTestData, connection);
                connection.Open();

                try
                {
                    foreach (var painting in testData)
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@Title", painting.Title);
                        command.Parameters.AddWithValue("@Price", painting.Price);
                        command.Parameters.AddWithValue("@Stock", painting.Stock);
                        command.Parameters.AddWithValue("@Artist", painting.Artist);
                        command.Parameters.AddWithValue("@Description", painting.Description);
                        command.Parameters.AddWithValue("@Category", painting.Category);
                        int id = (int)command.ExecuteScalar();
                        paintingIds.Add(id);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Could not insert TestData {ex} ");
                }
            }
        }

        public List<Painting> CreatePaintingTestData()
        {
            List<Painting> testData = new List<Painting>
        {
            new Painting {Title = _developer + "Starry Night", Price = 5000, Stock = 1, Artist = "Van Gogh", Description = "A masterpiece", Category = "Post-Impressionism"},
            new Painting {Title = _developer + "The Scream", Price = 4500, Stock = 1, Artist = "Edvard Munch", Description = "An iconic painting", Category = "Expressionism"},
            new Painting {Title = _developer + "The Persistence of Memory", Price = 4000, Stock = 0, Artist = "Salvador Dalí", Description = "A surreal painting", Category = "Surrealism"}

            };
            return testData;
        }

        public int GetPaintingIdById(int id)
        {
            string GetPaintingIDById = $"SELECT Id FROM Painting where Id = @id";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                int paintingId = 0;
                SqlCommand command = new SqlCommand(GetPaintingIDById, connection);
                connection.Open();
                try
                {
                    command.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        paintingId = (int)reader["Id"];
                    }

                    return paintingId;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Could not select Painting by ID {ex}");
                }
            }
        }
        #endregion
    }
}