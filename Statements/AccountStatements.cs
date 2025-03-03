using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealEstateManagemaentSystem2024.Statements
{
    public partial class AccountStatements : Form
    {
        public AccountStatements()
        {
            InitializeComponent();
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                switch (comboBox1.SelectedItem.ToString())
                {
                    case "Customer Statement":
                        LoadReport(@"C:\Users\Raghvendra\OneDrive\Desktop\RealEstateManagemaentSystem2024\CrystalReport\CustomerReport1.rpt");
                        break;

                    case "Building Statement":
                        LoadReport(@"C:\Users\Raghvendra\OneDrive\Desktop\RealEstateManagemaentSystem2024\CrystalReport\BuildingReport.rpt");
                        break;

                    default:
                        MessageBox.Show("Invalid selection!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                }
            }
        }

        private void LoadReport(string reportPath)
        {
            try
            {
                // Create Report Document
                ReportDocument reportDocument = new ReportDocument();

                // Load the Crystal Report file (.rpt)
                reportDocument.Load(reportPath);

                // Assign Report to CrystalReportViewer
                crystalReportViewer1.ReportSource = reportDocument;
                crystalReportViewer1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

