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
        double[] colSize = new double[5];
        bool isWindowLoaded = false;

        public ManageProducts()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            ColumnsSizes();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var sql = "SELECT * FROM Products"; //get all products to Listview
            DatabaseConnection.command = new SQLiteCommand(sql, DatabaseConnection.m_dbConnection);
            SQLiteDataReader reader = DatabaseConnection.command.ExecuteReader();
            while (reader.Read())
            {
                itemsInWareHouse.Items.Insert(0,new Product { Barcode = reader.GetString(0), Name = reader.GetString(1), Price = reader.GetFloat(2), Quantity = reader.GetInt32(3), IsAvailable = Convert.ToBoolean(reader.GetByte(4)) }); //add to list
                Trace.WriteLine($"Barcode: {reader.GetString(0)} | Name = { reader.GetString(1)} | Price = {reader.GetFloat(2)} | Quantity = {reader.GetInt32(3)} | IsAvailable = {reader.GetByte(4)}"); //show in output console
            }
            
            //itemsInWareHouse.ItemsSource = products; //add list to listviewer
            isWindowLoaded = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {

        }

        private void ColumnsSizes()
        {
            colSize[0] = 0.25;
            colSize[1] = 0.35;
            colSize[2] = 0.15;
            colSize[3] = colSize[2];
            colSize[4] = 0.10;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridView gView = itemsInWareHouse.View as GridView; //get ListView from XAML
            var listWidth = itemsInWareHouse.ActualWidth - SystemParameters.VerticalScrollBarWidth; //get width of ListView element + take into account vertical scrollbar
            var groubBoxWidth = AddNewProductBox.ActualWidth - 80;

            gView.Columns[0].Width = listWidth * colSize[0]; //barcode
            gView.Columns[1].Width = listWidth * colSize[1]; //name
            gView.Columns[2].Width = listWidth * colSize[2]; //count
            gView.Columns[3].Width = listWidth * colSize[3]; //price
            gView.Columns[4].Width = listWidth * colSize[4]; //IsAvailable

            sp_barcode.Width = groubBoxWidth * colSize[0];
            sp_name.Width = groubBoxWidth * colSize[1];
            sp_quantity.Width = groubBoxWidth * colSize[2];
            sp_price.Width = groubBoxWidth * colSize[3];
            sp_isAvailable.Width = groubBoxWidth * colSize[4];
        }
    }
}
