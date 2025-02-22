using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MySql.Data.MySqlClient;
using RealStateManagementSystem.config;

namespace RealEstateManagemaentSystem2024.Registers
{
    public partial class BookingRegister : Form
    {

        Database db = new Database();
        private bool isCustomerValid = false; // Flag to track customer existence

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
                    isCustomerValid = true; // Mark customer as valid
                }
                else
                {
                    tbCustName.Text = string.Empty;
                    isCustomerValid = false;

                    DialogResult result = MessageBox.Show(
                        "Customer not found. Would you like to add a new customer?",
                        "Customer Not Found",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (result == DialogResult.Yes)
                    {
                        OpenCustomerMasterForm(contactNumber);
                    }
                    else
                    {
                        tbCustContact.Text = string.Empty; // Clear contact number
                        tbCustContact.Focus(); // Refocus the textbox for user to enter a valid number
                        LoadCustomerContacts();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching customer name: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenCustomerMasterForm(string contactNumber)
        {
            var customerMasterForm = new CustomerMaster();
            customerMasterForm.PreFillContactNumber(contactNumber);
            customerMasterForm.ShowDialog();
        }

        private void SearchByCustomerName(string custName)
        {
            try
            {
                // Define the query to search by customer name
                string query = "SELECT * FROM booking_details WHERE cust_name LIKE @custName";

                // Execute the query with a parameter
                DataTable dataTable = db.ExecuteQuery(query, new MySqlParameter[]
                {
            new MySqlParameter("@custName", MySqlDbType.VarChar) { Value = $"%{custName}%" } // Supports partial search
                });

                // Bind only the data to the pre-defined columns
                PopulateDataGridView(dataTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BookingRegister_Load(object sender, EventArgs e)
        {
            // Attach events
            tbDownPayment.Enter += TextBox_Enter;
            tbDownPayment.Leave += TextBox_Leave;

            tbCustNameSearch.Enter += TextBox_Enter;
            tbCustNameSearch.Leave += TextBox_Leave;

            tbParkingCharges.Enter += TextBox_Enter;
            tbParkingCharges.Leave += TextBox_Leave;

            tbIGST.Enter += TextBox_Enter;
            tbIGST.Leave += TextBox_Leave;

            tbCGST.Enter += TextBox_Enter;
            tbCGST.Leave += TextBox_Leave;

            tbSGST.Enter += TextBox_Enter;
            tbSGST.Leave += TextBox_Leave;

            tbSubTotal.Enter += TextBox_Enter;
            tbSubTotal.Leave += TextBox_Leave;

            tbTotalAmount.Enter += TextBox_Enter;
            tbTotalAmount.Leave += TextBox_Leave;

            tbPaidAmount.Enter += TextBox_Enter;
            tbPaidAmount.Leave += TextBox_Leave;

            tbRemainingAmount.Enter += TextBox_Enter;
            tbRemainingAmount.Leave += TextBox_Leave;

            tbRoundOff.Enter += TextBox_Enter;
            tbRoundOff.Leave += TextBox_Leave;

            tbGrandTotal.Enter += TextBox_Enter;
            tbGrandTotal.Leave += TextBox_Leave;

            ClearBookingForm();
            LoadAvailableVehicles();
            GenerateNewBookingId();
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
            bookingDataGrid.ColumnHeadersHeight = 35;
            bookingDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Set DataGridView row height
            bookingDataGrid.RowTemplate.Height = 25;

            // Define columns with matching DataPropertyName values
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "BookingID", HeaderText = "Booking ID", DataPropertyName = "booking_id", Width = 80 });
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "BookingDate", HeaderText = "Booking Date", DataPropertyName = "booking_date", Width = 100 });
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "PaymentType", HeaderText = "Payment Type", DataPropertyName = "payment_type", Width = 120 });
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "QuotationNumber", HeaderText = "Quotation Number", DataPropertyName = "quotation_number", Width = 120 });
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "DownPayment", HeaderText = "Down Payment", DataPropertyName = "down_payment", Width = 120 });
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "CustomerContact", HeaderText = "Customer Contact", DataPropertyName = "cust_contact", Width = 120 });
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "CustomerName", HeaderText = "Customer Name", DataPropertyName = "cust_name", Width = 150 });
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

        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (textBox.Text == "0.00" || textBox.Text == "Search here")
                {
                    textBox.Text = "";
                }
            }
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    if (textBox == tbDownPayment || textBox == tbParkingCharges ||
                        textBox == tbIGST || textBox == tbCGST || textBox == tbSGST ||
                        textBox == tbSubTotal || textBox == tbTotalAmount ||
                        textBox == tbPaidAmount || textBox == tbRemainingAmount ||
                        textBox == tbRoundOff || textBox == tbGrandTotal)
                    {
                        textBox.Text = "0.00"; // Default numeric fields to "0.00"
                    }
                    else if (textBox == tbCustNameSearch) textBox.Text = "Search here"; // Search box placeholder
                }
                // If customer contact is empty, clear the customer name as well
                if (textBox == tbCustContact && string.IsNullOrWhiteSpace(tbCustContact.Text))
                {
                    tbCustName.Text = ""; // Clear customer name if contact is empty
                }
            }
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (!isCustomerValid)
            {
                MessageBox.Show("Customer not found or not added. Please enter a valid customer before saving.",
                                "Invalid Customer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbCustContact.Focus();
                return; // Stop execution
            }

            string bookingId = string.IsNullOrWhiteSpace(tbBookingId.Text) ? "BLK0001" : tbBookingId.Text.Trim();
            string bookingDate = dtpBookingDate.Value.ToString("yyyy-MM-dd");
            string paymentType = string.IsNullOrWhiteSpace(cbPaymentType.Text) ? "N/A" : cbPaymentType.Text.Trim();
            string quotationNumber = string.IsNullOrWhiteSpace(tbQuotationNumber.Text) ? "N/A" : tbQuotationNumber.Text.Trim();
            double downPayment = double.TryParse(tbDownPayment.Text, out double dp) ? dp : 0.0;
            string custContact = string.IsNullOrWhiteSpace(tbCustContact.Text) ? "N/A" : tbCustContact.Text.Trim();
            string custName = string.IsNullOrWhiteSpace(tbCustName.Text) ? "N/A" : tbCustName.Text.Trim();
            string projectName = string.IsNullOrWhiteSpace(cbProjectName.Text) ? "Not Selected" : cbProjectName.Text.Trim();
            string flatType = string.IsNullOrWhiteSpace(cmbProduct.Text) ? "N/A" : cmbProduct.Text.Trim();
            string vehicleName = string.IsNullOrWhiteSpace(cbVehicle.Text) ? "N/A" : cbVehicle.Text.Trim();
            double parkingCharges = double.TryParse(tbParkingCharges.Text, out double pc) ? pc : 0.0;
            double igst = double.TryParse(tbIGST.Text, out double i) ? i : 0.0;
            double cgst = double.TryParse(tbCGST.Text, out double c) ? c : 0.0;
            double sgst = double.TryParse(tbSGST.Text, out double s) ? s : 0.0;
            double subTotal = double.TryParse(tbSubTotal.Text, out double st) ? st : 0.0;
            double totalAmount = double.TryParse(tbTotalAmount.Text, out double ta) ? ta : 0.0;
            double paidAmount = double.TryParse(tbPaidAmount.Text, out double pa) ? pa : 0.0;
            double remainingAmount = double.TryParse(tbRemainingAmount.Text, out double ra) ? ra : 0.0;
            double roundOff = double.TryParse(tbRoundOff.Text, out double ro) ? ro : 0.0;
            double grandTotal = double.TryParse(tbGrandTotal.Text, out double gt) ? gt : 0.0;

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
            ClearBookingForm();
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

        private void LoadAvailableVehicles()
        {
            try
            {
                string query = "SELECT vehicle_name FROM parking_details WHERE available_parking > 0";
                DataTable dt = db.ExecuteQuery(query);

                cbVehicle.Items.Clear(); // Clear existing items
                foreach (DataRow row in dt.Rows)
                {
                    cbVehicle.Items.Add(row["vehicle_name"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading vehicle names: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            string bookingId = string.IsNullOrWhiteSpace(tbBookingId.Text) ? "BLK0001" : tbBookingId.Text.Trim();
            string bookingDate = dtpBookingDate.Value.ToString("yyyy-MM-dd");
            string paymentType = string.IsNullOrWhiteSpace(cbPaymentType.Text) ? "N/A" : cbPaymentType.Text.Trim();
            string quotationNumber = string.IsNullOrWhiteSpace(tbQuotationNumber.Text) ? "N/A" : tbQuotationNumber.Text.Trim();
            double downPayment = double.TryParse(tbDownPayment.Text, out double dp) ? dp : 0.0;
            string custContact = string.IsNullOrWhiteSpace(tbCustContact.Text) ? "N/A" : tbCustContact.Text.Trim();
            string custName = string.IsNullOrWhiteSpace(tbCustName.Text) ? "N/A" : tbCustName.Text.Trim();
            string projectName = string.IsNullOrWhiteSpace(cbProjectName.Text) ? "Not Selected" : cbProjectName.Text.Trim();
            string flatType = string.IsNullOrWhiteSpace(cmbProduct.Text) ? "N/A" : cmbProduct.Text.Trim();
            string vehicleName = string.IsNullOrWhiteSpace(cbVehicle.Text) ? "N/A" : cbVehicle.Text.Trim();
            double parkingCharges = double.TryParse(tbParkingCharges.Text, out double pc) ? pc : 0.0;
            double igst = double.TryParse(tbIGST.Text, out double i) ? i : 0.0;
            double cgst = double.TryParse(tbCGST.Text, out double c) ? c : 0.0;
            double sgst = double.TryParse(tbSGST.Text, out double s) ? s : 0.0;
            double subTotal = double.TryParse(tbSubTotal.Text, out double st) ? st : 0.0;
            double totalAmount = double.TryParse(tbTotalAmount.Text, out double ta) ? ta : 0.0;
            double paidAmount = double.TryParse(tbPaidAmount.Text, out double pa) ? pa : 0.0;
            double remainingAmount = double.TryParse(tbRemainingAmount.Text, out double ra) ? ra : 0.0;
            double roundOff = double.TryParse(tbRoundOff.Text, out double ro) ? ro : 0.0;
            double grandTotal = double.TryParse(tbGrandTotal.Text, out double gt) ? gt : 0.0;

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
            ClearBookingForm();
            GenerateNewBookingId();
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

        private void ClearBookingForm()
        {
            dtpBookingDate.Value = DateTime.Now;
            cbPaymentType.SelectedIndex = -1; // Reset selection
            tbQuotationNumber.Text = "N/A";
            tbDownPayment.Text = "0.00";
            cbProjectName.SelectedIndex = -1; // Reset selection
            cmbProduct.SelectedIndex = -1; // Reset selection
            cbVehicle.SelectedIndex = -1; // Reset selection
            tbParkingCharges.Text = "0.00";
            tbIGST.Text = "0.00";
            tbCGST.Text = "0.00";
            tbSGST.Text = "0.00";
            tbSubTotal.Text = "0.00";
            tbTotalAmount.Text = "0.00";
            tbPaidAmount.Text = "0.00";
            tbRemainingAmount.Text = "0.00";
            tbRoundOff.Text = "0.00";
            tbGrandTotal.Text = "0.00";

            cbA4.Checked = true;

            cbPaymentType.Select();
        }

        private void tbCustNameSearch_TextChanged(object sender, EventArgs e)
        {
            SearchByCustomerName(tbCustNameSearch.Text);
        }

        private void AutoCalculateBookingAmounts()
        {
            try
            {
                // Always IGST and CGST are 0
                tbIGST.Text = "0.00";
                tbCGST.Text = "0.00";

                // Get Parking Charges (ensure it's a valid number)
                double parkingCharges = string.IsNullOrWhiteSpace(tbParkingCharges.Text) ? 0.00 : Convert.ToDouble(tbParkingCharges.Text);

                // Fetch Flat Rate based on selected Flat Type
                double flatRate = GetFlatRate(cmbProduct.SelectedValue?.ToString());

                // Only show the error if the user has selected something
                if (cmbProduct.SelectedValue != null && flatRate == 0)
                {
                    MessageBox.Show("Flat rate not found. Please select a valid flat type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Calculate SGST (18% of Flat Rate)
                double sgst = flatRate * 0.18;
                tbSGST.Text = sgst.ToString("0.00");

                // Calculate Sub Total
                double subTotal = parkingCharges + sgst + flatRate;
                tbSubTotal.Text = subTotal.ToString("0.00");

                // Total Amount
                double totalAmount = subTotal;
                tbTotalAmount.Text = totalAmount.ToString("0.00");

                // Paid Amount = Down Payment
                double paidAmount = string.IsNullOrWhiteSpace(tbDownPayment.Text) ? 0.00 : Convert.ToDouble(tbDownPayment.Text);
                tbPaidAmount.Text = paidAmount.ToString("0.00");

                // Validate Down Payment
                if (paidAmount > totalAmount)
                {
                    MessageBox.Show("Down Payment is not valid. Please re-enter Paid Amount.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tbDownPayment.Text = "0.00"; // Reset Down Payment
                    tbPaidAmount.Text = "0.00";
                    return;
                }

                // Remaining Amount
                double remainingAmount = totalAmount - paidAmount;
                tbRemainingAmount.Text = remainingAmount.ToString("0.00");

                // Round Off Remaining Amount
                double roundOff = Math.Round(remainingAmount, 0); // Round to nearest integer
                tbRoundOff.Text = roundOff.ToString("0.00");

                // Grand Total
                tbGrandTotal.Text = roundOff.ToString("0.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Auto Calculation: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Fetch Flat Rate from Database
        private double GetFlatRate(string flatTypeId)
        {
            if (string.IsNullOrEmpty(flatTypeId)) return 0.00; // Return 0 if no flat type is selected

            double flatRate = 0.00;
            string query = $"SELECT rate FROM flat_details WHERE flat_type_id = '{flatTypeId}' LIMIT 1;";
            DataTable dt = db.ExecuteQuery(query);

            if (dt.Rows.Count > 0)
            {
                flatRate = Convert.ToDouble(dt.Rows[0]["rate"]);
            }

            return flatRate;
        }

        // Validate Down Payment
        private void ValidateDownPayment()
        {
            if (!double.TryParse(tbDownPayment.Text, out double paidAmount))
            {
                MessageBox.Show("Invalid input. Please enter a valid amount.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbDownPayment.Text = "0.00";
                tbPaidAmount.Text = "0.00";
                tbDownPayment.Focus();
                return;
            }

            double totalAmount = string.IsNullOrWhiteSpace(tbTotalAmount.Text) ? 0.00 : Convert.ToDouble(tbTotalAmount.Text);

            if (paidAmount > totalAmount)
            {
                MessageBox.Show("Down Payment is not valid. Please re-enter Paid Amount.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbDownPayment.Text = "0.00";
                tbPaidAmount.Text = "0.00";
                tbDownPayment.Focus();
            }
            else
            {
                tbPaidAmount.Text = paidAmount.ToString("0.00");
                AutoCalculateBookingAmounts(); // Recalculate amounts
            }
        }

        // Event Handlers (Trigger Calculation When Values Change)
        private void tbParkingCharges_TextChanged(object sender, EventArgs e) => AutoCalculateBookingAmounts();
        private void cmbProduct_SelectedIndexChanged_1(object sender, EventArgs e) => AutoCalculateBookingAmounts();

        // 🛠️ Validate Down Payment only when user leaves
        private void tbDownPayment_Leave(object sender, EventArgs e) => ValidateDownPayment();

    }
}
