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
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data.SQLite;

namespace pokladnaInitial
{
    public partial class MainWindow : Window
    {
        List<BoughtProduct> boughtProducts = new List<BoughtProduct>();
        bool readNewBarcode = false, isInPuchasedMode = false;
        string rawDataInput = "";

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DatabaseConnection.ConnectToDatabase();
        }

        //Parsing the input from barcode reader. With KeyEventArgs the language of keyboard doesnt matter
        private void ScreenKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string input = null;

            if (readNewBarcode)
            {
                textCleared.Content = "";
                rawDataInput = "";
                readNewBarcode = false;
            }

            switch (e.Key)
            {
                case Key.Enter:
                    readNewBarcode = true;
                    if (textCleared.Content.ToString().Length <= 3 && !isInPuchasedMode) //if user input is 3char or shorter it will assume you are inputing the quantity of purchased item
                    {
                        try
                        {
                            EditQuantity(int.Parse(textCleared.Content.ToString()));
                        }
                        catch (Exception)
                        {
                            System.Windows.MessageBox.Show("Vstup nebyl rozpoznán.");
                            textCleared.Content = "";
                        }
                    }
                    else
                    {
                        if (isInPuchasedMode)
                        {
                            try
                            {
                                System.Windows.MessageBox.Show($"Zákazníkovy je potřeba vrátit z částky {float.Parse(textCleared.Content.ToString())} Kč -> {float.Parse(textCleared.Content.ToString()) - GetCompletePrice()} Kč.");

                            }
                            catch (Exception)
                            {
                                System.Windows.MessageBox.Show("Zadali jste špatné hodnoty pro výpočet");
                            }
                        }
                        else
                        {
                            AddItemToBoughtList(textCleared.Content.ToString());
                        }
                    }
                    break;

                case Key.Space:
                    //Not used for now
                    break;
                
                default:
                    input = e.Key.ToString(); //convert keyEvent to string

                    if (input.Length > 1)
                    {
                        if (input.StartsWith("NumPad")) //When numpad is pressed
                            input = input.Remove(0, 6); //Remove NumPad part

                        else if (input.StartsWith("LeftShift"))
                            input = input.Remove(0, 9);

                        else if (input.StartsWith("RightShift"))
                            input = input.Remove(0, 10);

                        else if (input.StartsWith("D"))
                            input = input.Remove(0, 1);
                    }
                    break;
            }
            //Debug
            textCleared.Content += input;

        }

        private void AddItemToBoughtList(string barcode)
        {
            int item = 0;
            try
            {
                var sql = $"SELECT * FROM Products WHERE id = '{barcode}'";
                DatabaseConnection.command = new SQLiteCommand(sql, DatabaseConnection.m_dbConnection);
                SQLiteDataReader reader = DatabaseConnection.command.ExecuteReader();
                while (reader.Read())
                {
                    boughtProducts.Insert(0, new BoughtProduct { Barcode = reader.GetString(0), Name = reader.GetString(1), Price = reader.GetFloat(2), Quantity = 1, TotalPrice = 1 * reader.GetFloat(2)}); //add to list
                    boughtItems.ItemsSource = boughtProducts;
                    currentItemDisplayed.Content = reader.GetString(1);
                    quantityInWareHouse.Content = reader.GetInt32(3).ToString() + " ks";
                    totalPriceLabel.Content = GetCompletePrice().ToString() + " kč";
                    item++;
                }
                if (item <= 0)
                    throw new Exception();
                boughtItems.Items.Refresh();
            }

            catch (Exception)
            {
                System.Windows.MessageBox.Show($"Poslední naskenované zboží se nenachází v databázi!\nKód: "+ barcode);
            }
}

        private void EditQuantity(int quantity)
        {
            if (quantity >= 1)
            {
                //edit latest object
                boughtProducts[0].Quantity = quantity;
                boughtProducts[0].TotalPrice = quantity * boughtProducts[0].Price;
                boughtItems.ItemsSource = boughtProducts;
                boughtItems.Items.Refresh();
                totalPriceLabel.Content = GetCompletePrice().ToString() + " kč";
            }
        }

        private float GetCompletePrice()
        {
            float actualPrice = 0;
            foreach (var item in boughtProducts)
                actualPrice += item.TotalPrice;

            return actualPrice;
        }


        //----------------------------------------------------Buttons-------------------
        private void bt_WarehouseManagement_Click(object sender, RoutedEventArgs e)
        {
            ManageProducts window = new ManageProducts();
            window.ShowDialog();
        }

        private void bt_Help_Click(object sender, RoutedEventArgs e)
        {
            Help window = new Help();
            window.ShowDialog();
        }

        private void bt_PurchaseHistory_Click(object sender, RoutedEventArgs e)
        {
            PurchaseHistory window = new PurchaseHistory();
            window.ShowDialog();
        }
        private void bt_RemoveLatestItem_Click(object sender, RoutedEventArgs e)
        {
            boughtProducts.Remove(boughtProducts[0]);
            boughtItems.ItemsSource = boughtProducts;
            boughtItems.Items.Refresh();
        }

        private void bt_BuyOrder_Click(object sender, RoutedEventArgs e)
        {
            isInPuchasedMode = true;
            howMuchFromCustomer.Visibility = Visibility.Visible;

        }

        private void bt_CancelOrder_Click(object sender, RoutedEventArgs e)
        {
            boughtProducts.Clear();
            boughtItems.Items.Refresh();
            isInPuchasedMode = false;
        }

        //---------------------------------------------------Events-----------------------------
        private void OnSizeChangedSpacing(object sender, SizeChangedEventArgs e)
        {
            GridView gView = boughtItems.View as GridView; //get ListView from XAML

            var workingWidth = boughtItems.ActualWidth - SystemParameters.VerticalScrollBarWidth; //get width of ListView element + take into account vertical scrollbar

            gView.Columns[0].Width = workingWidth * 0.25; //barcode 0,25 -> 25% of screen size
            gView.Columns[1].Width = workingWidth * 0.35; //name
            gView.Columns[2].Width = workingWidth * 0.10; //count
            gView.Columns[3].Width = workingWidth * 0.15; //price
            gView.Columns[4].Width = workingWidth * 0.15; //price * count (price total per item type)
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            string message = "Po ukončení programu se ztratí veškerá neuložená data, chcete program vypnout?";
            string title = "Pozor ukončení programu.";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = System.Windows.Forms.MessageBox.Show(message, title, buttons);

            if (result != System.Windows.Forms.DialogResult.Yes)
                e.Cancel = true;

            DatabaseConnection.m_dbConnection.Close();
        }
    }
}
