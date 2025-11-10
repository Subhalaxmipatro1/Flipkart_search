using Flipkart_search.Model;
using Flipkart_search.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Flipkart_search
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

        }

        private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            PlaceholderText.Visibility = string.IsNullOrEmpty(SearchBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }


    }
}
