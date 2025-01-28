using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace RealEstateManagemaentSystem2024.MasterForm
{
    public partial class BookingRegister : Form
    {

        Database db = new Database();
        
        public BookingRegister()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = true; // Optional: Disable minimize button
            this.AutoSize = false;

            // Explicitly set the size
            this.Size = new Size(1173, 569);

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click_1(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label4_Click_1(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void BookingRegister_Load(object sender, EventArgs e)
        {

            DataTable buildingData = db.ExecuteQuery("SELECT building_or_project_name, building_location_pincode FROM building_details");
            // Configure the chart
            chart1.Series.Clear(); // Clear existing series
            Series series = new Series
            {
                Name = "Buildings",
                IsValueShownAsLabel = true, // Show labels on bars
                ChartType = SeriesChartType.Bar // Set the type of graph
            };
            chart1.Series.Add(series);

            // Add data points to the series
            foreach (DataRow row in buildingData.Rows)
            {
                string buildingName = row["building_or_project_name"].ToString();
                int pincode = int.Parse(row["building_location_pincode"].ToString());

                series.Points.AddXY(buildingName, pincode); // X = Building name, Y = Pincode
            }

            // Set chart title and axis labels
            chart1.Titles.Add("Building Details Chart");
            chart1.ChartAreas[0].AxisX.Title = "Building Name";
            chart1.ChartAreas[0].AxisY.Title = "Pincode";
        }
    }
}
