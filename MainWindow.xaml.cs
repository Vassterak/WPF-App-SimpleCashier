﻿using System;
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
    }

    public class Product
    {
        public string Barcode { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public int Count { get; set; }
    }
}
