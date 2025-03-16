using CrystalDecisions.CrystalReports.Engine;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace RealEstateManagemaentSystem2024.ParameterReport
{
    public partial class SaleInvoiceSearchReport : Form
    {
        Database db = new Database();

        public SaleInvoiceSearchReport()
        {
            InitializeComponent();
        }

        private void BookingSearchReport_Load(object sender, EventArgs e)
        {
            // You can initialize anything here if required.
        }

        private void LoadBookingReport(DateTime date)
        {
            try
            {
                // Fetch sale details from database
                string query = "SELECT * FROM quatation_details WHERE DATE(quatation_date) = @date";
                DataTable dt = db.ExecuteQuery(query, new MySqlParameter[]
                {
                    new MySqlParameter("@date", MySqlDbType.Date) { Value = date.Date } // ✅ FIXED: Use MySqlDbType.Date
                });

                // Check if data exists
                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No data found for the selected booking date.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ReportDocument rpt = new ReportDocument();
                string reportPath = @"C:\Users\Raghvendra\OneDrive\Desktop\RealEstateManagemaentSystem2024\CrystalReport\QuatationReport.rpt";

                // Check if the report file exists
                if (!File.Exists(reportPath))
                {
                    MessageBox.Show($"Report file not found: {reportPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Load the report and set data source
                rpt.Load(reportPath);
                rpt.SetDataSource(dt);

                // Optional: Set database credentials dynamically if needed
                // rpt.SetDatabaseLogon("your_db_username", "your_db_password", "your_server", "your_database");

                // Display the report
                crystalReportViewer1.ReportSource = rpt;
                crystalReportViewer1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating booking report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadBookingReport(dateTimePicker1.Value);
        }

        private void SaleInvoiceSearchReport_Load(object sender, EventArgs e)
        {

        }
    }
}
