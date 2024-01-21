using Microsoft.Data.SqlClient;
using System.Text;

namespace ChangeTownNamesCasing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var connection = new SqlConnection("Server=.;Integrated Security=True;Database=MinionsDB;TrustServerCertificate=True;"))
            {
                connection.Open();

                string countryName = Console.ReadLine();

                string townNameCasingQuery = $@"
                    UPDATE Towns 
                    SET Name = UPPER(t.Name) 
                    FROM Towns t 
                    JOIN Countries c ON t.CountryCode = c.Id
                    WHERE c.Name = @CountryName";

                using (var setCommand = new SqlCommand(townNameCasingQuery, connection))
                {
                    setCommand.Parameters.AddWithValue("@CountryName", countryName);

                    int chandegTownsCount = (int)setCommand.ExecuteNonQuery();

                    if (chandegTownsCount > 0)
                    {
                        Console.WriteLine($"{chandegTownsCount} town names were affected.");

                        string getTownNames = $@"
                            SELECT t.Name 
                            FROM Towns t 
                            JOIN Countries c ON t.CountryCode = c.Id
                            WHERE c.Name = @CountryName";

                        using (var getCommand = new SqlCommand(getTownNames, connection))
                        {
                            getCommand.Parameters.AddWithValue("@CountryName", countryName);

                            using (var townsReader = getCommand.ExecuteReader())
                            {
                                var townNames = new List<string>();

                                while (townsReader.Read())
                                {
                                    townNames.Add(townsReader.GetString(0));
                                }

                                Console.WriteLine($"[{string.Join(", ", townNames)}]");
                            }
                        }
                    }
                }
            }
        }
    }
}