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
            string custId = idGenerator.GenerateNextId(); // Generate the next unique ID
            string custName = tbCustName.Text.Trim();
            string custAddress = tbCustAddress.Text.Trim();
            string custContact = tbCustCont.Text.Trim();
            string custBirthDate = dtpCustomerBirthDate.Text;
            string custEmail = tbEmail.Text.Trim();

            string checkQuery = "SELECT COUNT(*) FROM customer_details WHERE cust_name = @custName AND cust_email = @custEmail";

            try
            {
                object result = db.ExecuteScalar(checkQuery, new MySqlParameter[]
                {
                    new MySqlParameter("@custName", MySqlDbType.VarChar) { Value = custName },
                    new MySqlParameter("@custEmail", MySqlDbType.VarChar) { Value = custEmail }
                });

                if (Convert.ToInt32(result) > 0)
                {
                    MessageBox.Show("Customer with the same name or email already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string saveQuery = "INSERT INTO customer_details (cust_id, cust_name, cust_address, cust_contact, cust_birth_date, cust_email) " +
                                   "VALUES (@custId, @custName, @custAddress, @custContact, @custBirthDate, @custEmail)";
                db.ExecuteNonQuery(saveQuery, new MySqlParameter[]
                {
                    new MySqlParameter("@custId", MySqlDbType.VarChar) { Value = custId },
                    new MySqlParameter("@custName", MySqlDbType.VarChar) { Value = custName },
                    new MySqlParameter("@custAddress", MySqlDbType.VarChar) { Value = custAddress },
                    new MySqlParameter("@custContact", MySqlDbType.VarChar) { Value = custContact },
                    new MySqlParameter("@custBirthDate", MySqlDbType.VarChar) { Value = custBirthDate },
                    new MySqlParameter("@custEmail", MySqlDbType.VarChar) { Value = custEmail }
                });

                MessageBox.Show("Customer details saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving customer: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            tbCustId.Clear();
            tbCustName.Clear();
            tbCustAddress.Clear();

            tbCustId.Text = idGenerator.GenerateNextId(); // Display the next generated ID
            tbCustName.Select();
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
            string custName = tbCustName.Text;
            string custAddress = tbCustAddress.Text;
            string custContact = tbCustCont.Text;
            string custBirthDate = dtpCustomerBirthDate.Text;
            string custEmail = tbEmail.Text;

            string query = "UPDATE customer_details SET cust_address = @custAddress, cust_contact = @custContact, " +
                           "cust_birth_date = @custBirthDate, cust_email = @custEmail WHERE cust_name = @custName";

            try
            {
                db.ExecuteNonQuery(query, new MySqlParameter[]
                {
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
