using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var context = new CarDealerContext())
            {
                //var xmlSupplierDir = File.ReadAllText
                //    (@"C:\Users\Yanmario\Desktop\Entity Framework Core\09.XML Processing\CarDealer\CarDealer\Datasets\suppliers.xml");
                //var xmlPartsDir = File.ReadAllText
                //    (@"C:\Users\Yanmario\Desktop\Entity Framework Core\09.XML Processing\CarDealer\CarDealer\Datasets\parts.xml");
                //var xmlCarsDir = File.ReadAllText
                //    (@"C:\Users\Yanmario\Desktop\Entity Framework Core\09.XML Processing\CarDealer\CarDealer\Datasets\cars.xml");
                //var xmlCustomersDir = File.ReadAllText
                //    (@"C:\Users\Yanmario\Desktop\Entity Framework Core\09.XML Processing\CarDealer\CarDealer\Datasets\customers.xml");
                //var xmlSalesDir = File.ReadAllText
                //    (@"C:\Users\Yanmario\Desktop\Entity Framework Core\09.XML Processing\CarDealer\CarDealer\Datasets\sales.xml");
                var result = GetSalesWithAppliedDiscount(context);
                Console.WriteLine(result);
            }
            //var context = new CarDealerContext();
 
           
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportSupplierDto[]), new XmlRootAttribute("Suppliers"));
            var supplierDto = (ImportSupplierDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var suppliers = new List<Supplier>();

            foreach (var dto in supplierDto)
            {
                var supplier = new Supplier
                {
                    Name = dto.Name,
                    IsImporter = dto.isImported
                };
                suppliers.Add(supplier);
            }
            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();
            return $"Successfully imported {suppliers.Count}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportPartsDto[]), new XmlRootAttribute("Parts"));
            var partsDtos = (ImportPartsDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var parts = new List<Part>();

            foreach (var dto in partsDtos)
            {
                var findSupplier = context.Suppliers.Find(dto.SupplierId);
                if (findSupplier != null)
                {
                    var part = new Part()
                    {
                        Name = dto.Name,
                        Price = dto.Price,
                        Quantity = dto.Quantity,
                        SupplierId = dto.SupplierId,
                    };
                    parts.Add(part);
                }
            }
            context.Parts.AddRange(parts);
            context.SaveChanges();
            return $"Successfully imported {parts.Count}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCarsDto[]), new XmlRootAttribute("Cars"));
            var carDto = (ImportCarsDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var cars = new List<Car>();

            foreach (var dto in carDto)
            {
                var car = new Car
                {
                    Make = dto.Make,
                    Model = dto.Model,
                    TravelledDistance = dto.TraveledDistance,
                };
                cars.Add(car);
                foreach (var part in dto.Parts.PartsId/*.Distinct()*/)
                {
                    if (context.Parts.Any(p => p.Id == part.PartId))
                    {
                        var partCar = new PartCar { CarId = car.Id, PartId = part.PartId };

                        if (car.PartCars.FirstOrDefault(pc => pc.PartId == part.PartId) == null)
                        {
                            car.PartCars.Add(partCar);
                        }
                    }
                    //var findPart = context.Parts.Find(part.PartId);
                    //if (findPart != null)
                    //{
                    //    var partOfCar = new PartCar
                    //    {
                    //        CarId = car.Id,
                    //        PartId = part.PartId
                    //    };
                    //    car.PartCars.Add(partOfCar);
                    //}
                }
                //cars.Add(car);
            }
            context.Cars.AddRange(cars);
            context.SaveChanges();
            return $"Successfully imported {cars.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCustomerDto[]), new XmlRootAttribute("Customers"));
            var customerDtos = (ImportCustomerDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var customers = new List<Customer>();

            foreach (var dto in customerDtos)
            {
                var customer = new Customer
                {
                    Name = dto.Name,
                    BirthDate = dto.BirthDate,
                    IsYoungDriver = dto.IsYoungDriver,
                };
                customers.Add(customer);
            }
            context.Customers.AddRange(customers);
            context.SaveChanges();
           return $"Successfully imported {customers.Count}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportSalesDto[]), new XmlRootAttribute("Sales"));
            var salesDtos = (ImportSalesDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var sales = new List<Sale>();

            foreach (var dto in salesDtos)
            {
                if (context.Cars.Find(dto.CarId) != null)
                {
                    var sale = new Sale
                    {
                     CarId = dto.CarId,
                     CustomerId = dto.CustomerId,
                     Discount = dto.Discount
                    };
                    sales.Add(sale);
                }
            }
            context.Sales.AddRange(sales);
            context.SaveChanges();
            return $"Successfully imported {sales.Count}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var carsWithDistance = context.Cars
                .Where(c => c.TravelledDistance > 2000000)
                .Select(c => new ExportCarsWithDistanceDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TravelledDistance
                })
                .OrderBy(m => m.Make)
                .ThenBy(m => m.Model)
                .Take(10)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportCarsWithDistanceDto[]), new XmlRootAttribute("cars"));
            var sb = new StringBuilder();

            var nameSpaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            xmlSerializer.Serialize(new StringWriter(sb), carsWithDistance, nameSpaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var bmwCars = context.Cars.Where(m => m.Make == "BMW")
                .Select(x => new ExportCarsFromBmwDto
                {
                    Id = x.Id,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .ToList();
           
            var xmlSerializer = new XmlSerializer(typeof(List<ExportCarsFromBmwDto>), new XmlRootAttribute("cars"));
            var sb = new StringBuilder();

            var nameSpaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            xmlSerializer.Serialize(new StringWriter(sb), bmwCars, nameSpaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var supplier = context.Suppliers.Where(s => s.IsImporter == false)
                .Select(s => new ExportSupplierDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count()
                })
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportSupplierDto[]), new XmlRootAttribute("suppliers"));
            var sb = new StringBuilder();

            var nameSpaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            xmlSerializer.Serialize(new StringWriter(sb), supplier, nameSpaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new ExportCarsWithPartsDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    Parts = c.PartCars.Select(p => new CarPartsDto
                        {
                            Name = p.Part.Name,
                            Price = p.Part.Price
                        })
                        .OrderByDescending(p => p.Price)
                        .ToArray()
                })
                .OrderByDescending(c => c.TravelledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportCarsWithPartsDto[]), new XmlRootAttribute("cars"));
            var sb = new StringBuilder();

            var nameSpaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            xmlSerializer.Serialize(new StringWriter(sb), cars, nameSpaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Any())
                .Select(c => new ExportSellsByCustomerDto
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count(),
                    SpentMoney = c.Sales.SelectMany(s => s.Car.PartCars).Sum(cp => cp.Part.Price)
                })
                .OrderByDescending(c => c.SpentMoney)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportSellsByCustomerDto[]), new XmlRootAttribute("customers"));
            var sb = new StringBuilder();

            var nameSpaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            xmlSerializer.Serialize(new StringWriter(sb), customers, nameSpaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(c => new ExportSellsWithDiscountDto
                {
                    Car = new ExportCarDto
                    {
                        Make = c.Car.Make,
                        Model = c.Car.Model,
                        TravelledDistance = c.Car.TravelledDistance
                    },
                    Discount = c.Discount,
                    CustomerName = c.Customer.Name,
                    Price = c.Car.PartCars.Sum(p => p.Part.Price),
                    PriceWithDiscount = c.Car.PartCars.Sum(p => p.Part.Price) -
                                        c.Car.PartCars.Sum(p => p.Part.Price) * c.Discount / 100
                })
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportSellsWithDiscountDto[]), new XmlRootAttribute("sales"));
            var sb = new StringBuilder();

            var nameSpaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            xmlSerializer.Serialize(new StringWriter(sb), sales, nameSpaces);

            return sb.ToString().TrimEnd();
        }
    }

}