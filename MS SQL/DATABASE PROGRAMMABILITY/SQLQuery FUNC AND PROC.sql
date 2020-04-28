-- problem 5/6
USE SoftUni
GO

CREATE FUNCTION ufn_GetSalaryLevel(@salary DECIMAL(18,4))
RETURNS VARCHAR(7)
AS
BEGIN
DECLARE @salaryLevel VARCHAR(7) = CASE
WHEN @salary < 30000 THEN 'Low'
WHEN @salary BETWEEN 30000 AND 50000 THEN 'Average'
ELSE 'High'
END
RETURN @salaryLevel
END

CREATE FUNCTION ufn_GetSalaryLevel(@salary DECIMAL(18,4))
RETURNS VARCHAR(7)
AS
BEGIN

DECLARE @salaryLevel VARCHAR(7)
IF(@salary < 30000)
BEGIN
SET @salaryLevel = 'Low'
END
ELSE IF(@Salary <= 50000)
BEGIN
SET @salaryLevel = 'Average'
END
ELSE
BEGIN
SET @salaryLevel = 'High'
END
RETURN @salaryLevel
END

--
GO

CREATE PROC usp_EmployeesBySalaryLevel(@salaryLevel VARCHAR(7))
AS
BEGIN

SELECT e.FirstName,e.LastName
FROM Employees e
WHERE dbo.ufn_GetSalaryLevel(e.Salary) = @salaryLevel

END

EXEC dbo.usp_EmployeesBySalaryLevel 'High'

-- problem 7

CREATE FUNCTION ufn_IsWordComprised(@setOfLetters VARCHAR(MAX), @word VARCHAR(MAX))
RETURNS BIT
AS 
BEGIN
DECLARE @counter int
DECLARE @currentLetter CHAR

WHILE(@counter <= LEN(@word))
BEGIN
SET @currentLetter = SUBSTRING(@word, @counter , 1)

DECLARE @charIndex INT = CHARINDEX(@currentLetter , @setOfLetters)

IF(@charIndex <= 0)
BEGIN
RETURN 0;
END

SET @counter +=1
END

RETURN 1
END

--PROBLEM 8

CREATE PROC usp_DeleteEmployeesFromDepartment(@departmentId INT)
AS
BEGIN

DELETE FROM EmployeesProjects
WHERE EmployeeID IN 
(
SELECT EmployeeID FROM Employees
WHERE DepartmentID = @departmentId
)

UPDATE Employees
SET ManagerID = NULL
WHERE ManagerID IN 
(
SELECT EmployeeID FROM Employees
WHERE DepartmentID = @departmentId
)

DELETE FROM Employees
WHERE DepartmentID = @departmentId

DELETE FROM Departments
WHERE DepartmentID = @departmentId

SELECT COUNT(*) FROM Employees
WHERE DepartmentID = @departmentId

END
