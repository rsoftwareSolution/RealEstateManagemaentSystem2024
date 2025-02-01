using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace RealEstateManagemaentSystem2024.MasterForm
{
    public partial class QuatationMaster : Form
    {
        Database db = new Database();
        private AutoIdGenerator idGenerator = new AutoIdGenerator("QUOT", 4);

        public QuatationMaster()
        {
            InitializeComponent();
        }

        private DataTable GetQuotationData()
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
            quotationPieChart.Series.Clear();

            // Create a new series
            Series series = new Series("Flat Types")
            {
                ChartType = SeriesChartType.Doughnut
            };

            // Fetch data from the database
            DataTable dt = GetQuotationData();

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
                quotationPieChart.Series.Add(series);
                quotationPieChart.Invalidate(); // Force redraw
            }
            else
            {
                MessageBox.Show("No data found for the chart.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SetupAutoCompleteForTextBox()
        {
            try
            {
                // Query to fetch all flat descriptions
                string query = "SELECT flat_desc FROM flat_type_description";

                // Assuming db.ExecuteQuery runs the SQL and returns a DataTable
                DataTable dt = db.ExecuteQuery(query);

                // If DataTable is empty
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No descriptions found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Create a list to hold the flat descriptions
                List<string> descriptions = new List<string>();

                foreach (DataRow row in dt.Rows)
                {
                    descriptions.Add(row["flat_desc"].ToString()); // Add descriptions to the list
                }

                // Create a custom AutoCompleteStringCollection
                AutoCompleteStringCollection autoCompleteCollection = new AutoCompleteStringCollection();

                // Clear any existing data in the collection before adding new data
                autoCompleteCollection.Clear();

                // Add all descriptions to the AutoComplete collection
                autoCompleteCollection.AddRange(descriptions.ToArray());

                // Print the data in the AutoComplete collection for debugging
                Console.WriteLine("Auto-complete collection data:");
                foreach (var item in autoCompleteCollection)
                {
                    Console.WriteLine(item);  // Check if data is correctly added
                }

                // Setup AutoComplete properties for the TextBox
                tbDescription.AutoCompleteMode = AutoCompleteMode.SuggestAppend; // Suggest and append
                tbDescription.AutoCompleteSource = AutoCompleteSource.CustomSource; // Custom source for suggestions
                tbDescription.AutoCompleteCustomSource = autoCompleteCollection; // Set the custom source

                Console.WriteLine("Auto-complete setup complete.");
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                MessageBox.Show($"Error setting up auto-complete: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void QuatationMaster_Load(object sender, EventArgs e)
        {

            SetupAutoCompleteForTextBox();

            LoadQuotationChart();

            tbDescription.Enter += TextBox_Enter;
            tbDescription.Leave += TextBox_Leave;

            tbCustCont.Enter += TextBox_Enter;
            tbCustCont.Leave += TextBox_Leave;

            tbBuildName.Enter += TextBox_Enter;
            tbBuildName.Leave += TextBox_Leave;

            tbFlatType.Enter += TextBox_Enter;
            tbFlatType.Leave += TextBox_Leave;

            tbPricePerSqFt.Enter += TextBox_Enter;
            tbPricePerSqFt.Leave += TextBox_Leave;

            tbBasePrice.Enter += TextBox_Enter;
            tbBasePrice.Leave += TextBox_Leave;

            tbAdditionalCharges.Enter += TextBox_Enter;
            tbAdditionalCharges.Leave += TextBox_Leave;

            tbDiscount.Enter += TextBox_Enter;
            tbDiscount.Leave += TextBox_Leave;

            tbTotalAmount.Enter += TextBox_Enter;
            tbTotalAmount.Leave += TextBox_Leave;

            tbDownPayment.Enter += TextBox_Enter;
            tbDownPayment.Leave += TextBox_Leave;

            // Set DataGridView header styles
            quotationDataGrid.EnableHeadersVisualStyles = false;
            quotationDataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
            quotationDataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            quotationDataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 11, FontStyle.Regular);
            quotationDataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Ensure columns are defined
            InitializeQuotationGrid();

            GenerateQuotationId();
            ClearFields();

            try
            {
                string query = "SELECT * FROM quatation_details";
                DataTable dataTable = db.ExecuteQuery(query);
                PopulateDataGridView(dataTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeQuotationGrid()
        {
            // Clear existing columns to avoid duplication
            quotationDataGrid.Columns.Clear();

            // Disable auto column generation
            quotationDataGrid.AutoGenerateColumns = false;

            // Set DataGridView Header Height
            quotationDataGrid.ColumnHeadersHeight = 35; // Set header row height
            quotationDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Set DataGridView row height
            quotationDataGrid.RowTemplate.Height = 30; // Adjust row height for better visibility

            // Define columns with custom widths
            quotationDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "QuatationID", HeaderText = "ID", DataPropertyName = "quatation_id", Width = 60 });
            quotationDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "QuatationDate", HeaderText = "Date", DataPropertyName = "quatation_date", Width = 100 });
            quotationDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Description", HeaderText = "Description", DataPropertyName = "discription", Width = 180 });
            quotationDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "CustomerContact", HeaderText = "Contact", DataPropertyName = "customer_contact", Width = 120 });
            quotationDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "BuildingName", HeaderText = "Building Name", DataPropertyName = "building_name", Width = 150 });
            quotationDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "FlatType", HeaderText = "Flat Type", DataPropertyName = "flat_type_name", Width = 100 });
            quotationDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "PricePerSqFt", HeaderText = "Price/Sq.Ft", DataPropertyName = "price_per_sq_ft", Width = 100 });
            quotationDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "BasePrice", HeaderText = "Base Price", DataPropertyName = "base_price", Width = 120 });
            quotationDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "AdditionalCharges", HeaderText = "Additional Charges", DataPropertyName = "additionl_charges", Width = 150 });
            quotationDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Discount", HeaderText = "Discount", DataPropertyName = "discount", Width = 100 });
            quotationDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "TotalAmount", HeaderText = "Total Amount", DataPropertyName = "total_amount", Width = 120 });
            quotationDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "DownPayment", HeaderText = "Down Payment", DataPropertyName = "down_payment", Width = 120 });
            quotationDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "PaymentMode", HeaderText = "Payment Mode", DataPropertyName = "Payment_mode", Width = 130 });

            // Ensure column headers are displayed properly
            quotationDataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void PopulateDataGridView(DataTable dataTable)
        {
            quotationDataGrid.Rows.Clear();
            foreach (DataRow row in dataTable.Rows)
            {
                quotationDataGrid.Rows.Add(
                    row["quatation_id"],
                    row["quatation_date"],
                    row["discription"],
                    row["customer_contact"],
                    row["building_name"],
                    row["flat_type_name"],
                    row["price_per_sq_ft"],
                    row["base_price"],
                    row["additionl_charges"],
                    row["discount"],
                    row["total_amount"],
                    row["down_payment"],
                    row["Payment_mode"]
                );
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string quotationId = tbQuatationId.Text.Trim();
                string quotationDate = dtpDate.Text.Trim();
                string description = tbDescription.Text.Trim();
                string customerContact = tbCustCont.Text.Trim();
                string buildingName = tbBuildName.Text.Trim();
                string flatTypeName = tbFlatType.Text.Trim();
                double pricePerSqFt = Convert.ToDouble(tbPricePerSqFt.Text.Trim());
                double basePrice = Convert.ToDouble(tbBasePrice.Text.Trim());
                double additionalCharges = Convert.ToDouble(tbAdditionalCharges.Text.Trim());
                double discount = Convert.ToDouble(tbDiscount.Text.Trim());
                double totalAmount = Convert.ToDouble(tbTotalAmount.Text.Trim());
                double downPayment = Convert.ToDouble(tbDownPayment.Text.Trim());
                string paymentMode = GetSelectedPaymentMode();

                if (string.IsNullOrEmpty(paymentMode))
                {
                    MessageBox.Show("Please select a payment mode.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string insertQuery = "INSERT INTO quatation_details (quatation_id, quatation_date, discription, customer_contact, building_name, flat_type_name, price_per_sq_ft, base_price, additionl_charges, discount, total_amount, down_payment, Payment_mode) " +
                                     "VALUES (@quotationId, @quotationDate, @description, @customerContact, @buildingName, @flatTypeName, @pricePerSqFt, @basePrice, @additionalCharges, @discount, @totalAmount, @downPayment, @paymentMode)";

                db.ExecuteNonQuery(insertQuery, new MySqlParameter[]
                {
                    new MySqlParameter("@quotationId", MySqlDbType.VarChar) { Value = quotationId },
                    new MySqlParameter("@quotationDate", MySqlDbType.VarChar) { Value = quotationDate },
                    new MySqlParameter("@description", MySqlDbType.VarChar) { Value = description },
                    new MySqlParameter("@customerContact", MySqlDbType.VarChar) { Value = customerContact },
                    new MySqlParameter("@buildingName", MySqlDbType.VarChar) { Value = buildingName },
                    new MySqlParameter("@flatTypeName", MySqlDbType.VarChar) { Value = flatTypeName },
                    new MySqlParameter("@pricePerSqFt", MySqlDbType.Double) { Value = pricePerSqFt },
                    new MySqlParameter("@basePrice", MySqlDbType.Double) { Value = basePrice },
                    new MySqlParameter("@additionalCharges", MySqlDbType.Double) { Value = additionalCharges },
                    new MySqlParameter("@discount", MySqlDbType.Double) { Value = discount },
                    new MySqlParameter("@totalAmount", MySqlDbType.Double) { Value = totalAmount },
                    new MySqlParameter("@downPayment", MySqlDbType.Double) { Value = downPayment },
                    new MySqlParameter("@paymentMode", MySqlDbType.VarChar) { Value = paymentMode }
                });

                MessageBox.Show("Quotation details saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                GenerateQuotationId();
                if (cbPrint.Checked == true)
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving quotation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetSelectedPaymentMode()
        {
            if (cbCash.Checked) return "Cash";
            if (cbCard.Checked) return "Card";
            if (cbOnline.Checked) return "Online";
            return "";
        }

        private void ClearFields()
        {
            tbDescription.Select();
            tbDescription.Text = "Description";
            tbCustCont.Text = "Contact";
            tbBuildName.Text = "Building Name";
            tbFlatType.Text = "Flat Type";
            tbPricePerSqFt.Text = "Price/Sq.ft";
            tbBasePrice.Text = "Best Price";
            tbAdditionalCharges.Text = "Additional Charges";
            tbDiscount.Text = "Discount";
            tbTotalAmount.Text = "Total Amount";
            tbDownPayment.Text = "Down Payment";
            cbCash.Checked = false;
            cbCard.Checked = false;
            cbOnline.Checked = false;
            GenerateQuotationId();
        }

        private void GenerateQuotationId()
        {
            try
            {
                string query = "SELECT MAX(quatation_id) FROM quatation_details";
                object maxIdObj = db.ExecuteScalar(query);

                if (maxIdObj == null || maxIdObj == DBNull.Value || string.IsNullOrEmpty(maxIdObj.ToString()))
                {
                    tbQuatationId.Text = "QUOT0001"; // Starting value if no previous entry
                    return;
                }

                string maxIdStr = maxIdObj.ToString();
                string prefix = "QUOT";  // Define the prefix you expect (e.g., "QUOT")
                string numericPart = maxIdStr.Substring(prefix.Length); // Extract numeric part after prefix

                if (int.TryParse(numericPart, out int numericPartValue))
                {
                    int nextId = numericPartValue + 1;
                    tbQuatationId.Text = $"{prefix}{nextId:D4}";  // Format the next ID with leading zeros
                }
                else
                {
                    throw new FormatException($"Unexpected format for quatation_id: {maxIdStr}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating quotation ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbQuatationId.Text = string.Empty;
            }
        }

        // Placeholder Handling Methods
        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (textBox.Text == "Description" || textBox.Text == "Contact" ||
                    textBox.Text == "Building Name" || textBox.Text == "Flat Type" ||
                    textBox.Text == "Price/Sq.ft" || textBox.Text == "Best Price" ||
                    textBox.Text == "Additional Charges" || textBox.Text == "Discount" ||
                    textBox.Text == "Total Amount" || textBox.Text == "Down Payment")
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
                    if (textBox == tbDescription) textBox.Text = "Description";
                    else if (textBox == tbCustCont) textBox.Text = "Contact";
                    else if (textBox == tbBuildName) textBox.Text = "Building Name";
                    else if (textBox == tbFlatType) textBox.Text = "Flat Type";
                    else if (textBox == tbPricePerSqFt) textBox.Text = "Price/Sq.ft";
                    else if (textBox == tbBasePrice) textBox.Text = "Best Price";
                    else if (textBox == tbAdditionalCharges) textBox.Text = "Additional Charges";
                    else if (textBox == tbDiscount) textBox.Text = "Discount";
                    else if (textBox == tbTotalAmount) textBox.Text = "Total Amount";
                    else if (textBox == tbDownPayment) textBox.Text = "Down Payment";
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            if (tbSearchQuotation.Text != "Search Quotation") // Avoid searching when placeholder text is present
            {
                SearchByQuotationId(tbSearchQuotation.Text);
            }
        }
        private void SearchByQuotationId(string quotationId)
        {
            try
            {
                // Define the query to search by Quotation ID or other relevant fields
                string query = "SELECT * FROM quatation_details WHERE quatation_id LIKE @quotationId";

                // Execute the query with a parameter
                DataTable dataTable = db.ExecuteQuery(query, new MySqlParameter[]
                {
            new MySqlParameter("@quotationId", MySqlDbType.VarChar) { Value = $"%{quotationId}%" } // Supports partial search
                });

                // Bind the results to the DataGridView
                PopulateDataGridView(dataTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string quotationId = tbQuatationId.Text.Trim();

                if (string.IsNullOrEmpty(quotationId))
                {
                    MessageBox.Show("Please select a quotation to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show("Are you sure you want to delete this quotation?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    string deleteQuery = "DELETE FROM quatation_details WHERE quatation_id = @quotationId";

                    db.ExecuteNonQuery(deleteQuery, new MySqlParameter[] {
                        new MySqlParameter("@quotationId", MySqlDbType.VarChar) { Value = quotationId }
                    });

                    MessageBox.Show("Quotation deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields(); // Clear form fields
                    GenerateQuotationId(); // Generate new ID for next entry
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting quotation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                string customerContact = tbCustCont.Text.Trim();

                if (string.IsNullOrEmpty(customerContact))
                {
                    MessageBox.Show("Please enter a customer contact number to search.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // SQL SELECT query to fetch quotation based on customer contact
                string selectQuery = "SELECT * FROM quatation_details WHERE customer_contact = @customerContact";

                // Fetch data from the database
                DataTable dt = db.ExecuteQuery(selectQuery, new MySqlParameter("@customerContact", MySqlDbType.VarChar) { Value = customerContact });

                if (dt.Rows.Count > 0)
                {
                    // Assuming we are showing the data in a DataGridView
                    quotationDataGrid.DataSource = dt;

                    // Optionally, populate the form fields with the first result (if needed)
                    DataRow row = dt.Rows[0];
                    tbQuatationId.Text = row["quatation_id"].ToString();
                    dtpDate.Text = row["quatation_date"].ToString();
                    tbDescription.Text = row["discription"].ToString();
                    tbBuildName.Text = row["building_name"].ToString();
                    tbFlatType.Text = row["flat_type_name"].ToString();
                    tbPricePerSqFt.Text = row["price_per_sq_ft"].ToString();
                    tbBasePrice.Text = row["base_price"].ToString();
                    tbAdditionalCharges.Text = row["additionl_charges"].ToString();
                    tbDiscount.Text = row["discount"].ToString();
                    tbTotalAmount.Text = row["total_amount"].ToString();
                    tbDownPayment.Text = row["down_payment"].ToString();
                    // You can set Payment mode as well if needed
                }
                else
                {
                    MessageBox.Show("No quotation found for the given customer contact.", "No Data Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching quotation by customer contact: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                string quotationId = tbQuatationId.Text.Trim();
                string quotationDate = dtpDate.Text.Trim();
                string description = tbDescription.Text.Trim();
                string customerContact = tbCustCont.Text.Trim();
                string buildingName = tbBuildName.Text.Trim();
                string flatTypeName = tbFlatType.Text.Trim();
                double pricePerSqFt = Convert.ToDouble(tbPricePerSqFt.Text.Trim());
                double basePrice = Convert.ToDouble(tbBasePrice.Text.Trim());
                double additionalCharges = Convert.ToDouble(tbAdditionalCharges.Text.Trim());
                double discount = Convert.ToDouble(tbDiscount.Text.Trim());
                double totalAmount = Convert.ToDouble(tbTotalAmount.Text.Trim());
                double downPayment = Convert.ToDouble(tbDownPayment.Text.Trim());
                string paymentMode = GetSelectedPaymentMode();

                if (string.IsNullOrEmpty(paymentMode))
                {
                    MessageBox.Show("Please select a payment mode.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Update query for an existing quotation
                string updateQuery = "UPDATE quatation_details SET quatation_date = @quotationDate, discription = @description, customer_contact = @customerContact, building_name = @buildingName, flat_type_name = @flatTypeName, " +
                                     "price_per_sq_ft = @pricePerSqFt, base_price = @basePrice, additionl_charges = @additionalCharges, discount = @discount, total_amount = @totalAmount, " +
                                     "down_payment = @downPayment, Payment_mode = @paymentMode WHERE quatation_id = @quotationId";

                db.ExecuteNonQuery(updateQuery, new MySqlParameter[] {
                    new MySqlParameter("@quotationId", MySqlDbType.VarChar) { Value = quotationId },
                    new MySqlParameter("@quotationDate", MySqlDbType.VarChar) { Value = quotationDate },
                    new MySqlParameter("@description", MySqlDbType.VarChar) { Value = description },
                    new MySqlParameter("@customerContact", MySqlDbType.VarChar) { Value = customerContact },
                    new MySqlParameter("@buildingName", MySqlDbType.VarChar) { Value = buildingName },
                    new MySqlParameter("@flatTypeName", MySqlDbType.VarChar) { Value = flatTypeName },
                    new MySqlParameter("@pricePerSqFt", MySqlDbType.Double) { Value = pricePerSqFt },
                    new MySqlParameter("@basePrice", MySqlDbType.Double) { Value = basePrice },
                    new MySqlParameter("@additionalCharges", MySqlDbType.Double) { Value = additionalCharges },
                    new MySqlParameter("@discount", MySqlDbType.Double) { Value = discount },
                    new MySqlParameter("@totalAmount", MySqlDbType.Double) { Value = totalAmount },
                    new MySqlParameter("@downPayment", MySqlDbType.Double) { Value = downPayment },
                    new MySqlParameter("@paymentMode", MySqlDbType.VarChar) { Value = paymentMode }
            });

                MessageBox.Show("Quotation details updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                GenerateQuotationId();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating quotation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbDescription_TextChanged(object sender, EventArgs e)
        {
        }
    }
}
