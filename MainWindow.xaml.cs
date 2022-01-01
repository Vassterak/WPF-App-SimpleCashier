using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace pokladnaInitial
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool readNewBarcode = false;
        List<Product> items = new List<Product>();
        public MainWindow()
        {

        }

        private void DatabzeTest()
        {
            InitializeComponent();

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


        private void ScreenKeyDown(object sender, KeyEventArgs e)
        {
            string input = null;
            if (readNewBarcode)
            {
                textCleared.Content = "";
                textZCtecky.Content = "";
                readNewBarcode = false;
            }

            switch (e.Key)
            {
                case Key.Enter:
                    readNewBarcode = true;
                    AddItemToBoughtList(textCleared.Content.ToString());
                    //textZCtecky.Content = "Key: Enter";
                    //textAscii.Content = "";
                    break;

                case Key.Space:
                    //textZCtecky.Content = "Key: space";
                    //textAscii.Content = "";
                    break;
                
                default:
                    input = e.Key.ToString(); //convert keyEvent to string

                    if (input.Length > 1)
                    {
                        if (input.StartsWith("NumPad")) //When numpad is pressed
                            input = input.Remove(0, 6); //Remove NumPad part

                        else if (input.StartsWith("D")) //Remove D part
                            input = input.Remove(0, 1);
                    }
                    break;
            }

            textZCtecky.Content += e.Key.ToString();
            textCleared.Content += input;

        }

        private void AddItemToBoughtList(string item)
        {
            boughtItems.Items.Insert(0,new Product() { Barcode = item, Name = "Jablko jarní",Count = 1, Price = 10f });
        }

        private void BoughtProductsList(object sender, SizeChangedEventArgs e)
        {
            GridView gView = boughtItems.View as GridView; //get ListView from XAML

            var workingWidth = boughtItems.ActualWidth - SystemParameters.VerticalScrollBarWidth; //get width of ListView element + take into account vertical scrollbar
            var col1 = 0.40; // 0.40 -> 40%
            var col2 = 0.10;
            var col3 = 0.20;
            var col4 = 0.30;

            gView.Columns[0].Width = workingWidth * col1;
            gView.Columns[1].Width = workingWidth * col2;
            gView.Columns[2].Width = workingWidth * col3;
            gView.Columns[3].Width = workingWidth * col4;
        }

        private int GetASCII(KeyEventArgs e)
        {
            char parsedCharacter = ' ';
            if (Char.TryParse(e.Key.ToString(), out parsedCharacter))
                textCleared.Content = (int)parsedCharacter;

            return 0;
        }
    }

    public class Product
    {
        public string Barcode { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public int Count { get; set; }
    }
}

//https://wpf-tutorial.com/listview-control/listview-with-gridview/
