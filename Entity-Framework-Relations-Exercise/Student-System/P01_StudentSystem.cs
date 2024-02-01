using EfCoreRelationsExercise.Data;

namespace EfCoreRelationsExercise
{
    internal class P01_StudentSystem
    {
        static void Main(string[] args)
        {
            var db = new StudentSystemContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }
    }
}
