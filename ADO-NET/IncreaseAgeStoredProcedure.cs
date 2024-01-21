using Microsoft.Data.SqlClient;

namespace IncreaseAgeStoredProcedure
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var connection = new SqlConnection("Server=.;Integrated Security=True;Database=MinionsDB;TrustServerCertificate=True"))
            {
                connection.Open();

                string createStoredProcQuery = @"
                    CREATE OR ALTER PROC dbo.usp_GetOlder(@MinionId INT)
                    AS
                    	UPDATE Minions
                    	SET Age = Age + 1
                    	WHERE Id = @MinionId";

                using (var createCommand = new SqlCommand(createStoredProcQuery, connection))
                {
                    createCommand.ExecuteNonQuery();
                }

                int minionId = int.Parse(Console.ReadLine());

                string useStoredProcQuery = @"EXEC dbo.usp_GetOlder @MinionId";

                using (var useProcCommand = new SqlCommand(useStoredProcQuery, connection))
                {
                    useProcCommand.Parameters.AddWithValue("@MinionId", minionId);
                    useProcCommand.ExecuteNonQuery();
                }

                string getMinionNameAndAgeQuery = @"SELECT CONCAT(Name, ' - ', Age, ' years old') AS Output FROM Minions WHERE Id = @MinionId";

                using (var getCommand = new SqlCommand(getMinionNameAndAgeQuery, connection))
                {
                    getCommand.Parameters.AddWithValue("@MinionId", minionId);

                    using (var reader = getCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.WriteLine(reader["Output"]);
                        }
                    }
                }
            }
        }
    }
}