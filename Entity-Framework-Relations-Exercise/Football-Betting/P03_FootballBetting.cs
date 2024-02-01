using FootballBetting.Data;

namespace FootballBetting
{
    internal class P03_FootballBetting
    {
        static void Main(string[] args)
        {
            var db = new FootballBettingContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }
    }
}
