using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Cinema.DataProcessor.ImportDto
{
    [XmlType("Customer")]
    public class ImportCustomerDto
    {
        [XmlElement("FirstName")]
        public string FirstName { get; set; }
        [MinLength(3), MaxLength(20), Required]
        [XmlElement("LastName")]
        public string LastName { get; set; }
        [Range(12, 110), Required]
        [XmlElement("Age")]
        public int Age { get; set; }
        [Range(0.01, double.MaxValue), Required]
        [XmlElement("Balance")]
        public decimal Balance { get; set; }
        [XmlArray("Tickets")]
        public ImportTicketDto[] Tickets { get; set; }
    }
}