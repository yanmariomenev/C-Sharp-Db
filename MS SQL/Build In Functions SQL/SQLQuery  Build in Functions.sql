USE SoftUni
-- PORBLEM 1
SELECT FirstName, LastName FROM Employees
WHERE FirstName LIKE 'SA%'

-- PROBLEM 2
SELECT FirstName, LastName FROM Employees
WHERE LastName LIKE '%ei%'

--PROBLEM 3

SELECT FirstName FROM Employees
WHERE DepartmentID IN (3, 10) 
AND CAST(DATEPART(YEAR, HireDate)AS INT) BETWEEN 1995 AND 2005

--PROBLEM 4

SELECT FirstName, LastName FROM Employees
WHERE JobTitle NOT LIKE '%engineer%'

--PROBLEM 5

SELECT [Name] FROM Towns
WHERE LEN([Name]) BETWEEN 5 AND 6
ORDER BY [Name]

-- PROBLEM 6
SELECT TownID,[Name] FROM Towns
WHERE [Name] LIKE '[MKBE]%'
ORDER BY [Name]

--PROBLEM 7

SELECT TownID,[Name] FROM Towns
WHERE [Name] NOT LIKE '[RBD]%'
ORDER BY [Name]

--PROBLEM 8

CREATE VIEW V_EmployeesHiredAfter2000 AS
SELECT FirstName, LastName FROM Employees
WHERE DATEPART(YEAR,HireDate) > 2000

--PROBLEM 9

SELECT FirstName , LastName FROM Employees
WHERE LEN(LastName) = 5

-- PROBLEM 10 AND 11
SELECT * FROM
(SELECT EmployeeID, FirstName, LastName, Salary,
DENSE_RANK() OVER (PARTITION by Salary order by EmployeeID) AS [Ranks]
    FROM Employees
   WHERE Salary BETWEEN 10000 AND 50000)
--ORDER BY Salary DESC
AS Rankings
WHERE Rankings.[Ranks] = 2
ORDER BY Rankings.Salary DESC;

-- PROBLEM 12

USE [Geography]

SELECT [CountryName], IsoCode FROM Countries
WHERE CountryName LIKE '%a%a%a%'
ORDER BY IsoCode

--PROBLEM 13

SELECT p.PeakName, r.RiverName, LOWER(CONCAT(LEFT(p.PeakName, LEN(p.PeakName)- 1), r.RiverName)) AS Mix
FROM Peaks AS p, Rivers AS r
WHERE RIGHT(P.PeakName, 1) = LEFT(r.RiverName, 1)
ORDER BY [Mix]

SELECT p.PeakName, r.RiverName, LOWER(CONCAT(LEFT(p.PeakName, LEN(p.PeakName)- 1), r.RiverName)) AS Mix
FROM Peaks AS p
INNER JOIN Rivers AS r ON RIGHT(p.PeakName,1) = LEFT(r.RiverName,1)
ORDER BY [Mix]

--

USE Diablo

SELECT [Name] AS [Game],
CASE
WHEN DATEPART(HOUR,[Start]) BETWEEN 0 AND 11 THEN 'Morning'
WHEN DATEPART(HOUR,[Start]) BETWEEN 12 AND 17 THEN 'Afternoon'
ELSE 'Evening'
END AS [Part of the Day],

CASE
WHEN Duration <= 3 THEN 'Extra Short'
WHEN Duration BETWEEN 4 AND 6 THEN 'Short'
WHEN Duration > 6 THEN 'Long'
WHEN Duration IS NULL THEN 'Extra Long'
END AS [Duration]
FROM Games
ORDER BY [Game], [Duration], [Part of the Day]

--PROBLEM 16

SELECT [UserName], [IpAddress] FROM Users
WHERE IpAddress LIKE '___.1_%._%.___'
ORDER BY Username
