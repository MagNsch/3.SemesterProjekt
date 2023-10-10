USE [DMA-CSD-V221_10434660]

CREATE TABLE Person(
	PersonId int IDENTITY(1,1) NOT NULL,
	fName nvarchar(30),
	lName nvarchar(30),
	phone nvarchar(8),
	email nvarchar(50),
	personType nvarchar(20),
	PRIMARY KEY (PersonId)
	)

CREATE TABLE Customer(
	Customer_Id int NOT NULL,
	Discount int,
	PRIMARY KEY (Customer_Id),
	FOREIGN KEY (Customer_Id) REFERENCES Person (PersonId)
	)

CREATE TABLE PostalCode(
	postalcode int NOT NULL,
	city nvarchar(60),
	PRIMARY KEY (postalcode)
	)

CREATE TABLE Address(
	Address_Id int IDENTITY(1,1) NOT NULL,
	address nvarchar(60),
	personId int,
	postalCode int,
	PRIMARY KEY (Address_Id),
	FOREIGN KEY (personId) REFERENCES Person (PersonId),
	FOREIGN KEY (postalCode) REFERENCES PostalCode (postalCode)
	)