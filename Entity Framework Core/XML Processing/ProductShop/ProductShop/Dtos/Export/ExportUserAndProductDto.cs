using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("User")]
   public class ExportUserAndProductDto
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }
        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlElement("age")]
        public int? Age { get; set; }
        [XmlElement("SoldProducts")]
        public userProductSoldDto UserProductSoldDto { get; set; }
    }

   public class userProductSoldDto
   {
       [XmlElement("count")]
        public int Count { get; set; }
        [XmlArray("products")]
        public ProductDto[] ProductDto { get; set; }
   }
}
