using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace RealStateManagementSystem.config
{
    public partial class CustomerMaster : Form
    {
        Database db = new Database();
        private AutoIdGenerator idGenerator = new AutoIdGenerator("CUST", 4);

        public CustomerMaster()
        {
            InitializeComponent();
        }

        private void InitializeCustomerGrid()
        {
            // Clear existing columns to avoid duplication
            customerDataGrid.Columns.Clear();

            // Disable auto column generation
            customerDataGrid.AutoGenerateColumns = false;

            // Set DataGridView Header Height
            customerDataGrid.ColumnHeadersHeight = 35;
            customerDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Set DataGridView row height
            customerDataGrid.RowTemplate.Height = 30; // Adjust row height for better visibility

            // Define columns with custom widths
            customerDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "CustomerID", HeaderText = "ID", DataPropertyName = "cust_id", Width = 100 });
            customerDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "CustomerName", HeaderText = "Name", DataPropertyName = "cust_name", Width = 170 });
            customerDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "CustomerAddress", HeaderText = "Address", DataPropertyName = "cust_address", Width = 200 });
            customerDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "CustomerContact", HeaderText = "Contact", DataPropertyName = "cust_contact", Width = 140 });
            customerDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "CustomerBirthDate", HeaderText = "Birth Date", DataPropertyName = "cust_birth_date", Width = 100 });
            customerDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "CustomerEmail", HeaderText = "Email", DataPropertyName = "cust_email", Width = 210 });

            // Ensure column headers are displayed properly
            customerDataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }


        private void CustomerMaster_Load(object sender, EventArgs e)
        {
            // Set DataGridView header styles
            customerDataGrid.EnableHeadersVisualStyles = false;
            customerDataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.SlateGray;
            customerDataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            customerDataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 11, FontStyle.Regular);
            customerDataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Ensure columns are defined
            InitializeCustomerGrid();

            GenerateCustomerId(); // Automatically set the next customer ID on load
            ClearFields();

            try
            {
                string query = "SELECT * FROM customer_details";
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
            // Sort data based on Customer ID (Assuming cust_id is a string, change to integer if needed)
            dataTable.DefaultView.Sort = "cust_id ASC"; // Use DESC for descending order
            DataTable sortedTable = dataTable.DefaultView.ToTable();

            customerDataGrid.Rows.Clear();

            foreach (DataRow row in sortedTable.Rows)
            {
                customerDataGrid.Rows.Add(
                    row["cust_id"],
                    row["cust_name"],
                    row["cust_address"],
                    row["cust_contact"],
                    row["cust_birth_date"],
                    row["cust_email"]
                );
            }
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            try
            {

                // Get user inputs from TextBoxes 
                string custId = tbCustId.Text.Trim();
                string custName = tbCustName.Text.Trim();
                string custAddress = tbCustAddress.Text.Trim();
                string custContact = tbCustCont.Text.Trim();
                string custBirthDate = dtpCustomerBirthDate.Text.Trim();
                string custEmail = tbEmail.Text.Trim();

                // Validate input fields
                if (string.IsNullOrEmpty(custName) || string.IsNullOrEmpty(custAddress) || string.IsNullOrEmpty(custContact))
                {
                    MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check if the customer with the same email or contact already exists
                string checkQuery = "SELECT COUNT(*) FROM customer_details WHERE cust_email = @custEmail OR cust_contact = @custContact";
                object existingRecord = db.ExecuteScalar(checkQuery, new MySqlParameter[]
                {
                    new MySqlParameter("@custEmail", MySqlDbType.VarChar) { Value = custEmail },
                    new MySqlParameter("@custContact", MySqlDbType.VarChar) { Value = custContact }
                });

                if (Convert.ToInt32(existingRecord) > 0)
                {
                    MessageBox.Show("Customer with the same email or contact already exists.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Insert the new customer record
                string insertQuery = "INSERT INTO customer_details (cust_id, cust_name, cust_address, cust_contact, cust_birth_date, cust_email) " +
                                     "VALUES (@custId, @custName, @custAddress, @custContact, @custBirthDate, @custEmail)";

                db.ExecuteNonQuery(insertQuery, new MySqlParameter[]
                {
                    new MySqlParameter("@custId", MySqlDbType.VarChar) { Value = custId },
                    new MySqlParameter("@custName", MySqlDbType.VarChar) { Value = custName },
                    new MySqlParameter("@custAddress", MySqlDbType.VarChar) { Value = custAddress },
                    new MySqlParameter("@custContact", MySqlDbType.VarChar) { Value = custContact },
                    new MySqlParameter("@custBirthDate", MySqlDbType.VarChar) { Value = custBirthDate },
                    new MySqlParameter("@custEmail", MySqlDbType.VarChar) { Value = custEmail }
                });

                MessageBox.Show("Customer details saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear the input fields and generate the next ID
                ClearFields();
                GenerateCustomerId();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving customer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateCustomerId()
        {
            try
            {
                // Corrected query for MySQL
                string query = @"SELECT cust_id FROM customer_details WHERE cust_id LIKE 'CUST%' ORDER BY CONVERT(SUBSTRING(cust_id, 5, CHAR_LENGTH(cust_id) - 4), UNSIGNED) DESC LIMIT 1";

                // Execute the query
                object maxIdObj = db.ExecuteScalar(query);

                int nextId = 1; // Default if no record exists

                if (maxIdObj != null && maxIdObj != DBNull.Value)
                {
                    string lastId = maxIdObj.ToString();  // Example: "CUST0021"

                    if (lastId.StartsWith("CUST") && int.TryParse(lastId.Substring(4), out int numericPart))
                    {
                        nextId = numericPart + 1; // Increment ID
                    }
                }

                // Generate new ID with zero padding (CUST0001, CUST0002, ...)
                tbCustId.Text = $"CUST{nextId:D4}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating customer ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbCustId.Text = string.Empty;  // Reset textbox on error
            }
        }

        private void ClearFields()
        {
            tbCustName.Clear();
            tbCustAddress.Clear();
            tbCustCont.Clear();
            tbEmail.Clear();
            tbCustName.Select();
            GenerateCustomerId();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            string custName = tbCustName.Text;
            string query = "SELECT * FROM customer_details WHERE cust_name = @custName";

            try
            {
                DataTable result = db.ExecuteQuery(query, new MySqlParameter[]
                {
                    new MySqlParameter("@custName", MySqlDbType.VarChar) { Value = custName }
                });

                if (result.Rows.Count > 0)
                {
                    DataRow row = result.Rows[0];
                    tbCustId.Text = row["cust_id"].ToString();
                    tbCustAddress.Text = row["cust_address"].ToString();
                    tbCustCont.Text = row["cust_contact"].ToString();
                    dtpCustomerBirthDate.Text = row["cust_birth_date"].ToString();
                    tbEmail.Text = row["cust_email"].ToString();
                    MessageBox.Show("Record fetched successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No record found for the given customer name.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string custId = tbCustId.Text;
            string custName = tbCustName.Text;
            string custAddress = tbCustAddress.Text;
            string custContact = tbCustCont.Text;
            string custBirthDate = dtpCustomerBirthDate.Text;
            string custEmail = tbEmail.Text;

            string query = "UPDATE customer_details SET cust_name = @custName, cust_address = @custAddress, cust_contact = @custContact, " +
                           "cust_birth_date = @custBirthDate, cust_email = @custEmail WHERE cust_id = @custId";

            try
            {
                db.ExecuteNonQuery(query, new MySqlParameter[]
                {
                    new MySqlParameter("@custId", MySqlDbType.VarChar) { Value = custId },
                    new MySqlParameter("@custAddress", MySqlDbType.VarChar) { Value = custAddress },
                    new MySqlParameter("@custContact", MySqlDbType.VarChar) { Value = custContact },
                    new MySqlParameter("@custBirthDate", MySqlDbType.VarChar) { Value = custBirthDate },
                    new MySqlParameter("@custEmail", MySqlDbType.VarChar) { Value = custEmail },
                    new MySqlParameter("@custName", MySqlDbType.VarChar) { Value = custName }
                });

                MessageBox.Show("Record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string custName = tbCustName.Text;
            string query = "DELETE FROM customer_details WHERE cust_name = @custName";

            try
            {
                int rowsAffected = db.ExecuteNonQuery(query, new MySqlParameter[]
                {
                    new MySqlParameter("@custName", MySqlDbType.VarChar) { Value = custName }
                });

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Customer deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("No customer found with the given name.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting customer: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            SearchByCustomerName(textBox5.Text);
        }

        private void SearchByCustomerName(string custName)
        {
            try
            {
                // Define the query to search by customer name
                string query = "SELECT cust_id, cust_name, cust_address, cust_contact, cust_birth_date, cust_email " +
                               "FROM customer_details WHERE cust_name LIKE @custName";

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

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
