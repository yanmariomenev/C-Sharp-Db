USE SoftUni

SELECT TOP(5) e.EmployeeID, e.JobTitle , a.AddressID, a.AddressText
FROM Employees e
JOIN Addresses a ON a.AddressID = e.AddressID
ORDER BY a.AddressID

SELECT TOP(50) e.FirstName, e.LastName,t.[Name] , a.AddressText
FROM Employees e
JOIN Addresses a ON a.AddressID = e.AddressID
JOIN Towns t ON t.TownID = a.TownID
ORDER BY e.FirstName, e.LastName

SELECT e.EmployeeID, e.FirstName , e.LastName , d.[Name]
FROM Employees e
JOIN Departments d ON d.DepartmentID = e.DepartmentID
WHERE d.[Name] = 'Sales'
ORDER BY E.EmployeeID

SELECT TOP(5) e.EmployeeID, e.FirstName, e.Salary, d.[Name]
FROM Employees e
JOIN Departments d ON d.DepartmentID = e.DepartmentID
WHERE e.Salary > 15000
ORDER BY d.DepartmentID

SELECT TOP(3) e.EmployeeID, e.FirstName
    FROM Employees e
   LEFT JOIN EmployeesProjects ep ON ep.EmployeeID = e.EmployeeID
   WHERE ep.ProjectID IS NULL
ORDER BY e.EmployeeID;

SELECT e.FirstName, e.LastName, e.HireDate, d.Name FROM Employees e 
JOIN Departments d ON d.DepartmentID = e.DepartmentID
WHERE e.HireDate > '1.1.1999' AND d.Name IN ('Sales', 'Finance')
ORDER BY e.HireDate

SELECT TOP(5) e.EmployeeID, e.FirstName, p.Name
FROM Employees e
JOIN EmployeesProjects ep ON ep.EmployeeID = e.EmployeeID
JOIN Projects p ON p.ProjectID = ep.ProjectID
WHERE p.StartDate > '08.13.2002' AND P.EndDate IS NULL
ORDER BY e.EmployeeID

SELECT e.EmployeeID , e.FirstName,
IIF(p.StartDate >= '01.01.2005', NULL, p.Name)
FROM Employees e
JOIN EmployeesProjects ep ON ep.EmployeeID = e.EmployeeID
JOIN Projects p ON p.ProjectID = ep.ProjectID
WHERE ep.EmployeeID = 24


-- TO FINISH IT
SELECT e.EmployeeID, CONCAT(e.FirstName, ' ', e.LastName) EmployeeName,
CONCAT(mng.FirstName, ' ', mng.LastName) ManagerName,
d.Name DeparmentName
FROM Employees e
JOIN Employees mng ON mng.EmployeeID = e.ManagerID
JOIN Departments d ON d.DepartmentID = e.DepartmentID
ORDER BY e.EmployeeID

USE [Geography]


SELECT c.CountryCode, m.MountainRange, p.PeakName, p.Elevation
FROM Countries c
JOIN MountainsCountries mc ON mc.CountryCode = c.CountryCode
JOIN Mountains m ON m.Id = mc.MountainId
JOIN Peaks p ON p.MountainId = m.Id
WHERE c.CountryCode = 'BG' AND p.Elevation > 2835
ORDER BY p.Elevation DESC 


SELECT TOP(5) c.CountryName, r.RiverName 
FROM Countries c
LEFT JOIN CountriesRivers cr ON cr.CountryCode = c.CountryCode
LEFT JOIN Rivers r ON r.Id = cr.RiverId
WHERE c.ContinentCode = 'AF'
ORDER BY c.CountryName

