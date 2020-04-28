using System;
using System.Net.Mime;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using P01_StudentSystem.Data;
using P01_StudentSystem.Data.Models;

namespace P01_StudentSystem
{
   public class StartUp
    {
       public static void Main()
       {
           using var dbContext = new StudentSystemContext();
           Seed(dbContext);
       }

       private static void Seed(StudentSystemContext dbContext)
       {
            var students = new[]
             {
                new Student
                {
                    Birthday = new DateTime(1985, 2, 3),
                    Name = "Pesho Ivanov",
                    PhoneNumber = "0888123456",
                    RegisteredOn = new DateTime(2015, 6, 7)
                },

                new Student
                {
                    Name = "Gosho Petrov",
                    RegisteredOn = new DateTime(2016, 3, 5)
                },

                new Student
                {
                    Name = "Stamat Ivanov",
                    Birthday = new DateTime(1995, 1, 7),
                    RegisteredOn = new DateTime(2016, 12, 20)
                },

                new Student
                {
                    Name = "Mihail Stamatov",
                    RegisteredOn = new DateTime(2017, 1, 7),
                    PhoneNumber = "0877445566"
                }
            };

            dbContext.Students.AddRange(students);

            var courses = new[]
            {
                new Course
                {
                    Name = "JS Fundamentals",
                    Description = "JS for beginners",
                    StartDate = new DateTime(2016, 9, 18),
                    EndDate = new DateTime(2016, 10, 20),
                    Price = 180.00m
                },

                new Course
                {
                    Name = "JS Advanced",
                    StartDate = new DateTime(2016, 10, 21),
                    EndDate = new DateTime(2016, 11, 19),
                    Price = 180.00m
                },

                new Course
                {
                    Name = "Js Applications",
                    StartDate = new DateTime(2016, 11, 20),
                    EndDate = new DateTime(2016, 12, 18),
                    Description = "JS Приложения",
                    Price = 200.00m
                }
            };

            dbContext.Courses.AddRange(courses);

            var resources = new[]
            {
                new Resource
                {
                    Name = "Intro",
                    Url = "softuni.bg/resources/0123456789",
                    ResourceType = ResourcesType.Presentation,
                    Course = courses[0]
                },

                new Resource
                {
                    Name = "OOP Intro",
                    Url = "softuni.bg/resources/1245687",
                    ResourceType = ResourcesType.Video,
                    Course = courses[1]
                },

                new Resource
                {
                    Name = "Objects",
                    Url = "softuni.bg/resources/556688",
                    ResourceType = ResourcesType.Document,
                    Course = courses[0]
                }
            };

            dbContext.Resources.AddRange(resources);

            var homeworks = new[]
            {
                new Homework
                {
                     Content = "softuni.bg/homeworks/124578",
                     ContentType = ContentTypes.Zip,
                     SubmissionTime = new DateTime(2016, 2, 5, 12, 45, 55),
                     Course = courses[0],
                     Student = students[2]
                },

                new Homework
                {
                    Content = "softuni.bg/homeworks/225588",
                    ContentType = ContentTypes.Pdf,
                    SubmissionTime = new DateTime(2017, 5, 8, 14, 22, 36),
                    Course = courses[1],
                    Student = students[0]
                },

                new Homework
                {
                    Content = "softuni.bg/homeworks/44778855",
                    ContentType = ContentTypes.Application,
                    SubmissionTime = new DateTime(2017, 4, 6, 18, 22, 54),
                    Course = courses[1],
                    Student = students[2]
                }
            };

            dbContext.HomeworkSubmissions.AddRange(homeworks);

            var studentcourses = new[]
            {
                new StudentCourse
                {
                    Student = students[0],
                    Course = courses[0]
                },

                new StudentCourse
                {
                    Student = students[1],
                    Course = courses[0]
                },

                new StudentCourse
                {
                    Student = students[1],
                    Course = courses[2]
                }
            };

            dbContext.StudentCourses.AddRange(studentcourses);

            dbContext.SaveChanges();
        }
    }
}
