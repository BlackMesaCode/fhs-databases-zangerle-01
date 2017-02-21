using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fhs_databases_zangerle_01
{

    public class MySQL
    {

        public static void PopulateMysqlDatabase(string sourceDir)
        {
            var connectionString = CreateConnectionString("127.0.0.1", "root", "pass", "gutenberg");

            int errorCount = 0;

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                var files = Directory.GetFiles(sourceDir);

                var i = 0;
                foreach (var file in files)
                {
                    var content = File.ReadAllText(file);
                    var insertStatement = $@"INSERT INTO `book` (`content`) VALUES (@content);";
                    var insertCommand = new MySqlCommand();
                    insertCommand.CommandText = insertStatement;
                    insertCommand.Parameters.AddWithValue("@content", content);
                    insertCommand.Connection = connection;
                    try
                    {
                        insertCommand.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        errorCount++;
                        //throw;
                    }

                    Console.WriteLine($"{i}/{files.Count()}");
                    i++;
                }
                connection.Close();
            }
            Console.WriteLine("Finished populating mysql database");
            Console.WriteLine("Skipped documents: " + errorCount);


        }


        public static string CreateConnectionString(string server, string user, string pass, string database = "")
        {
            MySqlConnectionStringBuilder connectionString = new MySqlConnectionStringBuilder();
            connectionString.Server = server;
            connectionString.UserID = user;
            connectionString.Password = pass;
            connectionString.Database = database;
            return connectionString.ToString();

        }

    }
}
