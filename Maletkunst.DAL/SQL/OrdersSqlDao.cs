using Maletkunst.DAL.Interfaces;
using Maletkunst.DAL.Models;
using System.Data.SqlClient;

namespace Maletkunst.DAL.SQL;

public class OrdersSqlDao : IOrdersDataAccess
{
    private const string connectionString = @"Data Source=hildur.ucn.dk;
                                            Initial Catalog=DMA-CSD-V221_10434660;
                                            User ID=DMA-CSD-V221_10434660;
                                            Password=Password1!;";

    private const string queryString_InsertOrder = @"INSERT INTO [Order] (Status, Total, Customer_Id)
                                                   VALUES(@Status, @Total, @Customer_Id);
                                                   SELECT CAST(scope_identity() AS int)";

    private const string queryString_InsertOrderLine = @"INSERT INTO [OrderLine]
                                                       VALUES(@Quantity, @SubTotal, @OrderNumber, @Painting_Id)";

    private const string queryString_SelectPaintingsWithStock = @"SELECT * FROM Painting
                                                                WHERE Id = @Painting_Id AND Stock > 0";

    private const string queryString_UpdatePaintingsStock = @"UPDATE Painting SET Stock = Stock - 1
                                                            WHERE Id = @Painting_Id";

    private const string queryString_InsertPerson = @"INSERT INTO Person (fName, lName, phone, email, personType)
                                                    VALUES (@fName, @lName, @phone, @email, @personType);
                                                    SELECT CAST(scope_identity() AS int)";

    private const string queryString_InsertCustomer = @"INSERT INTO Customer (Customer_Id, Discount)
                                                      VALUES (@customerId, @discount)";

    private const string queryString_InsertAddress = @"INSERT INTO Address (address, personId, postalCode)
                                                     VALUES (@address, @personId, @postalCode)";

    private IPaintingsDataAccess _paintingSqlDataAccess;

    public OrdersSqlDao(IPaintingsDataAccess paintingSqlDataAccess)
    {
        _paintingSqlDataAccess = paintingSqlDataAccess;
    }
    public IEnumerable<Order> GetAllOrders()
    {
        string queryString = @"SELECT * FROM [Order]";
        using SqlConnection connection = new(connectionString);
        SqlCommand command = new(queryString, connection);

        connection.Open();

        try { return BuildListOfOrders(command); }

        catch (Exception ex) { throw new Exception("ERROR occurred while getting all orders", ex); }
    }

    private IEnumerable<Order> BuildListOfOrders(SqlCommand command)
    {
        SqlDataReader reader = command.ExecuteReader();
        ICollection<Order> orders = new List<Order>();
        while (reader.Read())
        {
            orders.Add(new Order()
            {
                OrderNumber = (int)reader["OrderNumber"],
                OrderDate = (DateTime)reader["OrderDate"],
                Status = (string)reader["Status"],
                Total = (decimal)reader["Total"],
                OrdersCustomer = GetCustomerByCustomerId((int)reader["Customer_Id"]),
                OrderLines = GetOrderLineByOrderNumber((int)reader["OrderNumber"])
            });
        }
        return orders;
    }

    private Customer GetCustomerByCustomerId(int customerId)
    {
        string queryString = @"SELECT c.Customer_Id, p.fName AS FirstName, p.lName AS LastName, a.address AS Address, a.postalCode AS PostalCode,
                           pc.city AS City, p.phone AS Phone, p.email AS Email, c.Discount
                           FROM Customer c
                           INNER JOIN Person p ON c.Customer_Id = p.PersonId
                           INNER JOIN Address a ON p.PersonId = a.personId
                           INNER JOIN PostalCode pc ON a.postalCode = pc.postalcode
                           WHERE c.Customer_Id = @Customer_Id";

        using SqlConnection connection = new(connectionString);
        SqlCommand command = new(queryString, connection);
        command.Parameters.AddWithValue("@Customer_Id", customerId);
        connection.Open();
        try
        {
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Customer customer = new()
                {
                    Id = (int)reader["Customer_Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    Address = (string)reader["Address"],
                    PostalCode = (int)reader["PostalCode"],
                    City = (string)reader["City"],
                    Phone = (string)reader["Phone"],
                    Email = (string)reader["Email"],
                    Discount = (int)reader["Discount"]
                };
                return customer;
            }
            else { throw new Exception($"Customer with ID '{customerId}' not found."); }
        }
        catch (Exception ex) { throw new Exception("ERROR occurred while getting the customer by customer ID", ex); }
    }

    public int CreateOrder(Order order)
    {
        // QUERIES DEFINITIONS FOR ORDER
        //string queryString_InsertOrder = @"insert into [Order] (Status, Total, Customer_Id) values(@Status, @Total, @Customer_Id); SELECT CAST(scope_identity() AS int)";
        //string queryString_InsertOrderLine = @"insert into [OrderLine] values(@Quantity, @SubTotal, @OrderNumber, @Painting_Id)";
        //string queryString_SelectPaintingsWithStock = @"SELECT * FROM Painting WHERE Id = @Painting_Id AND Stock > 0";
        //string queryString_UpdatePaintingsStock = @"UPDATE Painting SET Stock = Stock - 1 WHERE Id = @Painting_Id";

        // QUERIES DEFINITIONS FOR CUSTOMER
        //string queryString_InsertPerson = @"INSERT INTO Person (fName, lName, phone, email, personType) VALUES (@fName, @lName, @phone, @email, @personType); SELECT CAST(scope_identity() AS int)";
        //string queryString_InsertCustomer = @"INSERT INTO Customer (Customer_Id, Discount) VALUES (@customerId, @discount)";
        //string queryString_InsertAddress = @"INSERT INTO Address (address, personId, postalCode) VALUES (@address, @personId, @postalCode)";

        try
        {
            //// STARTS USING CONNECTION
            using SqlConnection connection = new(connectionString);
            connection.Open();

            // STARTS TRANSACTION WITH ISOLATION LEVEL REPEATABLE READ (LOCKS TUPLE)
            SqlTransaction transaction = connection.BeginTransaction(System.Data.IsolationLevel.RepeatableRead);

            // COMMANDS FOR ORDER CREATION
            SqlCommand commandOrder = new(queryString_InsertOrder, connection, transaction);
            SqlCommand commandOrderLine = new(queryString_InsertOrderLine, connection, transaction);
            SqlCommand commandSelectPaintingStock = new(queryString_SelectPaintingsWithStock, connection, transaction);
            SqlCommand commandUpdatePaintingsStock = new(queryString_UpdatePaintingsStock, connection, transaction);

            // COMMANDS FOR CUSTOMER CREATION
            SqlCommand commandPerson = new(queryString_InsertPerson, connection, transaction);
            SqlCommand commandCustomer = new(queryString_InsertCustomer, connection, transaction);
            SqlCommand commandAddress = new(queryString_InsertAddress, connection, transaction);

            // PARAMETERS FOR PERSON CREATION
            commandPerson.Parameters.AddWithValue("@fName", order.OrdersCustomer.FirstName);
            commandPerson.Parameters.AddWithValue("@lName", order.OrdersCustomer.LastName);
            commandPerson.Parameters.AddWithValue("@phone", order.OrdersCustomer.Phone);
            commandPerson.Parameters.AddWithValue("@email", order.OrdersCustomer.Email);
            commandPerson.Parameters.AddWithValue("@personType", "Customer");

            try
            {
                // LOOP FOR CHECKING STOCK OF ORDERS PAINTINGS
                foreach (OrderLine orderLine in order.OrderLines)
                {
                    // CLEAR PARAMETERS
                    commandSelectPaintingStock.Parameters.Clear();

                    // PARAMETERS FOR PAINTING STOCK CHECK
                    commandSelectPaintingStock.Parameters.AddWithValue("@Painting_Id", orderLine.Painting.Id);

                    // READ TUPLES RETURNED - IF NO TUPLES RETURNED RETURN 0 TO INDICATE THAT ORDER HAS NOT BEEN CREATED
                    using SqlDataReader reader = commandSelectPaintingStock.ExecuteReader();
                    if (!reader.Read())
                    {
                        // implisit rollback
                        return 0;
                    }
                }

                // EXECUTION OF PERSON CREATION WITH GENERATED IDENTITY KEY
                int NewGeneratedPersonId = (int)commandPerson.ExecuteScalar();

                // PARAMETERS FOR CUSTOMER CREATION
                commandCustomer.Parameters.AddWithValue("@customerId", NewGeneratedPersonId);
                commandCustomer.Parameters.AddWithValue("@discount", 0);

                // EXECUTION OF CUSTOMER CREATION
                commandCustomer.ExecuteNonQuery();

                // PARAMETERS FOR ADDRESS CREATION
                commandAddress.Parameters.AddWithValue("@address", order.OrdersCustomer.Address);
                commandAddress.Parameters.AddWithValue("@personId", NewGeneratedPersonId);
                commandAddress.Parameters.AddWithValue("@postalCode", order.OrdersCustomer.PostalCode);

                // EXECUTION OF ADDRESS CREATION
                commandAddress.ExecuteNonQuery();


                // PARAMETERS FOR ORDER CREATION
                commandOrder.Parameters.AddWithValue("@Status", order.Status);
                commandOrder.Parameters.AddWithValue("@Total", order.Total);
                commandOrder.Parameters.AddWithValue("@Customer_Id", NewGeneratedPersonId);

                // EXECUTION OF ORDER CREATION WITH GENERATED IDENTITY KEY
                int newGeneratedOrderNumber = (int)commandOrder.ExecuteScalar();

                // DELAY INSERTED FOR TESTING CONCURENCY
                Thread.Sleep(3000);

                // LOOP TO CREATE ORDERLINES
                foreach (OrderLine orderLine in order.OrderLines)
                {
                    // CLEAR PARAMETERS
                    commandOrderLine.Parameters.Clear();
                    commandUpdatePaintingsStock.Parameters.Clear();

                    // PARAMETERS FOR ORDERLINE CREATION
                    commandOrderLine.Parameters.AddWithValue("@Quantity", orderLine.Quantity);
                    commandOrderLine.Parameters.AddWithValue("@SubTotal", orderLine.SubTotal);
                    commandOrderLine.Parameters.AddWithValue("@OrderNumber", newGeneratedOrderNumber);
                    commandOrderLine.Parameters.AddWithValue("@Painting_Id", orderLine.Painting.Id);

                    // EXECUTION OF ORDERLINE CREATION
                    commandOrderLine.ExecuteNonQuery();

                    // PARAMETERS FOR PAINTING STOCK ADJUSTMENT
                    commandUpdatePaintingsStock.Parameters.AddWithValue("@Painting_Id", orderLine.Painting.Id);

                    // EXECUTION OF PAINTING STOCK ADJUSTMENT
                    commandUpdatePaintingsStock.ExecuteNonQuery();
                }

                // SAVE THE CHANGES IN THE DATABASE
                transaction.Commit();

                // RETURNS THE NEW GENERATED ORDER NUMBER
                return newGeneratedOrderNumber;
            }

            // EXCEPTION HANDLING AND ROLL BACK
            catch (Exception e)
            {
                try { transaction.Rollback(); }
                catch (Exception exx) { throw new Exception("ERROR occurred while rolling back", exx); }
                throw new Exception("ERROR occurred while creating an order", e);
            }
        }
        catch (Exception exxx)
        {
            throw new Exception("Error opening connection to database", exxx);
        }
    }

    public IEnumerable<OrderLine> GetOrderLineByOrderNumber(int orderNumber)
    {
        string queryString = @"SELECT * FROM OrderLine WHERE Order_Id = @OrderNumber";
        using SqlConnection connection = new(connectionString);
        SqlCommand command = new(queryString, connection);
        command.Parameters.AddWithValue("@OrderNumber", orderNumber);

        connection.Open();

        try { return BuildListOfOrderLines(command); }

        catch (Exception ex) { throw new Exception("ERROR occurred while getting all order lines", ex); }
    }


    private IEnumerable<OrderLine> BuildListOfOrderLines(SqlCommand command)
    {
        SqlDataReader reader = command.ExecuteReader();
        ICollection<OrderLine> orderLines = new List<OrderLine>();
        while (reader.Read())
        {
            orderLines.Add(new OrderLine()
            {
                OrderLineId = (int)reader["OrderLineId"],
                Quantity = (int)reader["Quantity"],
                SubTotal = (decimal)reader["SubTotal"],
                Painting = _paintingSqlDataAccess.GetPaintingById((int)reader["Painting_Id"])
            });
        }
        return orderLines;
    }
}
