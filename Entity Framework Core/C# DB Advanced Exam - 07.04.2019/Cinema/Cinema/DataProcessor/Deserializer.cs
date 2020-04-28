using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Castle.Core.Internal;
using Cinema.Data.Models;
using Cinema.DataProcessor.ImportDto;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

namespace Cinema.DataProcessor
{
    using System;

    using Data;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";
        private const string SuccessfulImportMovie 
            = "Successfully imported {0} with genre {1} and rating {2}!";
        private const string SuccessfulImportHallSeat 
            = "Successfully imported {0}({1}) with {2} seats!";
        private const string SuccessfulImportProjection 
            = "Successfully imported projection {0} on {1}!";
        private const string SuccessfulImportCustomerTicket 
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            var movieDto = JsonConvert.DeserializeObject<ImportMovieDto[]>(jsonString);
            
            var movies = new List<Movie>();
            var sb = new StringBuilder();
            foreach (var m in movieDto)
            {
                var isValidDto = IsValid(m);
                var movieExist = movies.Any(t => t.Title == m.Title);
                var isValidEnum = Enum.TryParse(typeof(Genre), m.Genre, out object genre);

                if (movieExist || !isValidDto || !isValidEnum)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var movie = new Movie()
                {
                    Title = m.Title,
                    Genre = (Genre) Enum.Parse(typeof(Genre), m.Genre),
                    Duration = m.Duration,
                    Rating = m.Rating,
                    Director = m.Director

                };
                movies.Add(movie);
                sb.AppendLine(string.Format(SuccessfulImportMovie, movie.Title, movie.Genre,
                    movie.Rating.ToString("F2")));
            }
            context.Movies.AddRange(movies);
            context.SaveChanges();
            var result = sb.ToString();
            return result;
            
        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            var hallsAndSeatsDto = JsonConvert.DeserializeObject<ImportHallSeatsDto[]>(jsonString);

            var hallWithSeatsCollection = new List<Hall>();
            var sb = new StringBuilder();
            foreach (var hall in hallsAndSeatsDto)
            {
                if (IsValid(hall))
                {
                    var hallAndSeats = new Hall
                    {
                        Name = hall.Name,
                        Is4Dx = hall.Is4Dx,
                        Is3D = hall.Is3D
                    };

                    for (int i = 0; i < hall.Seats; i++)
                    {
                        hallAndSeats.Seats.Add(new Seat());
                    }
                    hallWithSeatsCollection.Add(hallAndSeats);

                    var status = "Normal";

                    if (hallAndSeats.Is4Dx)
                    {
                        status = hallAndSeats.Is3D ? "4Dx/3D" : "4Dx";
                    }
                    else if (hallAndSeats.Is3D)
                    {
                        status = "3D";
                    }

                    sb.AppendLine(string.Format(SuccessfulImportHallSeat, hallAndSeats.Name, status,
                        hallAndSeats.Seats.Count));
                    //context.Halls.Add(hallAndSeats);
                    //AddSeatsInDatabase(CinemaContext context, hallAndSeats.Id, hall.Seats);
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }
            context.Halls.AddRange(hallWithSeatsCollection);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportProjectionDto[]), new XmlRootAttribute("Projections"));

            var projectionsDto = (ImportProjectionDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var projections = new List<Projection>();
            var sb = new StringBuilder();

            foreach (var dto in projectionsDto)
            {
                var isMovieValid = context.Movies.Find(dto.MovieId);
                var isHallValid = context.Halls.Find(dto.HallId);
                if (isMovieValid == null || isHallValid == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var projection = new Projection
                {
                    MovieId = dto.MovieId,
                    HallId = dto.HallId,
                    DateTime = DateTime.ParseExact(dto.DateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                };
                projections.Add(projection);
                sb.AppendLine
                    (String.Format(SuccessfulImportProjection, isMovieValid.Title, projection.DateTime.ToString
                    ("MM/dd/yyyy", CultureInfo.InvariantCulture)));
            }
            context.Projections.AddRange(projections);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCustomerDto[]), new XmlRootAttribute("Customers"));

            var customersDto = (ImportCustomerDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var customersCollection = new List<Customer>();
            var sb = new StringBuilder();

            foreach (var dto in customersDto)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var customer = new Customer
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Age = dto.Age,
                    Balance = dto.Balance
                };
                foreach (var ticketDto in dto.Tickets)
                {
                    customer.Tickets.Add(new Ticket
                    {
                        ProjectionId = ticketDto.ProjectionId,
                        Price = ticketDto.Price
                    });
                }

                sb.AppendLine(string.Format(SuccessfulImportCustomerTicket, customer.FirstName, customer.LastName,
                    customer.Tickets.Count));

                customersCollection.Add(customer);
            }
            context.Customers.AddRange(customersCollection);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            // needs using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}