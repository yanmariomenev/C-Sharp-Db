using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Cinema.DataProcessor.ImportDto
{
    [XmlType("Ticket")]
    public class ImportTicketDto
    {
        [XmlElement("ProjectionId")]
        public int ProjectionId { get; set; }
        [Range(0.01, double.MaxValue), Required]
        [XmlElement("Price")]
        public decimal Price { get; set; }
    }
}