using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MySql.Data.MySqlClient;


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

        private void LoadCustomerContacts()
        {
            try
            {
                string query = "SELECT cust_contact FROM customer_details";
                DataTable dt = db.ExecuteQuery(query);

                AutoCompleteStringCollection contactCollection = new AutoCompleteStringCollection();
                foreach (DataRow row in dt.Rows)
                {
                    contactCollection.Add(row["cust_contact"].ToString());
                }

                tbCustContact.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                tbCustContact.AutoCompleteSource = AutoCompleteSource.CustomSource;
                tbCustContact.AutoCompleteCustomSource = contactCollection;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customer contacts: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AutoFillCustomerName(string contactNumber)
        {
            try
            {
                string query = "SELECT cust_name FROM customer_details WHERE cust_contact = @contact";
                DataTable dt = db.ExecuteQuery(query, new MySqlParameter("@contact", contactNumber));

                if (dt.Rows.Count > 0)
                {
                    tbCustName.Text = dt.Rows[0]["cust_name"].ToString();
                }
                else
                {
                    tbCustName.Text = string.Empty; // Clear if no match found
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching customer name: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BookingRegister_Load(object sender, EventArgs e)
        {
            LoadQuotationChart();
            LoadProjectNames();
            LoadCustomerContacts();

            // Set DataGridView header styles
            bookingDataGrid.EnableHeadersVisualStyles = false;
            bookingDataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.MintCream;
            bookingDataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            bookingDataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 10, FontStyle.Regular);
            bookingDataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Ensure columns are defined
            InitializeBookingGrid();
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

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            string bookingId = tbBookingId.Text.Trim();
            string bookingDate = dtpBookingDate.Value.ToString("yyyy-MM-dd");
            string paymentType = cbPaymentType.Text.Trim();
            string quotationNumber = tbQuotationNumber.Text.Trim();
            double downPayment = Convert.ToDouble(tbDownPayment.Text.Trim());
            string custContact = tbCustContact.Text.Trim();
            string custName = tbCustName.Text.Trim();
            string projectName = cbProjectName.Text.Trim();
            string flatType = cmbProduct.Text.Trim();
            string vehicleName = cbVehicle.Text.Trim();
            double parkingCharges = Convert.ToDouble(Text.Trim());
            double igst = Convert.ToDouble(tbIGST.Text.Trim());
            double cgst = Convert.ToDouble(tbCGST.Text.Trim());
            double sgst = Convert.ToDouble(tbSGST.Text.Trim());
            double subTotal = Convert.ToDouble(tbSubTotal.Text.Trim());
            double totalAmount = Convert.ToDouble(tbTotalAmount.Text.Trim());
            double paidAmount = Convert.ToDouble(tbPaidAmount.Text.Trim());
            double remainingAmount = Convert.ToDouble(tbRemainingAmount.Text.Trim());
            double roundOff = Convert.ToDouble(tbRoundOff.Text.Trim());
            double grandTotal = Convert.ToDouble(tbGrandTotal.Text.Trim());

            string query = "INSERT INTO booking_details VALUES (@id, @date, @payment, @quotation, @downPayment, @contact, @name, @project, @flatType, @vehicle, @parking, @igst, @cgst, @sgst, @subTotal, @totalAmount, @paid, @remaining, @roundOff, @grandTotal)";

            db.ExecuteNonQuery(query,
                new MySqlParameter("@id", bookingId),
                new MySqlParameter("@date", bookingDate),
                new MySqlParameter("@payment", paymentType),
                new MySqlParameter("@quotation", quotationNumber),
                new MySqlParameter("@downPayment", downPayment),
                new MySqlParameter("@contact", custContact),
                new MySqlParameter("@name", custName),
                new MySqlParameter("@project", projectName),
                new MySqlParameter("@flatType", flatType),
                new MySqlParameter("@vehicle", vehicleName),
                new MySqlParameter("@parking", parkingCharges),
                new MySqlParameter("@igst", igst),
                new MySqlParameter("@cgst", cgst),
                new MySqlParameter("@sgst", sgst),
                new MySqlParameter("@subTotal", subTotal),
                new MySqlParameter("@totalAmount", totalAmount),
                new MySqlParameter("@paid", paidAmount),
                new MySqlParameter("@remaining", remainingAmount),
                new MySqlParameter("@roundOff", roundOff),
                new MySqlParameter("@grandTotal", grandTotal));

            MessageBox.Show("Booking saved successfully");
            LoadBookingData();
            GenerateNewBookingId();
        }

        private void LoadProjectNames()
        {
            try
            {
                string query = "SELECT building_or_project_name FROM building_details ORDER BY building_or_project_name ASC";
                DataTable dt = db.ExecuteQuery(query);

                cbProjectName.DataSource = dt;
                cbProjectName.DisplayMember = "building_or_project_name"; // Display building names
                cbProjectName.ValueMember = "building_or_project_name";   // Store value as building name
                cbProjectName.SelectedIndex = -1; // Set default to no selection
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading project names: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateNewBookingId()
        {
            try
            {
                string query = @"SELECT booking_id FROM booking_details WHERE booking_id LIKE 'BLK%' 
                                 ORDER BY CONVERT(SUBSTRING(booking_id, 4, CHAR_LENGTH(booking_id) - 3), UNSIGNED) 
                                 DESC LIMIT 1";

                object maxIdObj = db.ExecuteScalar(query);

                int nextId = 1; // Default if no record exists

                if (maxIdObj != null && maxIdObj != DBNull.Value)
                {
                    string lastId = maxIdObj.ToString();  // Example: "BLK0021"

                    if (lastId.StartsWith("BLK") && int.TryParse(lastId.Substring(3), out int numericPart))
                    {
                        nextId = numericPart + 1; // Increment ID
                    }
                }

                tbBookingId.Text = $"BLK{nextId:D4}"; // Format ID as BLK0001, BLK0002, ...
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating Booking ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbBookingId.Text = string.Empty;
            }
        }

        private void LoadBookingData()
        {
            try
            {
                string query = "SELECT * FROM booking_details";
                DataTable dataTable = db.ExecuteQuery(query);
                PopulateDataGridView(dataTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateDataGridView(DataTable dataTable)
        {
            bookingDataGrid.Rows.Clear();

            foreach (DataRow row in dataTable.Rows)
            {
                bookingDataGrid.Rows.Add(
                    row["booking_id"],
                    row["booking_date"],
                    row["payment_type"],
                    row["quotation_number"],
                    row["down_payment"],
                    row["cust_contact"],
                    row["cust_name"],
                    row["building_or_project_name"],
                    row["flat_type"],
                    row["vehicle_name"],
                    row["parking_charges"],
                    row["igst"],
                    row["cgst"],
                    row["sgst"],
                    row["sub_total"],
                    row["total_amount"],
                    row["paid_amount"],
                    row["remaining_amount"],
                    row["round_off"],
                    row["grand_total"]
                );
            }
        }

        private void btn_Update_Click(object sender, EventArgs e)
        {
            string bookingId = tbBookingId.Text.Trim();
            string bookingDate = dtpBookingDate.Value.ToString("yyyy-MM-dd");
            string paymentType = cbPaymentType.Text.Trim();
            string quotationNumber = tbQuotationNumber.Text.Trim();
            double downPayment = Convert.ToDouble(tbDownPayment.Text.Trim());
            string custContact = tbCustContact.Text.Trim();
            string custName = tbCustName.Text.Trim();
            string projectName = cbProjectName.Text.Trim();
            string flatType = cmbProduct.Text.Trim();
            string vehicleName = cbVehicle.Text.Trim();
            double parkingCharges = Convert.ToDouble(tbParkingCharges.Text.Trim());
            double igst = Convert.ToDouble(tbIGST.Text.Trim());
            double cgst = Convert.ToDouble(tbCGST.Text.Trim());
            double sgst = Convert.ToDouble(tbSGST.Text.Trim());
            double subTotal = Convert.ToDouble(tbSubTotal.Text.Trim());
            double totalAmount = Convert.ToDouble(tbTotalAmount.Text.Trim());
            double paidAmount = Convert.ToDouble(tbPaidAmount.Text.Trim());
            double remainingAmount = Convert.ToDouble(tbRemainingAmount.Text.Trim());
            double roundOff = Convert.ToDouble(tbRoundOff.Text.Trim());
            double grandTotal = Convert.ToDouble(tbGrandTotal.Text.Trim());

            string query = "UPDATE booking_details SET booking_date=@date, payment_type=@payment, quotation_number=@quotation, down_payment=@downPayment, cust_contact=@contact, cust_name=@name, building_or_project_name=@project, flat_type=@flatType, vehicle_name=@vehicle, parking_charges=@parking, igst=@igst, cgst=@cgst, sgst=@sgst, sub_total=@subTotal, total_amount=@totalAmount, paid_amount=@paid, remaining_amount=@remaining, round_off=@roundOff, grand_total=@grandTotal WHERE booking_id=@id";

            db.ExecuteNonQuery(query,
                new MySqlParameter("@id", bookingId),
                new MySqlParameter("@date", bookingDate),
                new MySqlParameter("@payment", paymentType),
                new MySqlParameter("@quotation", quotationNumber),
                new MySqlParameter("@downPayment", downPayment),
                new MySqlParameter("@contact", custContact),
                new MySqlParameter("@name", custName),
                new MySqlParameter("@project", projectName),
                new MySqlParameter("@flatType", flatType),
                new MySqlParameter("@vehicle", vehicleName),
                new MySqlParameter("@parking", parkingCharges),
                new MySqlParameter("@igst", igst),
                new MySqlParameter("@cgst", cgst),
                new MySqlParameter("@sgst", sgst),
                new MySqlParameter("@subTotal", subTotal),
                new MySqlParameter("@totalAmount", totalAmount),
                new MySqlParameter("@paid", paidAmount),
                new MySqlParameter("@remaining", remainingAmount),
                new MySqlParameter("@roundOff", roundOff),
                new MySqlParameter("@grandTotal", grandTotal));

            MessageBox.Show("Booking updated successfully");
            LoadBookingData();
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Unable to process delete function for booking master data. If you want to perform delete/cancel booking go to cancelltion master.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ClearFields()
        {
            tbBookingId.Text = string.Empty;
            dtpBookingDate.Value = DateTime.Now; // Reset to current date
            cbPaymentType.SelectedIndex = -1;
            tbQuotationNumber.Text = string.Empty;
            tbDownPayment.Text = string.Empty;
            tbCustContact.Text = string.Empty;
            tbCustName.Text = string.Empty;
            cbProjectName.SelectedIndex = -1;
            tbParkingCharges.Text = string.Empty;
            tbIGST.Text = string.Empty;
            tbCGST.Text = string.Empty;
            tbSGST.Text = string.Empty;
            tbSubTotal.Text = string.Empty;
            tbTotalAmount.Text = string.Empty;
            tbPaidAmount.Text = string.Empty;
            tbRemainingAmount.Text = string.Empty;
            tbRoundOff.Text = string.Empty;
            tbGrandTotal.Text = string.Empty;

            GenerateNewBookingId();
            tbCustContact.Focus(); // Set focus to contact number field
        }

        private void tbCustContact_TextChanged(object sender, EventArgs e)
        {
            if (tbCustContact.Text.Length == 10) // Check for full 10-digit number
            {
                AutoFillCustomerName(tbCustContact.Text);
            }
        }
    }
}
