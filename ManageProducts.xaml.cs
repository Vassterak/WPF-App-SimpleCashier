using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace pokladnaInitial
{
    /// <summary>
    /// Interaction logic for ManageProducts.xaml
    /// </summary>
    public partial class ManageProducts : Window
    {
        List<Product> products;

        public ManageProducts()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            products = new List<Product>();

            var sql = "SELECT * FROM Products";
            DatabaseConnection.command = new SQLiteCommand(sql, DatabaseConnection.m_dbConnection);
            SQLiteDataReader reader = DatabaseConnection.command.ExecuteReader();
            while (reader.Read())
            {
                products.Add(new Product { Barcode = reader.GetString(0), Name = reader.GetString(1), Price = reader.GetFloat(2), Quantity = reader.GetInt32(3), IsAvailable = Convert.ToBoolean(reader.GetByte(4)) });
                Trace.WriteLine($"Barcode: {reader.GetString(0)} | Name = { reader.GetString(1)} | Price = {reader.GetFloat(2)} | Quantity = {reader.GetInt32(3)} | IsAvailable = {reader.GetByte(4)}");
            }
            itemsInWareHouse.ItemsSource = products;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridView gView = itemsInWareHouse.View as GridView; //get ListView from XAML

            var workingWidth = itemsInWareHouse.ActualWidth - SystemParameters.VerticalScrollBarWidth; //get width of ListView element + take into account vertical scrollbar
            var col1 = 0.40; // 0.40 -> 40%
            var col2 = 0.15;
            var col3 = 0.10;
            var col4 = 0.20;
            var col5 = 0.15;

            gView.Columns[0].Width = workingWidth * col1; //name
            gView.Columns[1].Width = workingWidth * col2; //count
            gView.Columns[2].Width = workingWidth * col3; //price
            gView.Columns[3].Width = workingWidth * col4; //barcode
            gView.Columns[4].Width = workingWidth * col5; //IsAvailable
        }
    }
}
