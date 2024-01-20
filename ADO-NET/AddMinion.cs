using Microsoft.Data.SqlClient;

namespace AddMinion
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var connection = new SqlConnection(
                "Data Source=localhost; Initial Catalog=MinionsDB;Integrated Security=True;TrustServerCertificate=True;"))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var minionInput = Console.ReadLine().Split();
                        string minionName = minionInput[1];
                        int age = int.Parse(minionInput[2]);
                        string townName = minionInput[3];

                        var villainInput = Console.ReadLine().Split();
                        string villainName = villainInput[1];

                        string checkTownQuery = $"SELECT COUNT(*) FROM Towns WHERE Name = @TownName";

                        using (var command = new SqlCommand(checkTownQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@TownName", townName);

                            int townCount = (int)command.ExecuteScalar();

                            if (townCount == 0)
                            {
                                string insertTownQuery = $@"INSERT INTO Towns (Name, CountryCode) 
                                    VALUES (@TownName, 1)";

                                using (var insertCommand = new SqlCommand(insertTownQuery, connection, transaction))
                                {
                                    insertCommand.Parameters.AddWithValue("@TownName", townName);
                                    insertCommand.ExecuteNonQuery();
                                    Console.WriteLine($"Town {townName} was added to the database.");
                                }
                            }
                        }

                        string checkVillainQuery = $"SELECT COUNT(*) FROM Villains WHERE Name = @VillainName";

                        using (var command = new SqlCommand(checkVillainQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@VillainName", villainName);

                            int villainCount = (int)command.ExecuteScalar();

                            if (villainCount == 0)
                            {
                                string insertVillainQuery = $@"INSERT INTO Villains (Name, EvilnessFactorId)
                            VALUES (@VillainName, 4)";

                                using (var insertCommand = new SqlCommand(insertVillainQuery, connection, transaction))
                                {
                                    insertCommand.Parameters.AddWithValue("@VillainName", villainName);
                                    insertCommand.ExecuteNonQuery();
                                    Console.WriteLine($"Villain {villainName} was added to the database.");
                                }
                            }
                        }

                        string checkMinionQuery = $"SELECT COUNT(*) FROM Minions WHERE Name = @MinionName AND Age = @MinionAge";

                        using (var command = new SqlCommand(checkMinionQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@MinionName", minionName);
                            command.Parameters.AddWithValue("@MinionAge", age);

                            int minionCount = (int)command.ExecuteScalar();

                            if (minionCount == 0)
                            {
                                string insertMinionQuery = $@"
                                INSERT INTO Minions (Name, Age, TownId)
                                VALUES (@MinionName, @MinionAge, (SELECT Id FROM Towns WHERE Name = @TownName))";

                                using (var insertCommand = new SqlCommand(insertMinionQuery, connection, transaction))
                                {
                                    insertCommand.Parameters.AddWithValue("@MinionName", minionName);
                                    insertCommand.Parameters.AddWithValue("@MinionAge", age);
                                    insertCommand.Parameters.AddWithValue("@TownName", townName);

                                    insertCommand.ExecuteNonQuery();
                                }
                            }
                        }

                        string insertServentQuery = @"
                            INSERT INTO MinionsVillains (MinionId, VillainId)
                            VALUES
                                (
                                    (SELECT Id FROM Minions WHERE Name = @MinionName AND Age = @MinionAge),
                                    (SELECT Id FROM Villains WHERE Name = @VillainName)
                                )";

                        using (var command = new SqlCommand(insertServentQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@MinionName", minionName);
                            command.Parameters.AddWithValue("@MinionAge", age);
                            command.Parameters.AddWithValue("@VillainName", villainName);

                            command.ExecuteNonQuery();

                            Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");

                        transaction.Rollback();
                    }
                }
            }
        }
    }
}