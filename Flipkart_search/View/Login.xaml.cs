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
using System.Net;
using System.Net.Mail;


namespace Flipkart_search.View
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Loginmail_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            PlaceholderText.Visibility = string.IsNullOrEmpty(Login_mail.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private string generatedOtp;

        private void OTP_Click(object sender, RoutedEventArgs e)
        {
            string recipientEmail = Login_mail.Text.Trim();

            if (string.IsNullOrEmpty(recipientEmail))
            {
                MessageBox.Show("Please enter your email ID before requesting OTP.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

           
            Random rand = new Random();
            string otp = rand.Next(100000, 999999).ToString();

           
            SendOtpEmail(recipientEmail, otp);
        }

        private void SendOtpEmail(string recipientEmail, string otp)
        {
            try
            {
                string senderEmail = "subhalaxmipatro@globussoft.in"; 
                //string senderPassword = "ckgp pcoh syav efps";
                string senderPassword = "caxu cqxb setv nncu";

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(senderEmail);
                mail.To.Add(recipientEmail);
                mail.Subject = "Your Flipkart OTP";
                mail.Body = $"Your OTP is: {otp}\nThis OTP is valid for 5 minutes.";
                mail.IsBodyHtml = false;

                SmtpClient smtp = new SmtpClient("mail.globussoft.in", 465);
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(mail);


                MessageBox.Show($"✅ OTP sent successfully to {recipientEmail}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Failed to send OTP:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void VerifyOtp_Click(object sender, RoutedEventArgs e)
        {
            if (OtpInput.Text.Trim() == generatedOtp)
                MessageBox.Show("OTP Verified Successfully!");
            else
                MessageBox.Show("Invalid OTP, please try again.");
        }
    }
}
