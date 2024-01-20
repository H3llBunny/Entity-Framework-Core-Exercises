using Microsoft.Data.SqlClient;

namespace InitialSetup
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var connection = new SqlConnection(
                "Data Source=localhost; Initial Catalog=master;Integrated Security=True;TrustServerCertificate=True;"))
            {
                connection.Open();

                string createDatabaseQuery = "CREATE DATABASE MinionsDB";

                using (var command = new SqlCommand(createDatabaseQuery, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine($"Database {connection.Database} created successfully.");
                }
            }

            using (var connection = new SqlConnection(
                "Data Source=localhost; Initial Catalog=MinionsDB; Integrated Security=True;TrustServerCertificate=True;"))
            {
                connection.Open();

                string[] createTableQueries = new string[]
                {
                    @"
                        CREATE TABLE Countries (
                            Id INT PRIMARY KEY IDENTITY,
                            Name VARCHAR(100)
                        )",
                    @"
                        CREATE TABLE Towns (
                            Id INT PRIMARY KEY IDENTITY,
                            Name VARCHAR(50),
                            CountryCode INT FOREIGN KEY REFERENCES Countries(Id)
                            )",
                    @"
                        CREATE TABLE Minions (
                            Id INT PRIMARY KEY IDENTITY,
                            Name VARCHAR(30),
                            Age INT,
                            TownId INT FOREIGN KEY REFERENCES Towns(Id)
                            )",
                    @"
                        CREATE TABLE EvilnessFactors (
                            Id INT PRIMARY KEY IDENTITY,
                            Name VARCHAR(50)
                            )",
                    @"
                        CREATE TABLE Villains (
                            Id INT PRIMARY KEY IDENTITY,
                            Name VARCHAR(50),
                            EvilnessFactorId INT FOREIGN KEY REFERENCES EvilnessFactors(Id)
                            )",
                    @"
                        CREATE TABLE MinionsVillains (
                            MinionId INT FOREIGN KEY REFERENCES Minions(Id),
                            VillainId INT FOREIGN KEY REFERENCES Villains(Id),
                            CONSTRAINT PK_MinionsVillains PRIMARY KEY (MinionId, VillainId)
                            )"
                };

                foreach (var createTableQuery in createTableQueries)
                {
                    using (var command = new SqlCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Table created successfully.");
                    }
                }
            }
        }
    }
}