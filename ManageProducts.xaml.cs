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
using System.Windows.Shapes;

namespace pokladnaInitial
{
    /// <summary>
    /// Interaction logic for ManageProducts.xaml
    /// </summary>
    public partial class ManageProducts : Window
    {
        public ManageProducts()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridView gView = itemsInWareHouse.View as GridView; //get ListView from XAML

            var workingWidth = itemsInWareHouse.ActualWidth - SystemParameters.VerticalScrollBarWidth; //get width of ListView element + take into account vertical scrollbar
            var col1 = 0.40; // 0.40 -> 40%
            var col2 = 0.10;
            var col3 = 0.10;
            var col4 = 0.30;
            var col5 = 0.10;

            gView.Columns[0].Width = workingWidth * col1; //name
            gView.Columns[1].Width = workingWidth * col2; //count
            gView.Columns[2].Width = workingWidth * col3; //price
            gView.Columns[3].Width = workingWidth * col4; //barcode
            gView.Columns[4].Width = workingWidth * col5; //IsAvailable
        }
    }
}
