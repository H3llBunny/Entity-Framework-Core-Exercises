using Microsoft.Data.SqlClient;

namespace VillainNames
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var connection = new SqlConnection(
                "Data Source=localhost; Initial Catalog=MinionsDB;Integrated Security=True;TrustServerCertificate=True;"))
            {
                connection.Open();

                string getVillainNamesAndMinionsCount =
                    @"SELECT
	                    CONCAT(v.Name, ' - ', COUNT(mv.MinionId)) AS Output 
                    FROM MinionsVillains mv
                    JOIN Villains v ON mv.VillainId = v.Id
                    JOIN Minions m ON mv.MinionId = m.Id
                    GROUP BY v.Name
                    HAVING COUNT(mv.MinionID) > 3
                    ORDER BY COUNT(mv.MinionID) DESC";

                using (var command = new SqlCommand(getVillainNamesAndMinionsCount, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(reader["Output"]);
                        }
                    }
                }
            }
        }
    }
}