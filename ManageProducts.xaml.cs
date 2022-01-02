using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
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
        double[] colSize = new double[5];
        bool isInEditMode = false;
        Product EditSelectedItem;

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

        private void bt_editItem_Click(object sender, RoutedEventArgs e)
        {
            //UI changes
            bt_editItem.Visibility = Visibility.Hidden;
            bt_addNewItem.Visibility = Visibility.Visible;
            AddNewProductBox.Header = "Uprav existující položku";
            bt_action.Content = "Uložit upravení položky";
            tb_barcode.IsReadOnly = true;

            isInEditMode = true;

        }

        private void bt_addNewItem_Click(object sender, RoutedEventArgs e)
        {
            //UI changes
            bt_editItem.Visibility = Visibility.Visible;
            bt_addNewItem.Visibility = Visibility.Hidden;
            AddNewProductBox.Header = "Přidej novou položku do skladu";
            bt_action.Content = "Uložit novou položku";
            tb_barcode.IsReadOnly = false;


            isInEditMode = false;
        }

        private void bt_action_Click(object sender, RoutedEventArgs e)
        {
            int quantity, isAvailable;
            float price;

            if (isInEditMode && EditSelectedItem != null)
            {
                try
                {
                    //input protection against SQL injection should be implemented :) 
                    quantity = int.Parse(tb_quantity.Text);
                    float.TryParse(tb_price.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out price); //a better fix
                    isAvailable = (comboxBox_isAvailable.Text.Trim().ToLower() == "ano") ? 1 : 0;

                    if (InsertIntoDatabase($"UPDATE Products SET title = '{tb_name.Text}', price = @price, quantity = {quantity}, isAvailable = {isAvailable} WHERE id = '{tb_barcode.Text}'", price))
                        MessageBox.Show($"Položka s názvem {tb_name.Text}. Byla úspěšně aktualizována");
                }
                catch (Exception)
                {
                    MessageBox.Show($"Došlo k chybě při pokusu o uložení nového produktu, prosím zkontrolujte si zda vaše vstupy jsou ve správném formátu.\nVstupy: {tb_barcode.Text}, {tb_name.Text}, {tb_price.Text}, {tb_quantity.Text}, {comboxBox_isAvailable.Text}");
                }
            }

            else
            {
                try
                {
                    //input protection against SQL injection should be implemented :) 
                    quantity = int.Parse(tb_quantity.Text);
                    float.TryParse(tb_price.Text.Replace(',','.'), NumberStyles.Any, CultureInfo.InvariantCulture, out price); //a better fix
                    isAvailable = (comboxBox_isAvailable.Text.Trim().ToLower() == "ano") ? 1 : 0;   

                    if (InsertIntoDatabase($"INSERT INTO Products (id, title, price, quantity, isAvailable) values ('{tb_barcode.Text}', '{tb_name.Text}', @price, {quantity}, {isAvailable})", price))
                        MessageBox.Show($"Položka s názvem {tb_name.Text}. Byla úspěšně přidána");
                }
                catch (Exception)
                {
                    MessageBox.Show($"Došlo k chybě při pokusu o uložení nového produktu, prosím zkontrolujte si zda vaše vstupy jsou ve správném formátu.\nVstupy: {tb_barcode.Text}, {tb_name.Text}, {tb_price.Text}, {tb_quantity.Text}, {comboxBox_isAvailable.Text}");
                }
            }
        }

        private bool InsertIntoDatabase(string sql, float price)
        {
            DatabaseConnection.command.Parameters.AddWithValue("@price", price);
            DatabaseConnection.command.CommandText = sql;
            //DatabaseConnection.command = new SQLiteCommand(sql, DatabaseConnection.m_dbConnection); removing teacher's bad approach
            try
            {
                DatabaseConnection.command.ExecuteNonQuery();
                return true;
            }
            catch (Exception)
            {

                Trace.WriteLine("Nastala chyba při vykonávání zápisu do databáze");
                MessageBox.Show("Nastala chyba při vykonávání zápisu do databáze\nKód: " + sql);
                return false;
            }
        }

        private void itemsInWareHouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInEditMode) //get selected item in list only when edit mode is enabled
            {
                EditSelectedItem = (Product)itemsInWareHouse.SelectedItem;

                if (EditSelectedItem != null)
                {
                    //MessageBox.Show(EditSelectedItem.Name);
                    tb_barcode.Text = EditSelectedItem.Barcode;
                    tb_name.Text = EditSelectedItem.Name;
                    tb_quantity.Text = EditSelectedItem.Quantity.ToString();
                    tb_price.Text = EditSelectedItem?.Price.ToString();
                    comboxBox_isAvailable.SelectedIndex = (EditSelectedItem.IsAvailable == true) ? 0 : 1;
                }
            }
        }


        //A dumb way how to do it, but I do not have a time to find a better solution that works as well as this
        private void tb_barcode_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void tb_name_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void tb_quantity_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void tb_price_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void tb_price_GotMouseCapture(object sender, MouseEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void tb_quantity_GotMouseCapture(object sender, MouseEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void tb_name_GotMouseCapture(object sender, MouseEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void tb_barcode_GotMouseCapture(object sender, MouseEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }
    }
}
