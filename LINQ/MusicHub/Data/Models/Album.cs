using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MusicHub.Data.Models
{
    public class Album
    {
        public Album()
        {
            this.Songs = new HashSet<Song>();
        }

        public int Id { get; set; }

        [MaxLength(40)]
        [Required]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public decimal Price => this.Songs != null ? this.Songs.Sum(s => s.Price) : 0m;

        public int? ProducerId { get; set; }
        public Producer? Producer { get; set; }

        public ICollection<Song> Songs { get; set; }
    }
}
