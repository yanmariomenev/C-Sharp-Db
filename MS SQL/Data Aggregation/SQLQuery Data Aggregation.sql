USE Gringotts
SELECT COUNT(Id) AS [Count] FROM WizzardDeposits

SELECT MAX(MagicWandSize) AS [LongestMagicWand]
FROM WizzardDeposits

SELECT DepositGroup, MAX(MagicWandSize) AS [LongestMagicWand]
FROM WizzardDeposits
GROUP BY DepositGroup

SELECT TOP(2) DepositGroup
FROM WizzardDeposits
GROUP BY DepositGroup
ORDER BY  AVG(MagicWandSize)

SELECT DepositGroup,SUM(DepositAmount) AS TotalSum FROM WizzardDeposits
WHERE MagicWandCreator = 'Ollivander Family'
GROUP BY DepositGroup
HAVING SUM(DepositAmount) < 150000
ORDER BY TotalSum DESC

SELECT DepositGroup, MagicWandCreator, MIN(DepositCharge) AS [MinDepositCharge]
FROM WizzardDeposits
GROUP BY DepositGroup , MagicWandCreator
ORDER BY MagicWandCreator, DepositGroup

SELECT DepositGroup, SUM(DepositAmount) FROM WizzardDeposits
WHERE MagicWandCreator = 'Ollivander Family'
GROUP BY DepositGroup

SELECT [Ages].AgeGroup, COUNT(Ages.AgeGroup)
    FROM
(
  SELECT 
    CASE
  		WHEN Age BETWEEN 0 AND 10 THEN '[0-10]'
  		WHEN Age BETWEEN 11 AND 20 THEN '[11-20]'
  		WHEN Age BETWEEN 21 AND 30 THEN '[21-30]'
  		WHEN Age BETWEEN 31 AND 40 THEN '[31-40]'
  		WHEN Age BETWEEN 41 AND 50 THEN '[41-50]'
  		WHEN Age BETWEEN 51 AND 60 THEN '[51-60]'
  	ELSE '[61+]'
  END AS [AgeGroup]
    FROM WizzardDeposits) AS [Ages]
GROUP BY Ages.AgeGroup;

SELECT LEFT(FirstName, 1) AS FirstLetter
    FROM WizzardDeposits
   WHERE DepositGroup =  'Troll Chest'
GROUP BY LEFT(FirstName, 1)
ORDER BY FirstLetter;

SELECT SUM([Difference]) AS SumDifference FROM (SELECT FirstName AS [Host Wizard], DepositAmount AS [Host Wizard Deposit],
LEAD(FirstName) OVER (ORDER BY Id) AS [Guest Wizard],
LEAD(DepositAmount) OVER (ORDER BY Id) AS [Guest Wizard Deposit],
DepositAmount - LEAD(DepositAmount) OVER (ORDER BY Id) AS [Difference]
FROM WizzardDeposits) AS DiffTable

SELECT * FROM WizzardDeposits

SELECT * INTO NewEmployeesTable FROM Employees 
WHERE Salary > 30000

DELETE FROM NewEmployeesTable 
WHERE ManagerID = 42

UPDATE NewEmployeesTable
SET Salary += 5000
WHERE DepartmentID = 1

SELECT DepartmentId, AVG(Salary) AS [AverageSalary]
FROM NewEmployeesTable
GROUP BY DepartmentId

SELECT DepartmentID, SUM(Salary)
    FROM Employees
GROUP BY DepartmentID
ORDER BY DepartmentID;


 SELECT DepartmentID, MIN(Salary)
    FROM Employees
   WHERE DepartmentID IN(2, 5, 7) AND HireDate > '01/01/2000'
GROUP BY DepartmentID;

SELECT DepartmentID, MAX(Salary) AS MaxSalary
    FROM Employees
GROUP BY DepartmentID
  HAVING MAX(Salary) NOT BETWEEN 30000 AND 70000

  SELECT COUNT(*) AS [Count]
  FROM Employees
 WHERE ManagerID IS NULL