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

namespace pokladnaInitial
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool readNewBarcode = false;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DatabaseConnection.ConnectToDatabase();
        }

        private void ScreenKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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
            boughtItems.Items.Insert(0,new Product() { Barcode = item, Name = "Jablko jarní",Quantity = 1, Price = 10f });
        }

        private void BoughtProductsList(object sender, SizeChangedEventArgs e)
        {
            GridView gView = boughtItems.View as GridView; //get ListView from XAML

            var workingWidth = boughtItems.ActualWidth - SystemParameters.VerticalScrollBarWidth; //get width of ListView element + take into account vertical scrollbar

            gView.Columns[0].Width = workingWidth * 0.25; //barcode 0,25 -> 25% of screen size
            gView.Columns[1].Width = workingWidth * 0.35; //name
            gView.Columns[2].Width = workingWidth * 0.10; //count
            gView.Columns[3].Width = workingWidth * 0.15; //price
            gView.Columns[4].Width = workingWidth * 0.15; //price * count (price total per item type)

        }

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

        }

        private void Bt_MinusOneQuantity_Click(object sender, RoutedEventArgs e)
        {

        }

        private void bt_BuyOrder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void bt_CancelOrder_Click(object sender, RoutedEventArgs e)
        {

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
