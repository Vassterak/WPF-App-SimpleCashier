using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace pokladnaInitial
{
    public static class DatabaseConnection
    {
        private static void DatabzeTest()
        {

            // případné vytvoření databáze (jen poprvé)
            SQLiteConnection.CreateFile("MyDatabase.sqlite");
            Console.WriteLine("Vytvořena databáze.");

            // připojení k databázi
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            m_dbConnection.Open();
            string sql;
            SQLiteCommand command;

            // případné vytvoření tabulky (jen jednou)
            sql = "CREATE TABLE highscores (jmeno VARCHAR(20), skore INT)";
            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
            Console.WriteLine("Vytvořena tabulka.");

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
