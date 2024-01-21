using Microsoft.Data.SqlClient;

namespace PrintAllMinionNames
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var connection = new SqlConnection("Server=.;Integrated Security=True;Database=MinionsDB;TrustServerCertificate=True"))
            {
                connection.Open();

                var minionList = new List<string>();

                string getMinionsQuery = "SELECT Name FROM Minions";

                using (var selectCommand = new SqlCommand(getMinionsQuery, connection))
                {
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            minionList.Add(reader.GetString(0));
                        }
                    }
                }

                int n = minionList.Count;

                for (int i = 0; i < n / 2; i++)
                {
                    Console.WriteLine(minionList[i]);
                    Console.WriteLine(minionList[n - 1 - i]);
                }

                if (n % 2 != 0)
                {
                    Console.WriteLine(minionList[n / 2]);
                }
            }
        }
    }
}