INSERT INTO Files([Name], Size , ParentId, CommitId)
VALUES
(
'Trade.idk',
2598.0,
1,
1
),
(
'menu.net',
9238.31,
2,
2
),
(
'Administrate.soshy',
1246.93,
3,
3
),
(
'Controller.php',
7353.15,
4,
4
),
(
'Find.java',
9957.86,
5,
5
),
(
'Controller.json',
14034.87,
3,
6
),
(
'Operate.xix',
7662.92,
7,
7
);


INSERT INTO Issues (Title,IssueStatus,RepositoryId,AssigneeId)
VALUES
(
'Critical Problem with HomeController.cs file',
'open',
1,
4
),
(
'Typo fix in Judge.html',
'open',
4,
3
),
(
'Implement documentation for UsersService.cs',
'closed',
8,
2
),
(
'Unreachable code in Index.cs',
'open',
9,
8
);

--UPDATE
UPDATE Issues
SET IssueStatus = 'closed'
WHERE AssigneeId = (SELECT TOP(1) Id FROM Users
WHERE Id = 6)

--delete
DELETE FROM RepositoriesContributors
WHERE RepositoriesContributors.RepositoryId IN (SELECT r.Id FROM Repositories r
WHERE r.Name = 'Softuni-Teamwork');

DELETE FROM Issues
WHERE Issues.RepositoryId IN (SELECT r.Id FROM Repositories r
WHERE r.Name = 'Softuni-Teamwork')

--COMMITS
SELECT Id,[Message],RepositoryId,ContributorId  FROM Commits
ORDER BY Id, [Message], RepositoryId, ContributorId

--HEAVY HTML
SELECT Id,[Name],Size FROM Files
WHERE Size > 1000 AND [Name] LIKE '%html%'
ORDER BY Size DESC, Id, [Name]

--Issues and Users
SELECT i.Id, u.Username + ' : ' + i.Title AS [IssueAssignee] FROM Issues i
JOIN Users u ON u.Id = i.AssigneeId
ORDER BY i.Id DESC, [IssueAssignee]

--NON DIRECTORY FILES
SELECT f2.Id,f2.Name, CONCAT(f2.Size, 'KB') AS Size FROM Files f
RIGHT JOIN Files f2 ON f.ParentId = f2.Id
WHERE f.ParentId is NULL
ORDER BY f2.Id, f2.Name , Size DESC

--Most Contributed Repositories
SELECT TOP(5) r.Id,r.Name,COUNT(c.Id)AS Commits  FROM Commits c
JOIN Repositories r ON r.Id = c.RepositoryId
JOIN RepositoriesContributors rc ON rc.RepositoryId = r.Id
GROUP BY r.Id , r.Name
ORDER BY Commits DESC, r.Id, r.Name

--USER FILES
SELECT u.Username, AVG(f.Size) AS Size FROM Users u
JOIN Commits c ON c.ContributorId = u.Id
JOIN Files f ON f.CommitId = c.Id
GROUP BY u.Username
ORDER BY  Size DESC, u.Username

-- user total commits
CREATE FUNCTION udf_UserTotalCommits(@username VARCHAR(100))
RETURNS INT
AS
BEGIN
RETURN (SELECT COUNT(*) FROM Users u
JOIN Commits c ON c.ContributorId = u.Id
WHERE u.Username = @username)
END

-- find extensions 
CREATE PROCEDURE usp_FindByExtension(@extension VARCHAR(50))
AS
BEGIN
SELECT f.Id,f.Name, CONCAT(f.Size, 'KB')AS [Size] FROM Files f
--WHERE CHARINDEX(@extension, f.Name) > 0
WHERE f.Name LIKE ('%' + @extension)
ORDER BY f.Id,f.Name, [Size] DESC
END