use [DMA-CSD-V221_10434660]


CREATE TABLE [Order](
	OrderNumber int identity(1,1) NOT NULL,
	OrderDate datetime NOT NULL DEFAULT GETDATE(),
	Status nvarchar(20),
	Total decimal(38,2),
	Customer_Id int,
	PRIMARY KEY(OrderNumber),
	FOREIGN KEY(Customer_Id) REFERENCES Customer(Customer_Id),
	)

	CREATE TABLE OrderLine(
	OrderLineId int identity(1,1),
	Quantity int,
	SubTotal decimal(38,2),
	Order_Id int,
	Painting_Id int,
	PRIMARY KEY(OrderLineId),
	FOREIGN KEY(Painting_Id) REFERENCES Painting(Id),
	FOREIGN KEY(Order_Id) REFERENCES [Order](OrderNumber)
)