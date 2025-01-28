using MySql.Data.MySqlClient;
using System;
using System.Data;
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

        private void CustomerMaster_Load(object sender, EventArgs e)
        {
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
            customerDataGrid.Rows.Clear();
            foreach (DataRow row in dataTable.Rows)
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
                // Initialize the AutoIdGenerator
                AutoIdGenerator idGenerator = new AutoIdGenerator("CUST", 4);

                // Fetch the current maximum cust_id from the database
                string maxIdQuery = "SELECT MAX(cust_id) AS maxId FROM customer_details";
                object maxIdResult = db.ExecuteScalar(maxIdQuery);
                int maxId = maxIdResult != DBNull.Value ? Convert.ToInt32(maxIdResult) : 0;

                // Set the counter for the generator
                idGenerator.ResetCounter(maxId);

                // Generate the next customer ID
                string nextCustomerId = idGenerator.GenerateNextId();

                // Get user inputs from TextBoxes
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
                    new MySqlParameter("@custId", MySqlDbType.VarChar) { Value = nextCustomerId },
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
                // Query to get the maximum customer ID
                string query = "SELECT MAX(cust_id) FROM customer_details";

                // Execute the query
                object maxIdObj = db.ExecuteScalar(query);

                // Check if the result is null or DBNull
                if (maxIdObj == null || maxIdObj == DBNull.Value || string.IsNullOrEmpty(maxIdObj.ToString()))
                {
                    tbCustId.Text = "CUST0001"; // Start with "CUST0001" if no record exists
                }

                // Extract the numeric part from the ID (e.g., "CUST0003" -> "0003")
                string maxIdStr = maxIdObj.ToString();
                if (maxIdStr.StartsWith("CUST") && int.TryParse(maxIdStr.Substring(4), out int numericPart))
                {
                    // Increment the numeric part
                    int nextId = numericPart + 1;

                    // Format the new ID with the prefix and leading zeros
                    tbCustId.Text = $"CUST{nextId:D4}";
                }
                else
                {
                    throw new FormatException($"Unexpected format for cust_id: {maxIdStr}");
                }
            }
            catch (Exception ex)
            {
                // Log or display the error message
                MessageBox.Show($"Error generating customer ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbCustId.Text = string.Empty; // Return an empty string in case of an error
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
    }
}
