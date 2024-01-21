using Microsoft.Data.SqlClient;

namespace IncreaseMinionAge
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var connection = new SqlConnection("Server=.;Integrated Security=True;Database=MinionsDB;TrustServerCertificate=True;"))
            {
                connection.Open();

                int[] minionIds = Console.ReadLine().Split().Select(int.Parse).ToArray();

                string updateMinionsAgeQuery = @"
                    UPDATE Minions
                    SET Name = LOWER(name), Age = Age + 1
                    WHERE Id = @MinionId";

                using (var updateCommand = new SqlCommand(updateMinionsAgeQuery, connection))
                {
                    for (int i = 0; i <= minionIds.Length - 1; i++)
                    {
                        updateCommand.Parameters.AddWithValue("@MinionId", minionIds[i]);
                        updateCommand.ExecuteNonQuery();
                        updateCommand.Parameters.RemoveAt(0);
                    }
                }

                string getAllMinionsQuery = "SELECT CONCAT(Name, ' ', Age) AS Output FROM Minions";

                using (var getCommand = new SqlCommand(getAllMinionsQuery, connection))
                {
                    using (var reader = getCommand.ExecuteReader())
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