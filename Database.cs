using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Forms;

namespace pokladnaInitial
{
    public static class DatabaseConnection
    {
        static SQLiteConnection m_dbConnection;
        static SQLiteCommand command;
        static string sql;

        public static void CreateNewDatabase()
        {
            SQLiteConnection.CreateFile($"db_shop.sqlite");
            Console.WriteLine("Database has been created");
        }

        private static void CreateTablesInNewDatabase()
        {
            sql = "CREATE TABLE `Products` (`id` varchar(255) PRIMARY KEY,`title` varchar(255),`price` float,`quantity` int,`isAvaible` bool)";
            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
            Console.WriteLine("Products table has been created");

            sql = "CREATE TABLE `HistoryOfPurchases` (`order_id` varchar(255),`product_id` varchar(255),`title` varchar(255),`price` float,`quantity` int,PRIMARY KEY(`order_id`, `product_id`))";
            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
            Console.WriteLine("HistoryOfPurchases table has been created");

            sql = "ALTER TABLE `HistoryOfPurchase` ADD FOREIGN KEY (`product_id`) REFERENCES `Products` (`id`)";
            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
            Console.WriteLine("Relation has been created");
        }

        public static void ConnectToDatabase()
        {
            try
            {
                m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
                m_dbConnection.Open();
            }

            catch (Exception)
            {
                string message = "Nepodařilo se nalést databázový soubor: 'db_shop.sqlite' Chcete vytvořit novou prázdnou databázi? ";
                string title = "Chyba databáze";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(message, title, buttons);
                if (result == DialogResult.Yes)
                {

                }
                else
                {
                    System.Environment.Exit(0);
                }
            }
        }

        private static void DatabzeTest()
        {
            // naplnění tabulky daty
            sql = "insert into highscores (jmeno, skore) values ('Petříček', 3000)";
            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
            sql = "insert into highscores (jmeno, skore) values ('Janička', 6000)";
            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
            sql = "insert into highscores (jmeno, skore) values ('Já Sám ', 9009)";
            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
            Console.WriteLine("Vložena data (Petříček-3000, Janička-6000, Já Sám-9009).");


            // výpis dat z tabulky
            Console.WriteLine("Výpis podle bodů:");
            Console.WriteLine("Jméno:   \t Skóre:");
            sql = "select * from highscores order by skore desc";
            command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                Console.WriteLine(reader["jmeno"] + " \t " + reader["skore"]);

            // uzavření připojení k databázi
            m_dbConnection.Close();

            Console.ReadLine();
        }
    }
}
