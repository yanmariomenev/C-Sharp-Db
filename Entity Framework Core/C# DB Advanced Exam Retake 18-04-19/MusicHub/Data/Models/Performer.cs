using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Serialization;

namespace MusicHub.Data.Models
{
    public class Performer
    {
        [Key]
        public int Id { get; set; }
        [MinLength(3), MaxLength(20), Required]
        public string FirstName { get; set; }
        [MinLength(3), MaxLength(20), Required]
        public string LastName { get; set; }
        [Range(18,70), Required]
        public int Age { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal NetWorth { get; set; }
        public ICollection<SongPerformer> PerformerSongs { get; set; } =  new HashSet<SongPerformer>();
    }
}