using System;
using System.IO;
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
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace pokladnaInitial
{
    public partial class MainWindow : Window
    {
        List<BoughtProduct> boughtProducts = new List<BoughtProduct>();
        StreamWriter sw;
        ButtonAutomationPeer peerBuy, peerDelLast;
        IInvokeProvider invokeProv;
        bool readNewBarcode = false, isInPuchasedMode = false;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DatabaseConnection.ConnectToDatabase();
            peerBuy = new ButtonAutomationPeer(bt_BuyOrder);
            peerDelLast = new ButtonAutomationPeer(bt_RemoveLatestItem);
        }

        //Parsing the input from barcode reader. With KeyEventArgs the language of keyboard doesnt matter
        private void ScreenKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string input = null;

            if (readNewBarcode)
            {
                textCleared.Content = "";
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
                                UpdateQuantityInDB();
                                CreateHistoryRecord();
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
                    invokeProv = peerBuy.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    invokeProv.Invoke();
                    break;

                case Key.Delete:
                    invokeProv = peerDelLast.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    invokeProv.Invoke();
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
        private void UpdateQuantityInDB()
        {
            var sql = "";
            SQLiteDataReader reader;
            int currentQuantity = 0;
            foreach (var item in boughtProducts)
            {
                sql = $"SELECT * FROM Products WHERE id = '{item.Barcode}'";
                DatabaseConnection.command = new SQLiteCommand(sql, DatabaseConnection.m_dbConnection);
                reader = DatabaseConnection.command.ExecuteReader();
                while (reader.Read())
                {
                    currentQuantity = reader.GetInt32(3);
                }

                sql = $"UPDATE Products SET quantity = {currentQuantity - item.Quantity} WHERE id = '{item.Barcode}'";
                DatabaseConnection.command = new SQLiteCommand(sql, DatabaseConnection.m_dbConnection);
                reader = DatabaseConnection.command.ExecuteReader();
            }
            Trace.WriteLine("Položky byli úspěšně odečteny ze skladu");

        }

        private void CreateHistoryRecord()
        {
            var sql = "";
            SQLiteDataReader reader;
            Guid orderID = Guid.NewGuid();
            string today = DateTime.Now.ToString("MM.dd.yyyy HH-mm-ss");

            //create new receipe file
            string path = @"c:\Účtenky\" + today + ".txt";
            //string path = @"c:\Účtenky\MyTest.txt";
            using (sw = File.CreateText(path))
            {
                sw.WriteLine("");
            }
            //initial write
            sw = new StreamWriter(path);
            sw.WriteLine("==========Obchod s.r.o.==========");
            sw.WriteLine("---------------------------------");
            sw.WriteLine("  Název zboží  |  Cena za kus  |  počet kusů  |  Cena za všchny kusy");

            foreach (var item in boughtProducts)
            {
                CreateReceipe(item, today, sw);
                sql = $"INSERT INTO HistoryOfPurchases (order_id, product_id, title, price, quantity, timeOfPurchase) values ('{orderID}', '{item.Barcode}', '{item.Name}', {1}, {item.Quantity}, '{today}')";
                DatabaseConnection.command = new SQLiteCommand(sql, DatabaseConnection.m_dbConnection);
                reader = DatabaseConnection.command.ExecuteReader();
            }
            sw.WriteLine("---------------------------------");
            sw.WriteLine($"Čas nákupu: {DateTime.Now.ToString("MM.dd.yyyy HH:mm:ss")}");
            sw.WriteLine($"Celková cena nákupu: {GetCompletePrice()} Kč");
            Trace.WriteLine("Vyvořena historie nákupu");
            System.Windows.MessageBox.Show("Účtenka byla vytvořena");
            sw.Close();
        }

        private void CreateReceipe(BoughtProduct product, string time, StreamWriter sw)
        {
            sw.WriteLine($"{product.Name} - {product.Price} kč - {product.Quantity} ks - {product.TotalPrice} kč celkem");
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
            howMuchFromCustomer.Visibility = Visibility.Hidden;
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
