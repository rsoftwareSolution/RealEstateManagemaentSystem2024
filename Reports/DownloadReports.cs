using CrystalDecisions.CrystalReports.Engine;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace RealEstateManagemaentSystem2024.reports
{
    public partial class DownloadReports : Form
    {
        Database db = new Database();
        private string saleId;

        public DownloadReports(string saleId)
        {
            InitializeComponent();
            this.saleId = saleId;
        }

        private void DownloadReports_Load(object sender, EventArgs e)
        {
            LoadSaleInvoiceReport();
        }

        private void LoadSaleInvoiceReport()
        {
            try
            {
                // Fetch sale details from database
                string query = "SELECT * FROM saleinvoice_details WHERE sale_id = @saleId";
                DataTable dt = db.ExecuteQuery(query, new MySqlParameter[]
                {
                    new MySqlParameter("@saleId", MySqlDbType.VarChar) { Value = saleId }
                });

                // Check if data exists
                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No data found for the selected sale.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Load Crystal Report
                ReportDocument rpt = new ReportDocument();
                string reportPath = @"C:\Users\Raghvendra\OneDrive\Desktop\RealEstateManagemaentSystem2024\CrystalReport\SaleInvoice.rpt";


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
                MessageBox.Show($"Error generating invoice: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }
    }
}
