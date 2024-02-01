using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballBetting.Data.Models
{
    public class Color
    {
        public Color()
        {
            this.PrimaryKitColorTeams = new HashSet<Team>();
            this.SecondaryKitColorTeams = new HashSet<Team>();
        }

        [Key]
        public int ColorId { get; set; }

        public string Name { get; set; }

        [InverseProperty(nameof(Team.PrimaryKitColor))]
        public ICollection<Team> PrimaryKitColorTeams { get; set; }

        [InverseProperty(nameof(Team.SecondaryKitColor))]
        public ICollection<Team> SecondaryKitColorTeams { get; set; }
    }
}
