using Flipkart_search.Model;
using Flipkart_search.View;
using Flipkart_search.ViewModel;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Flipkart_search
{
    public partial class MainWindow : Window
    {
        private bool isMouseOverMenu = false;
        private bool isMenuManuallyOpening = false;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

            Loaded += (s, e) =>
            {
                if (LoginMenu != null)
                {
                    LoginMenu.MouseEnter += (s2, e2) => isMouseOverMenu = true;
                    LoginMenu.MouseLeave += (s3, e3) =>
                    {
                        isMouseOverMenu = false;
                        LoginButton.ContextMenu.IsOpen = false;
                    };
                }
            };

        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PlaceholderText.Visibility = string.IsNullOrEmpty(SearchBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void MoreButton_click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.ContextMenu != null)
            {
                btn.ContextMenu.PlacementTarget = btn;
                btn.ContextMenu.IsOpen = true;
            }
        }


        private void LoginButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (LoginButton?.ContextMenu == null)
                return;

           
            if (!LoginButton.ContextMenu.IsOpen && !isMenuManuallyOpening)
            {
                isMenuManuallyOpening = true;

                LoginButton.ContextMenu.PlacementTarget = LoginButton;
                LoginButton.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                LoginButton.ContextMenu.IsOpen = true;

              
                Dispatcher.InvokeAsync(() => isMenuManuallyOpening = false,
                    System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            }
        }

        private void LoginButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Task.Delay(200).ContinueWith(_ =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (!isMouseOverMenu && LoginButton?.ContextMenu != null)
                        LoginButton.ContextMenu.IsOpen = false;
                });
            });
        }


        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            
            LoginButton.ContextMenu.IsOpen = false;

            // Show Login wndw 
            var loginWindow = new Login();
            loginWindow.Owner = this; 
            loginWindow.ShowDialog();
        }
    }
}
