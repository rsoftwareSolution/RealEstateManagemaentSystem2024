using Microsoft.Web.WebView2.WinForms;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using RealEstateManagemaentSystem2024.masterForm;
using RealEstateManagemaentSystem2024.reports;
using RealEstateManagemaentSystem2024.Statements;
using RealStateManagementSystem.config;
using RealStateManagementSystem.mainForm;
using RealStateManagementSystem.masterForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealEstateManagemaentSystem2024.mainForm
{
    public partial class Dashboard : Form
    {

        private Database db;
        private string userName;
        private string contact;

        public Dashboard(string userName, string contact)
        {
            InitializeComponent();
            // Assign the passed values to the class-level variables
            this.userName = userName;
            this.contact = contact;
        }

        public Dashboard()
        {
            InitializeComponent();

            webView21.Source = new Uri(@"E:\appu bcs\RealEstateManagemaentSystem2024\Html\BackgroundImage.html");
            Console.WriteLine("WebView2 page loaded");

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void Masters_Opening(object sender, CancelEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
           
        }

        

        private void panel3_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {
        }

        private void button14_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
           
        }

        private void webView21_Click(object sender, EventArgs e)
        {

        }

        private async void Dashboard_Load(object sender, EventArgs e)
        {
            try
            {
                await webView21.EnsureCoreWebView2Async();

                if (webView21.CoreWebView2 != null)
                {
                    webView21.CoreWebView2.WebMessageReceived += (msgSender, msgArgs) =>
                    {
                        var message = msgArgs.WebMessageAsJson;
                        try
                        {
                            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

                            if (data != null && data.ContainsKey("action") && data.ContainsKey("formName"))
                            {

                                if (data["action"] == "openForm")
                                {
                                    string formName = data["formName"];
                                    switch (formName)
                                    {
                                        case "CustomerMaster":
                                            var customerMasterForm = new CustomerMaster();
                                            customerMasterForm.Show();
                                            break;

                                        case "EnquiryMaster":
                                            var enquiryMasterForm = new EnquiryMaster();
                                            enquiryMasterForm.Show();
                                            break;

                                        case "BuildingMaster":
                                            var buildingMasterForm = new BuildingMaster();
                                            buildingMasterForm.Show();
                                            break;

                                        case "FlatMaster":
                                            var flatMasterForm = new FlatMaster();
                                            flatMasterForm.Show();
                                            break;

                                        case "PaymentMaster":
                                            var paymentMasterForm = new PaymentMaster();
                                            paymentMasterForm.Show();
                                            break;

                                        case "CancellationMaster":
                                            var cancellationMasterForm = new CancellationMaster();
                                            cancellationMasterForm.Show();
                                            break;

                                        case "UserMaster":
                                            var userMasterForm = new UserMaster();
                                            userMasterForm.Show();
                                            break;

                                        case "BookingInvoice":
                                           // var bookingInvoiceForm = new BookingMaster();
                                           // bookingInvoiceForm.Show();
                                            break;

                                        case "SaleInvoice":
                                            var saleInvoiceForm = new SaleInvoice();
                                            saleInvoiceForm.Show();
                                            break;

                                        case "DownloadReports":
                                            var downloadReportsForm = new DownloadReports();
                                            downloadReportsForm.Show();
                                            break;

                                        case "AccountStatements":
                                            var accountStatementsForm = new AccountStatements();
                                            accountStatementsForm.Show();
                                            break;

                                        case "Settings":
                                            var settingsForm = new Settings();
                                            MessageBox.Show($"The form '{formName}' is not implemented.", "Form Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                            break;

                                        case "Exit":
                                            Application.Exit();
                                            break;

                                        default:
                                            MessageBox.Show($"The form '{formName}' is not implemented.", "Form Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            break;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error deserializing message: " + ex.Message);
                        }
                    };
                }
                else
                {
                    Console.WriteLine("CoreWebView2 initialization failed. CoreWebView2 is null.");
                }

                // Load the HTML page inside WebView2
                webView21.Source = new Uri(@"file:///E:/appu bcs/RealEstateManagemaentSystem2024/Html/BackgroundImage.html");
                Console.WriteLine("WebView2 page loaded");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing WebView2: " + ex.Message);
                Console.WriteLine("Error initializing WebView2: " + ex.Message);
            }
        }
    }
}
