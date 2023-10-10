namespace Maletkunst.DAL.Models;

public class Customer : Person
{
	public int Discount { get; set; }

	public Customer(int id, string firstName, string lastName, string address, int postalCode, string city, string phone, string email, int discount) : base(id, firstName, lastName, address, postalCode, city, phone, email)
	{
		Id = id;
		FirstName = firstName;
		LastName = lastName;
		Address = address;
		PostalCode = postalCode;
		City = city;
		Phone = phone;
		Email = email;
		Discount = discount;
	}

	public Customer() { }
}
