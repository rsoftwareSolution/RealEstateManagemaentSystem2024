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
        private bool isUpdating = false; // Flag to prevent recursion

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
            LoadFlatType();

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
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "FlatRate", HeaderText = "Flat Rate", DataPropertyName = "flat_rate", Width = 100 });
            bookingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "GSTGroup", HeaderText = "GST Group", DataPropertyName = "gst_group", Width = 100 });
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

        private void LoadQuotationId(string customerContact)
        {
            // Query to fetch quotation ID based on customer contact
            string query = $@" SELECT quatation_id FROM quatation_details WHERE customer_contact = '{customerContact}' LIMIT 1;";

            DataTable dt = db.ExecuteQuery(query);

            if (dt.Rows.Count > 0)
            {
                // If a quotation ID is found, fill it in the textbox
                tbQuotationNumber.Text = dt.Rows[0]["quatation_id"].ToString();
            }
            else
            {
                // If no quotation ID is found, set it to "N/A"
                tbQuotationNumber.Text = "N/A";
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

            string query = "INSERT INTO booking_details VALUES (@id, @date, @payment, @quotation, @downPayment, @contact, @name, @project, @flatType, @vehicle, @parking, @flatRate, @GSTGroup, @sgst, @subTotal, @totalAmount, @paid, @remaining, @roundOff, @grandTotal)";

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
                new MySqlParameter("@flatRate", igst),
                new MySqlParameter("@GSTGroup", cgst),
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
                string query = "SELECT vehicle_name FROM parking_details WHERE available_parking > 0 ORDER BY vehicle_name ASC";
                DataTable dt = db.ExecuteQuery(query);

                // 🔹 FIX: Clear DataSource first (Prevents "Item Collection Modification" Error)
                cbVehicle.DataSource = null;

                // 🔹 FIX: Set new DataSource (Avoid modifying `Items` directly)
                cbVehicle.DataSource = dt;
                cbVehicle.DisplayMember = "vehicle_name";
                cbVehicle.ValueMember = "vehicle_name";
                cbVehicle.SelectedIndex = -1; // No selection by default
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading vehicle names: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadFlatType()
        {
            try
            {
                string query = "SELECT flat_type_name FROM flat_type_details";
                DataTable dt = db.ExecuteQuery(query);

                // 🔹 FIX: Assign DataSource instead of manually adding items
                cmbProduct.DataSource = dt;
                cmbProduct.DisplayMember = "flat_type_name";
                cmbProduct.ValueMember = "flat_type_name";
                cmbProduct.SelectedIndex = -1; // No selection by default
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading flat types: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    row["flat_rate"],
                    row["gst_group"],
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

            string query = "UPDATE booking_details SET booking_date=@date, payment_type=@payment, quotation_number=@quotation, down_payment=@downPayment, cust_contact=@contact, cust_name=@name, building_or_project_name=@project, flat_type=@flatType, vehicle_name=@vehicle, parking_charges=@parking, flat_rate=@flatRate, gst_group=@gstGroup, sgst=@sgst, sub_total=@subTotal, total_amount=@totalAmount, paid_amount=@paid, remaining_amount=@remaining, round_off=@roundOff, grand_total=@grandTotal WHERE booking_id=@id";

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
                new MySqlParameter("@flatRate", igst),
                new MySqlParameter("@GSTGroup", cgst),
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

        private void tbCustContact_TextChanged(object sender, EventArgs e)
        {
            if (tbCustContact.Text.Length == 10) // Check for full 10-digit number
            {
                AutoFillCustomerName(tbCustContact.Text);
                LoadQuotationId(tbCustContact.Text);
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
            tbCustName.Clear();
            tbCustContact.Clear();
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

            LoadAvailableVehicles();
            GenerateNewBookingId();
            LoadQuotationChart();
            LoadProjectNames();
            LoadCustomerContacts();
            LoadFlatType();
        }

        private void tbCustNameSearch_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbCustNameSearch.Text))
            {
                ClearBookingForm();  // Clear form if search box is empty
            }
            else
            {
                SearchByCustomerName(tbCustNameSearch.Text);
            }
        }

        // Fetch Flat Rate from Database
        private double GetFlatRate(string flatTypeName)
        {
            if (string.IsNullOrEmpty(flatTypeName)) return 0.00; // Return 0 if no flat type is selected

            double flatRate = 0.00;

            // Step 1: Get flat_type_id from flat_type_name
            string getFlatTypeIdQuery = $@"SELECT flat_type_id FROM flat_type_details WHERE flat_type_name = '{flatTypeName}' LIMIT 1;";

            DataTable dtType = db.ExecuteQuery(getFlatTypeIdQuery);

            if (dtType.Rows.Count == 0) return 0.00; // If no matching flat type found, return 0

            string flatTypeId = dtType.Rows[0]["flat_type_id"].ToString(); // Get flat_type_id

            // Step 2: Get rate from flat_details using flat_type_id
            string getFlatRateQuery = $@"SELECT rate FROM flat_details WHERE flat_type_id = '{flatTypeId}' LIMIT 1;";

            DataTable dtRate = db.ExecuteQuery(getFlatRateQuery);

            if (dtRate.Rows.Count > 0)
            {
                flatRate = Convert.ToDouble(dtRate.Rows[0]["rate"]);
            }

            return flatRate;
        }


        private void tbParkingCharges_TextChanged(object sender, EventArgs e)
        {
            // Always IGST and CGST are 0
            tbCGST.Text = "0.00";

            // Use IGST text as a temporary replacement for flat rate
            double flatRate = 0.00;
            double.TryParse(tbIGST.Text, out flatRate); // Convert IGST text to double safely

            // Calculate SGST (18% of Flat Rate)
            double sgst = flatRate * 0.18;
            tbSGST.Text = sgst.ToString("0.00");

            // Get Parking Charges (ensure it's a valid number)
            double parkingCharges = string.IsNullOrWhiteSpace(tbParkingCharges.Text) ? 0.00 : Convert.ToDouble(tbParkingCharges.Text);

            // Calculate Sub Total
            double subTotal = parkingCharges + flatRate;
            tbSubTotal.Text = subTotal.ToString("0.00");

            // Total Amount
            double totalAmount = subTotal + sgst;

            tbTotalAmount.Text = totalAmount.ToString("0.00");
        }

        private void cbVehicle_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbProduct_Leave(object sender, EventArgs e)
        {
            double flatRate = GetFlatRate(cmbProduct.Text);
            tbIGST.Text = flatRate.ToString("0.00"); // Convert double to string with 2 decimal places
        }

        private void tbPaidAmount_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void bookingDataGrid_SelectionChanged_1(object sender, EventArgs e)
        {
            if (bookingDataGrid.SelectedRows.Count == 0) return;

            try
            {
                isUpdating = true; // Prevent event recursion

                DataGridViewRow row = bookingDataGrid.SelectedRows[0];

                tbBookingId.Text = row.Cells["BookingID"].Value?.ToString();
                dtpBookingDate.Text = row.Cells["BookingDate"].Value?.ToString();
                cbPaymentType.Text = row.Cells["PaymentType"].Value?.ToString();
                tbQuotationNumber.Text = row.Cells["QuotationNumber"].Value?.ToString();
                tbDownPayment.Text = row.Cells["DownPayment"].Value?.ToString();
                tbCustContact.Text = row.Cells["CustomerContact"].Value?.ToString();
                tbCustName.Text = row.Cells["CustomerName"].Value?.ToString();
                cbProjectName.Text = row.Cells["BuildingName"].Value?.ToString();
                cmbProduct.Text = row.Cells["FlatType"].Value?.ToString();
                cbVehicle.Text = row.Cells["VehicleName"].Value?.ToString();
                tbParkingCharges.Text = row.Cells["ParkingCharges"].Value?.ToString();
                tbIGST.Text = row.Cells["FlatRate"].Value?.ToString();
                tbCGST.Text = row.Cells["GSTGroup"].Value?.ToString();
                tbSGST.Text = row.Cells["SGST"].Value?.ToString();
                tbSubTotal.Text = row.Cells["SubTotal"].Value?.ToString();
                tbTotalAmount.Text = row.Cells["TotalAmount"].Value?.ToString();
                tbPaidAmount.Text = row.Cells["PaidAmount"].Value?.ToString();
                tbRemainingAmount.Text = row.Cells["RemainingAmount"].Value?.ToString();
                tbRoundOff.Text = row.Cells["RoundOff"].Value?.ToString();
                tbGrandTotal.Text = row.Cells["GrandTotal"].Value?.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isUpdating = false; // Reset flag
            }
        }

        private void DisableTextBoxEvents()
        {
            tbPaidAmount.TextChanged -= tbPaidAmount_TextChanged;
        }

        private void EnableTextBoxEvents()
        {
            tbPaidAmount.TextChanged += tbPaidAmount_TextChanged;
        }

        private void bookingDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            DisableTextBoxEvents();

            // Set textbox values from grid
            tbPaidAmount.Text = bookingDataGrid.SelectedRows[0].Cells["PaidAmount"].Value?.ToString();

            EnableTextBoxEvents();
        }

        private void tbPaidAmount_Leave(object sender, EventArgs e)
        {
            if (isUpdating) return; // Prevent infinite loop

            try
            {
                isUpdating = true; // Set flag to prevent recursive calls

                // Parse numeric values safely
                double downPayment = string.IsNullOrWhiteSpace(tbDownPayment.Text) ? 0.00 : Convert.ToDouble(tbDownPayment.Text);
                double paidAmount = string.IsNullOrWhiteSpace(tbPaidAmount.Text) ? 0.00 : Convert.ToDouble(tbPaidAmount.Text);
                double totalAmount = string.IsNullOrWhiteSpace(tbTotalAmount.Text) ? 0.00 : Convert.ToDouble(tbTotalAmount.Text);

                // 🔹 FIX: Only consider the entered Paid Amount (without accumulating)
                double finalPaidAmount = paidAmount;
                tbPaidAmount.Text = finalPaidAmount.ToString("0.00");

                // 🔹 FIX: Remaining Amount should consider Total Amount - (Paid + Down Payment)
                double remainingAmount = totalAmount - (finalPaidAmount + downPayment);
                tbRemainingAmount.Text = remainingAmount.ToString("0.00");

                // Calculate Grand Total (Remaining Amount + Paid Amount)
                double grandTotal = remainingAmount + finalPaidAmount;
                tbGrandTotal.Text = grandTotal.ToString("0.00");

                // Calculate Round Off based on Grand Total
                double roundOff = Math.Round(grandTotal);
                tbRoundOff.Text = roundOff.ToString("0.00");
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter valid numeric values.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                isUpdating = false; // Reset flag
            }
        }
    }
}
