using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballBetting.Data.Models
{
    public class Team
    {
        public Team()
        {
            this.Players = new HashSet<Player>();
            this.HomeTeamGames = new HashSet<Game>();
            this.AwayTeamGames = new HashSet<Game>();
        }

        [Key]
        public int TeamId { get; set; }

        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        [MaxLength(255)]
        public string? LogoUrl { get; set; }

        [Column(TypeName = "varchar")]
        [MaxLength(3)]
        public string Initials { get; set; }

        [Column(TypeName = "money")]
        public decimal Budget { get; set; }

        [ForeignKey(nameof(Color.ColorId))]
        public int PrimaryKitColorId { get; set; }
        public Color PrimaryKitColor { get; set; }

        [ForeignKey(nameof(Color.ColorId))]
        public int SecondaryKitColorId { get; set; }
        public Color SecondaryKitColor { get; set; }

        public int TownId { get; set; }

        public Town Town { get; set; }

        [ForeignKey(nameof(Team.TeamId))]
        public int HomeTeamId { get; set; }
        public Team HomeTeam { get; set; }

        [ForeignKey(nameof(Team.TeamId))]
        public int AwayTeamId { get; set; }
        public Team AwayTeam { get; set; }

        public ICollection<Player> Players { get; set; }

        [InverseProperty(nameof(Game.HomeTeam))]
        public ICollection<Game> HomeTeamGames { get; set; }

        [InverseProperty(nameof(Game.AwayTeam))]
        public ICollection<Game> AwayTeamGames { get; set; }
    }
}
