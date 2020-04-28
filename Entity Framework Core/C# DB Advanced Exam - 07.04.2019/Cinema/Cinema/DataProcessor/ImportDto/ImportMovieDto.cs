using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Cinema.Data.Models;

namespace Cinema.DataProcessor.ImportDto
{
   public class ImportMovieDto
    {

        [Required]
        [MinLength(3), MaxLength(20)]
        public string Title { get; set; }

        public string Genre { get; set; }

        public TimeSpan Duration { get; set; }

        [Range(1, 10)]
        public double Rating { get; set; }

        [Required]
        [MinLength(3), MaxLength(20)]
        public string Director { get; set; }
    }
}
