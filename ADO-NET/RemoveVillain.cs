using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace RemoveVillain
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var connetion = new SqlConnection("Server=.;Integrated Security=True;Database=MinionsDB;TrustServerCertificate=True;"))
            {
                connetion.Open();

                using (var transaction = connetion.BeginTransaction())
                {
                    try
                    {
                        int villainId = int.Parse(Console.ReadLine());
                        int minionsCount = 0;
                        string villainName = string.Empty;
                        string releaseMinionsQuery = @"
                            DELETE 
                            FROM MinionsVillains
                            WHERE VillainId = @VillainId";

                        using (var releaseCommand = new SqlCommand(releaseMinionsQuery, connetion, transaction))
                        {
                            releaseCommand.Parameters.AddWithValue("@VillainId", villainId);
                            minionsCount = (int)releaseCommand.ExecuteNonQuery();
                        }

                        string VillainNameQuery = @"
                            SELECT Name 
                            FROM Villains
                            WHERE Id = @VillainId";

                        using (var getNameCommand = new SqlCommand(VillainNameQuery, connetion, transaction))
                        {
                            getNameCommand.Parameters.AddWithValue("@VillainId", villainId);

                            using (var reader = getNameCommand.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    villainName = reader.GetString(0);
                                    reader.Close();

                                    string removeVillainQuery = @"
                                        DELETE
                                        FROM Villains
                                        WHERE Id = @VillainId";

                                    using (var removeCommand = new SqlCommand(removeVillainQuery, connetion, transaction))
                                    {
                                        removeCommand.Parameters.AddWithValue("@VillainId", villainId);
                                        removeCommand.ExecuteNonQuery();

                                        Console.WriteLine($"{villainName} was deleted.");
                                        Console.WriteLine($"{minionsCount} minions were released.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("No such villain was found.");
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }
    }
}