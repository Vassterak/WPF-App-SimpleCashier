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
    /// Interaction logic for PurchaseHistory.xaml
    /// </summary>
    public partial class PurchaseHistory : Window
    {
        double[] colSize = new double[5];

        public PurchaseHistory()
        {
            InitializeComponent();
            ColumnsSizes();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var sql = "SELECT * FROM HistoryOfPurchases"; //get all products to Listview
            DatabaseConnection.command = new SQLiteCommand(sql, DatabaseConnection.m_dbConnection);
            SQLiteDataReader reader = DatabaseConnection.command.ExecuteReader();
            while (reader.Read())
            {
                receiptHistory.Items.Insert(0, new HistoryPurchase { OrderID = reader.GetString(0), ProductID = reader.GetString(1), Name = reader.GetString(2), PriceTotal = reader.GetFloat(3), Quantity = reader.GetInt32(4), Date = reader.GetString(5) }); //add to list
                //Trace.WriteLine($"Barcode: {reader.GetString(0)} | Name = { reader.GetString(1)} | Price = {reader.GetFloat(2)} | Quantity = {reader.GetInt32(3)} | IsAvailable = {reader.GetByte(4)}"); //show in output console
            }
        }

        private void ColumnsSizes()
        {
            colSize[0] = 0.25;
            colSize[1] = 0.15;
            colSize[2] = 0.25;
            colSize[3] = 0.05;
            colSize[4] = 0.10;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridView gView = receiptHistory.View as GridView; //get ListView from XAML
            var listWidth = receiptHistory.ActualWidth - SystemParameters.VerticalScrollBarWidth; //get width of ListView element + take into account vertical scrollbar
            var groubBoxWidth = receiptHistory.ActualWidth - 80;

            gView.Columns[0].Width = listWidth * colSize[0]; //barcode
            gView.Columns[1].Width = listWidth * colSize[1]; //name
            gView.Columns[2].Width = listWidth * colSize[2]; //count
            gView.Columns[3].Width = listWidth * colSize[3]; //price
            gView.Columns[4].Width = listWidth * colSize[4]; //IsAvailable
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            receiptHistory.Items.Clear();

            var sql = $"SELECT * FROM HistoryOfPurchases WHERE order_id = '{tb_lookForOrder.Text}'"; //get all products to Listview
            DatabaseConnection.command = new SQLiteCommand(sql, DatabaseConnection.m_dbConnection);
            SQLiteDataReader reader = DatabaseConnection.command.ExecuteReader();
            while (reader.Read())
            {
                receiptHistory.Items.Insert(0, new HistoryPurchase { OrderID = reader.GetString(0), ProductID = reader.GetString(1), Name = reader.GetString(2), PriceTotal = reader.GetFloat(3), Quantity = reader.GetInt32(4), Date = reader.GetString(5) }); //add to list
                //Trace.WriteLine($"Barcode: {reader.GetString(0)} | Name = { reader.GetString(1)} | Price = {reader.GetFloat(2)} | Quantity = {reader.GetInt32(3)} | IsAvailable = {reader.GetByte(4)}"); //show in output console
            }
        }
    }
}
