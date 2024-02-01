using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballBetting.Data.Models
{
    public class User
    {
        public User()
        {
            this.Bets = new HashSet<Bet>();
        }

        [Key]
        public int UserId { get; set; }

        [Column(TypeName = "varchar")]
        [MaxLength(16)]
        public string Username { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(16, MinimumLength = 8)]
        public string Password { get; set; }

        [Column(TypeName = "varchar")]
        [EmailAddress]
        public string Email { get; set; }

        public string Name { get; set; }

        [Column(TypeName = "money")]
        public decimal Balance { get; set; }

        public ICollection<Bet> Bets { get; set; }
    }
}
