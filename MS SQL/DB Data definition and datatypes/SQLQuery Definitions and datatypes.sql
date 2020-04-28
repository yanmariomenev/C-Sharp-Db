CREATE DATABASE Minions

GO 

USE Minions

GO

CREATE TABLE Minions (
Id INT NOT NULL,
[Name] NVARCHAR(50) NOT NULL,
Age INT NOT NULL,
)

CREATE TABLE Towns(
Id INT NOT NULL,
[Name] NVARCHAR(50) NOT NULL,
)

ALTER TABLE Minions
ADD CONSTRAINT PK_Id
PRIMARY KEY(Id)

ALTER TABLE Towns
ADD CONSTRAINT PK_TownId
PRIMARY KEY(Id)

ALTER TABLE Minions
ADD TownId INT 

ALTER TABLE Minions
ADD CONSTRAINT FK_MinionTownId
FOREIGN KEY (TownId) REFERENCES Towns(Id)

GO

INSERT INTO Towns(Id,[Name]) VALUES 
(1, 'Sofia'),
(2,'Plovdiv'),
(3, 'Varna')

INSERT INTO Minions (Id,[Name], Age, TownId) VALUES
(1, 'Kevin', 22, 1),
(2, 'Bob', 15, 3),
(3, 'Steward', NULL, 2)

-- Changes from not allowing null to allowing nulls.
ALTER TABLE Minions
ALTER COLUMN Age INT NULL
