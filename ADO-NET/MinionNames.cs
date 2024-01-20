using Microsoft.Data.SqlClient;
using System.Text;

namespace MinionNames
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var connection = new SqlConnection(
                "Data Source=localhost; Initial Catalog=MinionsDB;Integrated Security=True;TrustServerCertificate=True;"))
            {
                connection.Open();

                int villainId = int.Parse(Console.ReadLine());

                string villainNameAndMinionsById =
                    $@"SELECT
                        CONCAT('Villain', ': ', v.Name) AS VillainName,
                        m.Name AS MinionName,
                        m.Age
                    FROM Villains v
                    LEFT JOIN MinionsVillains mv ON v.Id = mv.VillainId
                    LEFT JOIN Minions m ON mv.MinionId = m.Id
                    WHERE v.Id = {villainId}
                    ORDER BY m.Name";

                using (var command = new SqlCommand(villainNameAndMinionsById, connection))
                {
                    using (var sqlDataReader = command.ExecuteReader())
                    {
                        if (!sqlDataReader.HasRows)
                        {
                            Console.WriteLine($"No villain with ID {villainId} exists in the database.");
                        }
                        else
                        {
                            var sb = new StringBuilder();
                            int count = 1;

                            sqlDataReader.Read();

                            sb.AppendLine(sqlDataReader[0] as string);

                            if (sqlDataReader["MinionName"] == DBNull.Value)
                            {
                                sb.Append("(no minions)");
                            }
                            else
                            {
                                sb.Append(count + ". " + sqlDataReader["MinionName"] as string);
                                sb.AppendLine($" {sqlDataReader.GetInt32(2)}");

                                while (sqlDataReader.Read())
                                {
                                    sb.AppendLine($"{count++}. {sqlDataReader["MinionName"] as string} {sqlDataReader.GetInt32(2)}");
                                    count++;
                                }
                            }

                            Console.WriteLine(sb);
                        }
                    }
                }
            }
        }
    }
}