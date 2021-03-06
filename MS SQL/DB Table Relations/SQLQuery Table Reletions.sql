
--problem 3
CREATE TABLE Students
(
StudentID INT PRIMARY KEY IDENTITY,
[Name] NVARCHAR(50) NOT NULL
)

CREATE TABLE Exams 
(
ExamId INT PRIMARY KEY ,
[Name] NVARCHAR(50) NOT NULL
)

CREATE TABLE StudentsExams
(
StudentID INT FOREIGN KEY REFERENCES  Students(StudentID),
ExamID INT FOREIGN KEY REFERENCES Exams(ExamID),
CONSTRAINT PK_CompositeStudentIdExamId
PRIMARY KEY (StudentID, ExamID)
)

INSERT INTO Students([Name]) VALUES
('Mila'), ('Toni'), ('Ron');

INSERT INTO Exams(ExamId,[Name]) VALUES
(101, 'SpringMVC'), (102, 'Neo4j'), (103, 'Oracle 11g');

INSERT INTO StudentsExams VALUES
(1, 101), (1, 102), (2, 101), (3, 103), (2, 102), (2, 103);

-- problem 1
CREATE TABLE Passports
(
	PassportID INT PRIMARY KEY ,
	PassportNumber VARCHAR(50) NOT NULL
);

CREATE TABLE Persons
(
	PersonID INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(30) NOT NULL,
	Salary DECIMAL(15, 2) NOT NULL,
	PassportID INT FOREIGN KEY REFERENCES Passports(PassportID) NOT NULL
);

INSERT INTO Passports([PassportID], [PassportNumber]) VALUES
(101, 'N34FG21B'),
(102, 'K65LO4R7'),
(103, 'ZE657QP2');

INSERT INTO Persons([FirstName], [Salary], [PassportID]) VALUES
('Roberto', 43300.00, 102),
('Tom', 56100.00, 103),
('Yana', 60200.00, 101);

SELECT *
  FROM Persons AS p
  JOIN Passports As pass ON pass.PassportID = p.PassportID;

  --problem 2 

  CREATE TABLE Manufacturers
(
	ManufacturerID INT PRIMARY KEY IDENTITY ,
	[Name] NVARCHAR(50) NOT NULL,
	EstablishedOn DATE NOT NULL
);

CREATE TABLE Models
(
	ModelID INT PRIMARY KEY NOT NULL,
	[Name] NVARCHAR(50) NOT NULL,
	ManufacturerID INT FOREIGN KEY REFERENCES Manufacturers(ManufacturerID) NOT NULL
);

INSERT INTO Manufacturers([Name], EstablishedOn) VALUES
('BMW', '07/03/1916'),
('Tesla', '01/01/2003'),
('Lada', '01/05/1966');

INSERT INTO Models(MoDelID, [Name], ManufacturerID) VALUES
(101, 'X1', 1),
(102, 'i6', 1),
(103, 'Model S', 2),
(104, 'Model X', 2),
(105, 'Model 3', 2),
(106, 'Nova', 3);

-- problem 4

CREATE TABLE Teachers
(
	TeacherID INT PRIMARY KEY IDENTITY(101, 1),
	[Name] NVARCHAR(30) NOT NULL,
	ManagerID INT FOREIGN KEY REFERENCES Teachers(TeacherID)
);

INSERT INTO Teachers([Name], ManagerID) VALUES
('John', NULL),
('Maya', 106),
('Silvia', 106),
('Ted', 105),
('Mark', 101),
('Greta', 101);


-- problem 5

CREATE TABLE Cities
(
	CityID INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(50) NOT NULL
);

CREATE TABLE Customers
(
	CustomerID INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(50) NOT NULL,
	Birthday DATE NOT NULL,
	CityID INT FOREIGN KEY REFERENCES Cities(CityID) NOT NULL
);

CREATE TABLE Orders
(
	OrderID INT PRIMARY KEY IDENTITY,
	CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID) NOT NULL
);

CREATE TABLE ItemTypes
(
	ItemTypeID INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(50) NOT NULL
);

CREATE TABLE Items
(
	ItemID INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(50) NOT NULL,
	ItemTypeID INT FOREIGN KEY REFERENCES ItemTypes(ItemTypeID) NOT NULL,
);

CREATE TABLE OrderItems
(
	OrderID INT FOREIGN KEY REFERENCES Orders(OrderID) NOT NULL,
	ItemID INT FOREIGN KEY REFERENCES Items(ItemID) NOT NULL,
	CONSTRAINT PK_OrderItems PRIMARY KEY (OrderID, ItemID)
);

--problem 6

CREATE TABLE Majors
(
	MajorID INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(50) NOT NULL
);

CREATE TABLE Students
(
	StudentID INT PRIMARY KEY IDENTITY ,
	StudentNumber NVARCHAR(50) NOT NULL,
	StudentName NVARCHAR(50) NOT NULL,
	MajorID INT FOREIGN KEY REFERENCES Majors(MajorID) NOT NULL
);

CREATE TABLE Payments
(
	PaymentID INT PRIMARY KEY IDENTITY,
	PaymentDate DATE NOT NULL,
	PaymentAmount DECIMAL(15, 2) NOT NULL,
	StudentID INT FOREIGN KEY REFERENCES Students(StudentID) NOT NULL
);

CREATE TABLE Subjects
(
	SubjectID INT PRIMARY KEY IDENTITY ,
	SubjectName NVARCHAR(50) NOT NULL
);

CREATE TABLE Agenda
(
	StudentID INT FOREIGN KEY REFERENCES Students(StudentID) NOT NULL,
	SubjectID INT FOREIGN KEY REFERENCES Subjects(SubjectID) NOT NULL,
	CONSTRAINT PK_Agenda PRIMARY KEY (StudentID, SubjectID)
);