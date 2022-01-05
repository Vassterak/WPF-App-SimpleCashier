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
        public static SQLiteConnection m_dbConnection;
        public static SQLiteCommand command;
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
                System.Windows.Forms.MessageBox.Show(e.ToString());

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
                MessageBox.Show("Nastala neočekávaná chyba, nelze vytvořit soubor s databází zkontrolujte zda má aplikace dostatečná opraávnění"); 
            }
        }

        private static void CreateTablesInNewDatabase()
        {
            sql = "CREATE TABLE `Products` (`id` varchar(255) PRIMARY KEY,`title` varchar(255),`price` float,`quantity` int,`isAvailable` bool)";
            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
            Trace.WriteLine("Products table has been created");

            sql = "CREATE TABLE `HistoryOfPurchases` (`order_id` varchar(255),`product_id` varchar(255),`title` varchar(255),`price` float,`quantity` int,`timeOfPurchase` varchar(255),PRIMARY KEY(`order_id`, `product_id`), FOREIGN KEY (`product_id`) REFERENCES `Products` (`id`))";
            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
            Trace.WriteLine("HistoryOfPurchases table has been created");

            //sql = "ALTER TABLE `HistoryOfPurchase` ADD FOREIGN KEY (`product_id`) REFERENCES `Products` (`id`)"; Cannot do that SQlite is not supporting that :/
            //command = new SQLiteCommand(sql, m_dbConnection);
            //command.ExecuteNonQuery();
            //Trace.WriteLine("Relation has been created");
        }

        public static bool InsertIntoDatabase(string sql)
        {
            DatabaseConnection.command.CommandText = sql;
            try
            {
                DatabaseConnection.command.ExecuteNonQuery();
                return true;
            }
            catch (Exception)
            {
                Trace.WriteLine("Nastala chyba při vykonávání zápisu do databáze");
                return false;
            }
        }
        public static bool InsertIntoDatabase(string sql, float price)
        {
            DatabaseConnection.command.Parameters.AddWithValue("@price", price);
            DatabaseConnection.command.CommandText = sql;
            try
            {
                DatabaseConnection.command.ExecuteNonQuery();
                return true;
            }
            catch (Exception)
            {
                Trace.WriteLine("Nastala chyba při vykonávání zápisu do databáze");
                return false;
            }
        }
    }
}
