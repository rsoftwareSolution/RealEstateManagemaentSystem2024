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

namespace RealEstateManagemaentSystem2024.Registers
{
    public partial class BookingRegister : Form
    {

        Database db = new Database();

        public BookingRegister()
        {
            InitializeComponent();
        }

        private DataTable GetBookingData()
        {
            string query = "SELECT flat_type_name, COUNT(*) as count FROM quatation_details GROUP BY flat_type_name";
            DataTable result = db.ExecuteQuery(query); // Assuming db.ExecuteQuery() returns DataTable

            // Check if data is returned
            if (result.Rows.Count == 0)
            {
                MessageBox.Show("No data found for chart.");
            }

            return result;
        }

        private void LoadQuotationChart()
        {
            // Clear previous data to prevent duplicates
            bookingPieChart.Series.Clear();

            // Create a new series
            Series series = new Series("Flat Types")
            {
                ChartType = SeriesChartType.Pie
            };

            // Fetch data from the database
            DataTable dt = GetBookingData();

            if (dt.Rows.Count > 0) // Check if data exists
            {
                foreach (DataRow row in dt.Rows)
                {
                    string flatType = row["flat_type_name"].ToString();
                    double count = 0;

                    if (row["count"] != DBNull.Value && double.TryParse(row["count"].ToString(), out double tempCount))
                    {
                        count = tempCount;
                    }

                    // Log data for debugging
                    Console.WriteLine($"Flat Type: {flatType}, Count: {count}");

                    if (count > 0)
                    {
                        // Add data point to series
                        series.Points.AddXY(flatType, count);
                    }
                }

                // Add the series to the chart
                bookingPieChart.Series.Add(series);
                bookingPieChart.Invalidate(); // Force redraw
            }
            else
            {
                MessageBox.Show("No data found for the chart.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BookingRegister_Load(object sender, EventArgs e)
        {
            LoadQuotationChart();

            // Set DataGridView header styles
            bookingDataGrid.EnableHeadersVisualStyles = false;
            bookingDataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.MintCream;
            bookingDataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            bookingDataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 10, FontStyle.Regular);
            bookingDataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Ensure columns are defined
            InitializeBookingGrid();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void InitializeBookingGrid()
        {
            // Clear existing columns to avoid duplication
            bookingDataGrid.Columns.Clear();

            // Disable auto column generation
            bookingDataGrid.AutoGenerateColumns = false;

            // Set DataGridView Header Height
            bookingDataGrid.ColumnHeadersHeight = 35; // Set header row height
            bookingDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Set DataGridView row height
            bookingDataGrid.RowTemplate.Height = 25; // Adjust row height for better visibility

            // Define columns with custom widths
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "BookingID", HeaderText = "Booking ID", DataPropertyName = "booking_id", Width = 80 });
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "BookingDate", HeaderText = "Booking Date", DataPropertyName = "booking_date", Width = 100 });
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "CustomerName", HeaderText = "Customer Name", DataPropertyName = "cust_name", Width = 150 });
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "CustomerContact", HeaderText = "Customer Contact", DataPropertyName = "cust_contact", Width = 120 });
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "BuildingName", HeaderText = "Building/Project Name", DataPropertyName = "building_or_project_name", Width = 180 });
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "FlatType", HeaderText = "Flat Type", DataPropertyName = "flat_type", Width = 100 });
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "VehicleName", HeaderText = "Vehicle Name", DataPropertyName = "vehicle_name", Width = 120 });
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "ParkingCharges", HeaderText = "Parking Charges", DataPropertyName = "parking_charges", Width = 130 });
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "IGST", HeaderText = "IGST", DataPropertyName = "igst", Width = 100 });
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "CGST", HeaderText = "CGST", DataPropertyName = "cgst", Width = 100 });
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "SGST", HeaderText = "SGST", DataPropertyName = "sgst", Width = 100 });
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "SubTotal", HeaderText = "Sub Total", DataPropertyName = "sub_total", Width = 120 });
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "TotalAmount", HeaderText = "Total Amount", DataPropertyName = "total_amount", Width = 120 });
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "PaidAmount", HeaderText = "Paid Amount", DataPropertyName = "paid_amount", Width = 120 });
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "RemainingAmount", HeaderText = "Remaining Amount", DataPropertyName = "remaining_amount", Width = 150 });
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "RoundOff", HeaderText = "Round Off", DataPropertyName = "round_off", Width = 100 });
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "GrandTotal", HeaderText = "Grand Total", DataPropertyName = "grand_total", Width = 120 });

            // Ensure column headers are displayed properly
            bookingDataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void txtCustName_TextChanged(object sender, EventArgs e)
        {

        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
