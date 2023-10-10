namespace Maletkunst.DAL.Models;

public class Person
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
	public int PostalCode { get; set; }
	public string City { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }

	public Person() { }

	public Person(int id, string firstName, string lastName, string address, int postalCode, string city, string phone, string email) : this (firstName, lastName, address, postalCode, city, phone, email)
	{
		Id = id;
	}

	public Person(string firstName, string lastName, string address, int postalCode, string city, string phone, string email)
	{
		FirstName = firstName;
		LastName = lastName;
		Address = address;
		PostalCode = postalCode;
		City = city;
		Phone = phone;
		Email = email;
	}
}