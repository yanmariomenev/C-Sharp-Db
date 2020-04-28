--PROBLEM 2 INSERT
INSERT INTO Subjects([Name], Lessons)
VALUES
('Geometry', 12),
('Health', 10),
('Drama', 7),
('Sports', 9)

INSERT INTO Teachers(FirstName,
LastName,
[Address],
Phone,
SubjectId
)
VALUES
(
    'Ruthanne',
    'Bamb',
    '84948 Mesta Junction',
    '3105500146',
    6
),
(
    'Gerrard',
    'Lowin',
    '370 Talisman Plaza',
    '3324874824',
    2
),
(
    'Merrile',
    'Lambdin',
    '81 Dahle Plaza',
    '4373065154',
    5
),
(
    'Bert',
    'Ivie',
    '2 Gateway Circle',
    '4409584510',
    4
);

-- PROBLEM 3 UPDATE

UPDATE StudentsSubjects
SET Grade = 6.00
WHERE SubjectId IN (1,2) AND Grade >= 5.50

-- PROBLEM 4 DELETE
DELETE FROM StudentsTeachers
WHERE TeacherId IN (SELECT Id FROM Teachers
WHERE Phone LIKE '%72%')

DELETE FROM Teachers
WHERE CHARINDEX('72', Phone) > 0 -- or SELECT Id FROM Teachers WHERE Phone LIKE '%72$'


-- PROBLEM 5 
SELECT FirstName, LastName, Age FROM Students
WHERE Age >= 12
ORDER BY FirstName, LastName

-- PROBLEM 6
SELECT s.FirstName, s.LastName, COUNT(st.TeacherId) AS TeacherCount 
FROM Students s
LEFT OUTER JOIN StudentsTeachers st
ON st.StudentId = s.Id
GROUP BY s.FirstName, s.LastName

-- PROBLEM 7

SELECT CONCAT(FirstName, ' ' , LastName) [Full Name]
FROM Students s
LEFT OUTER JOIN StudentsExams se
ON se.StudentId = s.Id
WHERE se.ExamId IS NULL
ORDER BY [Full Name]

-- PROBLEM 8

SELECT TOP(10) s.FirstName, s.LastName, CAST(AVG(se.Grade)AS DECIMAL(3,2))[Grade]
FROM Students s
JOIN StudentsExams se
ON se.StudentId = s.Id
GROUP BY s.FirstName, s.LastName
ORDER BY [Grade] DESC, s.FirstName, s.LastName

--PROBLEM 9
SELECT CONCAT(s.FirstName, ' ' , s.MiddleName + ' ' , s.LastName) [Full Name]
FROM Students s
LEFT OUTER JOIN StudentsSubjects ss
ON ss.StudentId = s.Id
WHERE ss.StudentId IS NULL
ORDER BY [Full Name]

-- PROBLEM 10

SELECT s.[Name], AVG(ss.Grade) [AverageGrade]
FROM Subjects s
JOIN StudentsSubjects ss
ON ss.SubjectId = s.Id
GROUP BY  s.[Name], s.Id
ORDER BY s.Id

-- PROBLEM 11

CREATE FUNCTION udf_ExamGradesToUpdate(@studentId INT, @grade DECIMAL(3,2))
RETURNS NVARCHAR(100)
AS
BEGIN

DECLARE @studentName NVARCHAR(30) = (SELECT TOP(1) FirstName FROM Students
WHERE Id= @studentId)
IF(@studentName IS NULL)
BEGIN
RETURN 'The student with provided id does not exist in the school!';
END

IF(@grade > 6.00)
BEGIN
RETURN 'Grade cannot be above 6.00!';
END

DECLARE @studentGradeCount INT = (SELECT COUNT(Grade) FROM StudentsExams
WHERE StudentId = @studentId AND Grade > @grade AND Grade <= (Grade + 0.50))

RETURN CONCAT('You have to update ', @studentGradeCount, ' grades for the student ',@studentName)

END

--PROBLEM 12

CREATE PROC usp_ExcludeFromSchool(@StudentId INT)
AS
BEGIN

DECLARE @studentsMatchingId BIT =(SELECT COUNT(*) FROM Students
WHERE Id = @StudentId);

IF(@studentsMatchingId = 0)
BEGIN
RAISERROR ('This school has no student with the provided id!',16 , 1)
RETURN;
END

DELETE FROM StudentsExams
WHERE StudentId = @StudentId

DELETE FROM StudentsSubjects
WHERE StudentId = @StudentId

DELETE FROM StudentsTeachers
WHERE StudentId = @StudentId

DELETE FROM Students
WHERE Id = @StudentId

END