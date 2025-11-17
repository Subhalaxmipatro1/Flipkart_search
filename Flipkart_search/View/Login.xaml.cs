using System;
using System.Windows;
using System.Windows.Controls;
using Flipkart_search.ViewModel;

namespace Flipkart_search.View
{
    public partial class Login : Window
    {
        private readonly LoginService _loginService = new LoginService();
        private string _simulatedOtp;
        private string _requestId;
        private string _otpRecipientId;

        public Login()
        {
            InitializeComponent();
            ResetLoginState();
        }

        private void Loginmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            PlaceholderText.Visibility =
                string.IsNullOrEmpty(Login_mail.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        // -------------------- REQUEST OTP --------------------
        private async void OTP_Click(object sender, RoutedEventArgs e)
        {
            _otpRecipientId = Login_mail.Text.Trim();

            if (string.IsNullOrEmpty(_otpRecipientId))
            {
                MessageBox.Show("Enter valid email or mobile number.");
                return;
            }

            RequestOtpButton.IsEnabled = false;
            RequestOtpButton.Content = "Sending OTP...";

            try
            {
                // Step 1: Simulated Request ID
                _requestId = await _loginService.RequestOtp(_otpRecipientId);

                // Step 2: Generate dummy OTP
                Random rand = new Random();
                _simulatedOtp = rand.Next(100000, 999999).ToString();

                MessageBox.Show($"Simulated OTP sent!\nYour OTP is: {_simulatedOtp}",
                    "OTP", MessageBoxButton.OK, MessageBoxImage.Information);

                // UI Update
                OtpPanel.Visibility = Visibility.Visible;
                Login_mail.IsReadOnly = true;
                OtpInput.Text = "";
                OtpInput.Focus();

                RequestOtpButton.Content = "Resend OTP";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                RequestOtpButton.IsEnabled = true;
            }
        }

        // -------------------- VERIFY OTP --------------------
        private async void VerifyOtp_Click(object sender, RoutedEventArgs e)
        {
            string userOtp = OtpInput.Text.Trim();

            if (userOtp != _simulatedOtp)
            {
                MessageBox.Show("Invalid OTP!");
                return;
            }

            VerifyOtpButton.IsEnabled = false;
            VerifyOtpButton.Content = "Verifying...";

            try
            {
                // Step 3: Generate Dummy Token
                string token = await _loginService.LoginAndGetToken(_otpRecipientId, _requestId, userOtp);

                // Step 4: Extract Dummy User ID
                string userId = await _loginService.FetchUserIdFromProfile(token);

                MessageBox.Show(
                    $"LOGIN SUCCESS!\n\nToken: {token.Substring(0, 25)}...\nUser ID: {userId}",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information
                );

                ResetLoginState();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Verification failed:\n" + ex.Message);
            }
            finally
            {
                VerifyOtpButton.IsEnabled = true;
                VerifyOtpButton.Content = "Verify OTP";
            }
        }

        // -------------------- RESET UI --------------------
        private void ResetLoginState()
        {
            Login_mail.IsReadOnly = false;
            Login_mail.Text = "";

            _simulatedOtp = null;
            _requestId = null;
            _otpRecipientId = null;

            RequestOtpButton.Content = "Request OTP";
            VerifyOtpButton.Content = "Verify OTP";

            OtpPanel.Visibility = Visibility.Collapsed;
        }
    }
}
