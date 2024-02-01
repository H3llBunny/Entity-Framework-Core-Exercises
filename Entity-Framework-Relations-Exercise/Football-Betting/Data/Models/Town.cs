using System.ComponentModel.DataAnnotations;

namespace FootballBetting.Data.Models
{
    public class Town
    {
        public Town()
        {
            this.Teams = new HashSet<Team>();
        }

        [Key]
        public int TownId { get; set; }

        public string Name { get; set; }

        public ICollection<Team> Teams { get; set; }

        public int CountryId { get; set; }

        public Country Country { get; set; }
    }
}
