using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Cinema.Data.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }
        [MinLength(3), MaxLength(20), Required]
        public string Title { get; set; }
        [Required]
        public Genre Genre { get; set; }
        [Required]
        public TimeSpan Duration { get; set; }
        [Range(1,10), Required]
        public double Rating { get; set; }
        [MinLength(3),MaxLength(20)]
        public string Director { get; set; }

        public ICollection<Projection> Projections { get; set; } = new HashSet<Projection>();
    }

    public enum Genre
    {
        Action = 0,
        Drama = 1,
        Comedy = 2,
        Crime = 3,
        Western = 4,
        Romance = 5,
        Documentary = 6,
        Children = 7,
        Animation = 8,
        Musical = 9
    }
}
