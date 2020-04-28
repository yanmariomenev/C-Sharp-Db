using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.Dtos.Import
{
    [XmlType("Car")]
   public class ImportCarsDto
    {
        [XmlElement("make")]
        public string Make { get; set; }
        [XmlElement("model")]
        public string Model { get; set; }
        [XmlElement("TraveledDistance")]
        public long TraveledDistance { get; set; }
        //[XmlArray("parts")]
        [XmlElement("parts")]
        //public int[] PartsId { get; set; }
        public PartsDto Parts { get; set; }
    }
    [XmlRoot("parts")]
    public class PartsDto
    {
        [XmlElement("partId")]
        public PartsIdDto[] PartsId { get; set; }
    }
    [XmlRoot(ElementName = "parts")]
    public class PartsIdDto
    {
        [XmlAttribute(AttributeName = "id")]
        public int PartId { get; set; }
    }
}
