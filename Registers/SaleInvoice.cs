using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MySql.Data.MySqlClient;
using RealEstateManagemaentSystem2024.reports;
using RealStateManagementSystem.config;

namespace RealEstateManagemaentSystem2024.Registers
{
    public partial class SaleInvoice : Form
    {
        Database db = new Database();
        private bool isCustomerValid = false; // Flag to track customer existence
        private bool isUpdating = false; // Flag to prevent recursion


        public SaleInvoice()

        {
            InitializeComponent();

        }

        private void GenerateNewSaleId()
        {
            try
            {
                string query = @"SELECT sale_id FROM saleinvoice_details WHERE sale_id LIKE 'INV%' 
                             ORDER BY CONVERT(SUBSTRING(sale_id, 4, CHAR_LENGTH(sale_id) - 3), UNSIGNED) 
                             DESC LIMIT 1";

                object maxIdObj = db.ExecuteScalar(query);

                int nextId = 1; // Default if no record exists

                if (maxIdObj != null && maxIdObj != DBNull.Value)
                {
                    string lastId = maxIdObj.ToString();  // Example: "INV0021"

                    if (lastId.StartsWith("INV") && int.TryParse(lastId.Substring(3), out int numericPart))
                    {
                        nextId = numericPart + 1; // Increment ID
                    }
                }

                tbSaleId.Text = $"INV{nextId:D4}"; // Format ID as INV0001, INV0002, ...
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating Sale ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbSaleId.Text = string.Empty;
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
                cbFlatType.DataSource = dt;
                cbFlatType.DisplayMember = "flat_type_name";
                cbFlatType.ValueMember = "flat_type_name";
                cbFlatType.SelectedIndex = -1; // No selection by default
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading flat types: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearSaleForm()
        {
            dtpSaleDate.Value = DateTime.Now;
            cbPaymentType.SelectedIndex = -1; // Reset selection
            tbBookingNumber.Text = "N/A";
            tbDownPayment.Text = "0.00";

            cbProjectName.SelectedIndex = -1; // Reset selection
            cbFlatType.SelectedIndex = -1; // Reset selection
            tbCustName.Clear();
            tbCustContact.Clear();
            tbParkingCharges.Text = "0.00";
            tbGSTGroup.Text = "18%";
            tbSGST.Text = "0.00";
            tbRemainingAmount.Text = "0.00";
            tbSubTotal.Text = "0.00";
            tbTotalAmount.Text = "0.00";
            tbBookingAmount.Text = "0.00";
            tbRoundOff.Text = "0.00";
            tbGrandTotal.Text = "0.00";

            cbPaymentType.Select();

            LoadAvailableVehicles();
            GenerateNewSaleId();
            LoadSaleChart();
            LoadProjectNames();
            LoadCustomerContacts();
            LoadFlatType();
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
                    if (textBox == tbCustNameSearch) textBox.Text = "Search here"; // Search box placeholder
                }
                // If customer contact is empty, clear the customer name as well
                if (textBox == tbCustContact && string.IsNullOrWhiteSpace(tbCustContact.Text))
                {
                    tbCustName.Text = ""; // Clear customer name if contact is empty
                }
            }
        }

        private void SaleInvoice_Load(object sender, EventArgs e)
        {
            tbCustNameSearch.Enter += TextBox_Enter;
            tbCustNameSearch.Leave += TextBox_Leave;

            ClearSaleForm();
            LoadAvailableVehicles();
            GenerateNewSaleId();
            LoadSaleChart();
            LoadProjectNames();
            LoadCustomerContacts();
            LoadFlatType();

            // Set DataGridView header styles
            dgvSaleDetails.EnableHeadersVisualStyles = false;
            dgvSaleDetails.ColumnHeadersDefaultCellStyle.BackColor = Color.GhostWhite;
            dgvSaleDetails.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvSaleDetails.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 10, FontStyle.Regular);
            dgvSaleDetails.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Ensure columns are defined
            InitializeSaleGrid();
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private DataTable GetSaleData()
        {
            string query = "SELECT flat_type, COUNT(*) as count FROM booking_details GROUP BY flat_type";
            DataTable result = db.ExecuteQuery(query); // Assuming db.ExecuteQuery() returns DataTable

            // Check if data is returned
            if (result.Rows.Count == 0)
            {
                MessageBox.Show("No data found for chart.");
            }

            return result;
        }

        private void LoadSaleChart()
        {
            // Clear previous data to prevent duplicates
            salePieChart.Series.Clear();

            // Create a new series
            Series series = new Series("Flat Types")
            {
                ChartType = SeriesChartType.Doughnut
            };

            // Fetch data from the database
            DataTable dt = GetSaleData();

            if (dt.Rows.Count > 0) // Check if data exists
            {
                foreach (DataRow row in dt.Rows)
                {
                    string flatType = row["flat_type"].ToString();
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
                salePieChart.Series.Add(series);
                salePieChart.Invalidate(); // Force redraw
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
                string query = "SELECT * FROM saleinvoice_details WHERE cust_name LIKE @custName";

                // Execute the query with a parameter
                DataTable dataTable = db.ExecuteQuery(query, new MySqlParameter[]
                {
            new MySqlParameter("@custName", MySqlDbType.VarChar) { Value = $"%{custName}%" } // Supports partial search
                });

                if (dataTable.Rows.Count > 0)
                {
                    // Bind the new data
                    dgvSaleDetails.DataSource = dataTable;
                }
                else
                {
                    // Instead of setting DataSource to null, create an empty DataTable with same structure
                    DataTable emptyTable = ((DataTable)dgvSaleDetails.DataSource)?.Clone();
                    if (emptyTable != null)
                    {
                        dgvSaleDetails.DataSource = emptyTable; // Assign empty DataTable to prevent errors
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateDataGridView(DataTable dataTable)
        {
            dgvSaleDetails.Rows.Clear(); // Clear existing rows

            foreach (DataRow row in dataTable.Rows)
            {
                int rowIndex = dgvSaleDetails.Rows.Add();
                dgvSaleDetails.Rows[rowIndex].Cells["sale_id"].Value = row["sale_id"].ToString();
                dgvSaleDetails.Rows[rowIndex].Cells["sale_date"].Value = Convert.ToDateTime(row["sale_date"]).ToString("yyyy-MM-dd");
                dgvSaleDetails.Rows[rowIndex].Cells["payment_type"].Value = row["payment_type"].ToString();
                dgvSaleDetails.Rows[rowIndex].Cells["booking_number"].Value = row["booking_number"].ToString();
                dgvSaleDetails.Rows[rowIndex].Cells["cust_contact"].Value = row["cust_contact"].ToString();
                dgvSaleDetails.Rows[rowIndex].Cells["cust_name"].Value = row["cust_name"].ToString();
                dgvSaleDetails.Rows[rowIndex].Cells["project_name"].Value = row["project_name"].ToString();
                dgvSaleDetails.Rows[rowIndex].Cells["flat_type"].Value = row["flat_type"].ToString();
                dgvSaleDetails.Rows[rowIndex].Cells["flat_number"].Value = row["flat_number"].ToString();
                dgvSaleDetails.Rows[rowIndex].Cells["total_amount"].Value = row["total_amount"].ToString();
                dgvSaleDetails.Rows[rowIndex].Cells["grand_total"].Value = row["grand_total"].ToString();
            }
        }

        private void LoadBookingId(string customerContact)
        {
            string query = $"SELECT * FROM booking_details WHERE cust_contact = '{customerContact}' LIMIT 1;";
            DataTable dt = db.ExecuteQuery(query);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                tbBookingNumber.Text = row["booking_id"].ToString();
                tbCustName.Text = row["cust_name"].ToString();
                tbCustContact.Text = row["cust_contact"].ToString();
                cbProjectName.Text = row["building_or_project_name"].ToString();
                cbFlatType.Text = row["flat_type"].ToString();
                tbFlatRate.Text = row["flat_rate"].ToString();
                tbParkingCharges.Text = row["parking_charges"].ToString();
                tbTotalAmount.Text = row["total_amount"].ToString();
                tbDownPayment.Text = row["paid_amount"].ToString();
                tbBookingAmount.Text = row["paid_amount"].ToString();
                tbGSTGroup.Text = row["gst_group"].ToString();
                tbSGST.Text = row["sgst"].ToString();
                tbSubTotal.Text = row["sub_total"].ToString();
            }
            else
            {
                tbBookingNumber.Text = "N/A";
            }
        }

        private void GetBookingData()
        {
            string query = "SELECT flat_type_name, COUNT(*) as count FROM quatation_details GROUP BY flat_type_name";
            DataTable result = db.ExecuteQuery(query);

            if (result.Rows.Count == 0)
            {
                MessageBox.Show("No data found for chart.");
            }
        }

        private void InitializeSaleGrid()
        {
            dgvSaleDetails.DataSource = null;
            dgvSaleDetails.Columns.Clear();

            // Set grid properties
            dgvSaleDetails.AutoGenerateColumns = false;
            dgvSaleDetails.AllowUserToAddRows = false;
            dgvSaleDetails.ReadOnly = true;
            dgvSaleDetails.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Define columns
            dgvSaleDetails.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Sale ID", DataPropertyName = "sale_id", Width = 100 });
            dgvSaleDetails.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Sale Date", DataPropertyName = "sale_date", Width = 100 });
            dgvSaleDetails.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Payment Type", DataPropertyName = "payment_type", Width = 120 });
            dgvSaleDetails.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Booking Number", DataPropertyName = "booking_number", Width = 120 });
            dgvSaleDetails.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Down Payment", DataPropertyName = "down_payment", Width = 120 });
            dgvSaleDetails.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Customer Contact", DataPropertyName = "cust_contact", Width = 120 });
            dgvSaleDetails.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Customer Name", DataPropertyName = "cust_name", Width = 150 });
            dgvSaleDetails.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Project Name", DataPropertyName = "project_name", Width = 150 });
            dgvSaleDetails.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Flat Type", DataPropertyName = "flat_type", Width = 120 });
            dgvSaleDetails.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Flat Number", DataPropertyName = "flat_number", Width = 120 });
            dgvSaleDetails.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Vehicle Name", DataPropertyName = "vehicle_name", Width = 120 });
            dgvSaleDetails.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Parking Charges", DataPropertyName = "parking_charges", Width = 120 });
            dgvSaleDetails.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Flat Rate", DataPropertyName = "flat_rate", Width = 120 });
            dgvSaleDetails.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "GST Group", DataPropertyName = "gst_group", Width = 120 });
            dgvSaleDetails.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "GST Amount", DataPropertyName = "gst_amount", Width = 120 });
            dgvSaleDetails.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Sub Total", DataPropertyName = "sub_total", Width = 120 });
            dgvSaleDetails.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Total Amount", DataPropertyName = "total_amount", Width = 120 });
            dgvSaleDetails.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Booking Amount", DataPropertyName = "booking_amount", Width = 120 });
            dgvSaleDetails.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Round Off", DataPropertyName = "round_off", Width = 120 });
            dgvSaleDetails.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Grand Total", DataPropertyName = "grand_total", Width = 120 });

            // Load data
            LoadSaleData();
        }

        private void LoadSaleData()
        {
            try
            {
                string query = "SELECT * FROM saleinvoice_details";
                DataTable dt = db.ExecuteQuery(query);

                if (dt.Rows.Count > 0)
                {
                    dgvSaleDetails.DataSource = dt;
                }
                else
                {
                    dgvSaleDetails.DataSource = null;
                    MessageBox.Show("No sale records found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sale data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetFlatNumber(string flatType)
        {
            string query = "SELECT flat_id FROM flat_details fd JOIN flat_type_details ftd ON fd.flat_type_id = ftd.flat_type_id WHERE ftd.flat_type_name = @flatType LIMIT 1";
            DataTable dt = db.ExecuteQuery(query, new MySqlParameter("@flatType", flatType));

            return dt.Rows.Count > 0 ? dt.Rows[0]["flat_id"].ToString() : "N/A";
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            string saleId = string.IsNullOrWhiteSpace(tbSaleId.Text) ? "INV0001" : tbSaleId.Text.Trim();
            string saleDate = dtpSaleDate.Value.ToString("yyyy-MM-dd");
            string paymentType = string.IsNullOrWhiteSpace(cbPaymentType.Text) ? "N/A" : cbPaymentType.Text.Trim();
            string bookingNumber = string.IsNullOrWhiteSpace(tbBookingNumber.Text) ? "N/A" : tbBookingNumber.Text.Trim();
            double downPayment = double.TryParse(tbDownPayment.Text, out double dp) ? dp : 0.0;
            string custContact = string.IsNullOrWhiteSpace(tbCustContact.Text) ? "N/A" : tbCustContact.Text.Trim();
            string custName = string.IsNullOrWhiteSpace(tbCustName.Text) ? "N/A" : tbCustName.Text.Trim();
            string projectName = string.IsNullOrWhiteSpace(cbProjectName.Text) ? "Not Selected" : cbProjectName.Text.Trim();
            string flatType = string.IsNullOrWhiteSpace(cbFlatType.Text) ? "N/A" : cbFlatType.Text.Trim();
            string flatNumber = GetFlatNumber(flatType);
            string vehicleName = string.IsNullOrWhiteSpace(cbVehicle.Text) ? "N/A" : cbVehicle.Text.Trim();
            double parkingCharges = double.TryParse(tbParkingCharges.Text, out double pc) ? pc : 0.0;
            double flatRate = double.TryParse(tbFlatRate.Text, out double fr) ? fr : 0.0;
            double gstGroup = double.TryParse(tbGSTGroup.Text, out double gg) ? gg : 0.0;
            double gstAmount = double.TryParse(tbSGST.Text, out double ga) ? ga : 0.0;
            double subTotal = double.TryParse(tbSubTotal.Text, out double st) ? st : 0.0;
            double totalAmount = double.TryParse(tbTotalAmount.Text, out double ta) ? ta : 0.0;
            double bookingAmount = double.TryParse(tbBookingAmount.Text, out double ba) ? ba : 0.0;
            double roundOff = double.TryParse(tbRoundOff.Text, out double ro) ? ro : 0.0;
            double grandTotal = double.TryParse(tbGrandTotal.Text, out double gt) ? gt : 0.0;

            string query = "INSERT INTO saleinvoice_details (sale_id, sale_date, payment_type, booking_number, down_payment, cust_contact, cust_name, project_name, flat_type, flat_number, vehicle_name, parking_charges, flat_rate, gst_group, gst_amount, sub_total, total_amount, booking_amount, round_off, grand_total) " +
                           "VALUES (@saleId, @saleDate, @paymentType, @bookingNumber, @downPayment, @custContact, @custName, @projectName, @flatType, @flatNumber, @vehicleName, @parkingCharges, @flatRate, @gstGroup, @gstAmount, @subTotal, @totalAmount, @bookingAmount, @roundOff, @grandTotal)";

            db.ExecuteNonQuery(query,
                new MySqlParameter("@saleId", saleId),
                new MySqlParameter("@saleDate", saleDate),
                new MySqlParameter("@paymentType", paymentType),
                new MySqlParameter("@bookingNumber", bookingNumber),
                new MySqlParameter("@downPayment", downPayment),
                new MySqlParameter("@custContact", custContact),
                new MySqlParameter("@custName", custName),
                new MySqlParameter("@projectName", projectName),
                new MySqlParameter("@flatType", flatType),
                new MySqlParameter("@flatNumber", flatNumber),
                new MySqlParameter("@vehicleName", vehicleName),
                new MySqlParameter("@parkingCharges", parkingCharges),
                new MySqlParameter("@flatRate", flatRate),
                new MySqlParameter("@gstGroup", gstGroup),
                new MySqlParameter("@gstAmount", gstAmount),
                new MySqlParameter("@subTotal", subTotal),
                new MySqlParameter("@totalAmount", totalAmount),
                new MySqlParameter("@bookingAmount", bookingAmount),
                new MySqlParameter("@roundOff", roundOff),
                new MySqlParameter("@grandTotal", grandTotal));

            MessageBox.Show("Sale invoice saved successfully.");
            if (cbA4.Checked)
            {
                DownloadReports downloadReports = new DownloadReports(saleId);
                downloadReports.Show();
            }

            ClearSaleForm();
        }

        private void tbBookingAmount_Leave(object sender, EventArgs e)
        {
                if (isUpdating) return; // Prevent infinite loop

                try
                {
                    isUpdating = true; // Set flag to prevent recursive calls

                    // Parse numeric values safely
                    double downPayment = string.IsNullOrWhiteSpace(tbDownPayment.Text) ? 0.00 : Convert.ToDouble(tbDownPayment.Text);
                    double paidAmount = string.IsNullOrWhiteSpace(tbBookingAmount.Text) ? 0.00 : Convert.ToDouble(tbBookingAmount.Text);
                    double totalAmount = string.IsNullOrWhiteSpace(tbTotalAmount.Text) ? 0.00 : Convert.ToDouble(tbTotalAmount.Text);

                    // 🔹 FIX: Only consider the entered Paid Amount (without accumulating)
                    double finalPaidAmount = paidAmount;
                    tbBookingAmount.Text = finalPaidAmount.ToString("0.00");

                    // 🔹 FIX: Remaining Amount should consider Total Amount - (Paid + Down Payment)
                    double remainingAmount = totalAmount - (finalPaidAmount);
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

        private void tbCustContact_TextChanged(object sender, EventArgs e)
        {
            if (tbCustContact.Text.Length == 10) // Check for full 10-digit number
            {
                AutoFillCustomerName(tbCustContact.Text);
                LoadBookingId(tbCustContact.Text);
            }
        }

        private void tbCustNameSearch_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbCustNameSearch.Text))
            {
                ClearSaleForm();  // Clear form if search box is empty
            }
            else
            {
                SearchByCustomerName(tbCustNameSearch.Text);
            }
        }

        private void btn_Update_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Unable to process update/delete function for sale invoice data. If you want to perform update/delete sale data please contact to admin.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Unable to process update/delete function for sale invoice data. If you want to perform update/delete sale data please contact to admin.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
