using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace pokladnaInitial
{
    public static class DatabaseConnection
    {
        static SQLiteConnection m_dbConnection;
        static SQLiteCommand command;
        static string sql;

        public static void ConnectToDatabase()
        {
            try
            {
                m_dbConnection = new SQLiteConnection("Data Source=db_shop.sqlite; FailIfMissing=True");
                m_dbConnection.Open();
            }

            catch (Exception e)
            {
                Trace.WriteLine(e);

                string message = "Nepodařilo se nalést databázový soubor: 'db_shop.sqlite' Chcete vytvořit novou prázdnou databázi? ";
                string title = "Chyba databáze";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(message, title, buttons);
                if (result == DialogResult.Yes)
                {
                    CreateNewDatabase();
                }
                else
                {
                    System.Environment.Exit(0);
                }
            }
        }

        public static void CreateNewDatabase()
        {
            try
            {
                SQLiteConnection.CreateFile("db_shop.sqlite");
                m_dbConnection = new SQLiteConnection("Data Source=db_shop.sqlite; FailIfMissing=True");
                m_dbConnection.Open();
                Trace.WriteLine("Database has been created");
                CreateTablesInNewDatabase();
            }
            catch (Exception)
            {
                Trace.WriteLine("Se to posralo :D");
                MessageBox.Show("Nastala neočekávaná chyba, nelze vytvořit soubor s databází zkontrolujte zda má aplikace dostatečná opraávnění"); 
            }
        }

        private static void CreateTablesInNewDatabase()
        {
            sql = "CREATE TABLE `Products` (`id` varchar(255) PRIMARY KEY,`title` varchar(255),`price` float,`quantity` int,`isAvailable` bool)";
            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
            Trace.WriteLine("Products table has been created");

            sql = "CREATE TABLE `HistoryOfPurchases` (`order_id` varchar(255),`product_id` varchar(255),`title` varchar(255),`price` float,`quantity` int,`timeOfPurchase` date,PRIMARY KEY(`order_id`, `product_id`), FOREIGN KEY (`product_id`) REFERENCES `Products` (`id`))";
            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
            Trace.WriteLine("HistoryOfPurchases table has been created");

            //sql = "ALTER TABLE `HistoryOfPurchase` ADD FOREIGN KEY (`product_id`) REFERENCES `Products` (`id`)"; Cannot do that SQlite is not supporting that :/
            //command = new SQLiteCommand(sql, m_dbConnection);
            //command.ExecuteNonQuery();
            //Trace.WriteLine("Relation has been created");
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
            Trace.WriteLine("Vložena data (Petříček-3000, Janička-6000, Já Sám-9009).");


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
