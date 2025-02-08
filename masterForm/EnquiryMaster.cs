using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace RealStateManagementSystem.masterForm
{
    public partial class EnquiryMaster : Form
    {
        Database db = new Database();

        public EnquiryMaster()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {
            btnEdit.Enabled = true;
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void EnquiryMaster_Load(object sender, EventArgs e)
        {
            // Assuming 'dataGridView1' is your DataGridView control
            enquiryDataGrid.Font = new Font("Times New Roman", 11, FontStyle.Regular); // Set font name, size, and style
      
          tbEnquirName.Select();

            //This code represents data load into grid 
            try
            {
                // Initialize your Database helper class
                Database db = new Database();

                // Define the query
                string query = "SELECT * FROM enquiry_details";

                // Execute the query and get the result as a DataTable
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
            // Clear the existing rows (but keep the headers)
            enquiryDataGrid.Rows.Clear();

            // Loop through each row in the DataTable and add it to the DataGridView
            foreach (DataRow row in dataTable.Rows)
            {
                enquiryDataGrid.Rows.Add(
                    row["enquir_id"],
                    row["enquir_name"],
                    row["enquir_email"],
                    row["enquir_contact"],
                    row["enquiry_date"]
                );
            }

        }

        private void SearchByEnquirName(string enquirName)
        {
            try
            {
                // Initialize your Database helper class
                Database db = new Database();

                // Define the query to search by username
                string query = "SELECT enquir_id, enquir_name, enquir_email, enquir_contact, enquiry_date FROM enquiry_details WHERE enquir_name LIKE @enquirName";

                // Execute the query with a parameter
                DataTable dataTable = db.ExecuteQuery(query, new MySqlParameter[]
                {
                    new MySqlParameter("@EnquirName", MySqlDbType.VarChar) { Value = $"%{enquirName}%" } // Supports partial search
                });

                // Bind only the data to the pre-defined columns
                PopulateDataGridView(dataTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            // Get enquiry inputs from TextBoxes
            string enquirName = tbEnquirName.Text.Trim();
            string enquirEmail = tbEnquirEmail.Text.Trim();
            string enquirContact = tbEnquirContact.Text.Trim();
            string enquiryDate = dateTimePicker1.Text;

            // Query to check if enquir_name or enquir_email already exists
            string checkQuery = "SELECT COUNT(*) FROM enquiry_details WHERE enquir_name = @enquirName OR enquir_email = @enquirEmail";

            try
            {
                // Check for existing record
                object result = db.ExecuteScalar(checkQuery, new MySqlParameter[]
                {
            new MySqlParameter("@enquirName", MySqlDbType.VarChar) { Value = enquirName },
            new MySqlParameter("@enquirEmail", MySqlDbType.VarChar) { Value = enquirEmail }
                });

                // If the count is greater than 0, a record already exists
                if (Convert.ToInt32(result) > 0)
                {
                    MessageBox.Show("enquir with the same enquirname or enquiremail already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Exit the method without saving
                }

                // Query to save the new enquiry
                string saveQuery = "INSERT INTO enquiry_details(enquir_name, enquir_email, enquir_contact, enquiry_date) VALUES (@enquirName, @enquirEmail, @enquirContact, @enquirydate)";

                // Execute the save query
                db.ExecuteNonQuery(saveQuery, new MySqlParameter[]
                {
            new MySqlParameter("@enquirName", MySqlDbType.VarChar) { Value = enquirName },
            new MySqlParameter("@enquirEmail", MySqlDbType.VarChar) { Value = enquirEmail },
            new MySqlParameter("@enquirContact", MySqlDbType.VarChar) { Value = enquirContact },
            new MySqlParameter("@enquirydate", MySqlDbType.VarChar) { Value = enquiryDate }
                });

                MessageBox.Show("enquiry saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear the input fields
                clr_text();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving enquiry: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clr_text()
        {
            tbEnquirName.Clear();
            tbEnquirEmail.Clear();
            tbEnquirContact.Clear();
            tbEnquirName.Select();
            tbEnquirName.Select();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Get user inputs
            string currentEnquirName = tbEnquirName.Text; // Current name entered in the textbox
            string enquiremail = tbEnquirEmail.Text;
            string enquircontact = tbEnquirContact.Text;
            string enquirydate = dateTimePicker1.Text;

            // Define the UPDATE query
            string query = "UPDATE enquiry_details SET enquir_name = @enquirName, enquir_email = @enquirEmail, enquir_contact = @enquirContact, enquiry_date = @enquirydate WHERE enquir_name = @oldEnquirName";

            try
            {
                // Fetch the current name before updating (store it as the old value)
                string oldEnquirName = currentEnquirName;

                // Allow user to modify the text box to input the new name
                string newEnquirName = tbEnquirName.Text; // Assume this is updated after user edits

                // Execute the UPDATE query using ExecuteNonQuery
                db.ExecuteNonQuery(query, new MySqlParameter[]
                {
        new MySqlParameter("@enquirName", MySqlDbType.VarChar) { Value = newEnquirName },
        new MySqlParameter("@enquirEmail", MySqlDbType.VarChar) { Value = enquiremail },
        new MySqlParameter("@enquirContact", MySqlDbType.VarChar) { Value = enquircontact },
        new MySqlParameter("@enquirydate", MySqlDbType.VarChar) { Value = enquirydate },
        new MySqlParameter("@oldEnquirName", MySqlDbType.VarChar) { Value = oldEnquirName }
                });

                MessageBox.Show("Record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                clr_text();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            // Get the enquirname entered by the enquiry
            string enquirName = tbEnquirName.Text; // Assuming tbName is a TextBox for entering the enquirname

            // Define the SELECT query
            string query = "SELECT enquir_email, enquir_contact, enquiry_date FROM enquiry_details WHERE enquir_name = @enquirName";

            try
            {
                // Execute the SELECT query
                DataTable result = db.ExecuteQuery(query, new MySqlParameter[]
                {
                    new MySqlParameter("@enquirName", MySqlDbType.VarChar) { Value = enquirName }
                });

                // Check if the record exists
                if (result.Rows.Count > 0)
                {
                    // Populate text boxes with the fetched data
                    tbEnquirEmail.Text = result.Rows[0]["enquir_email"].ToString();
                    tbEnquirContact.Text = result.Rows[0]["enquir_contact"].ToString();
                    dateTimePicker1.Text = result.Rows[0]["enquiry_date"].ToString();

                    MessageBox.Show("Record fetched successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No record found for the given enquirname", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string enquirName = tbEnquirName.Text;
            string enquirEmail = tbEnquirEmail.Text;

            // Define the DELETE query
            string query = "DELETE FROM enquiry_details WHERE enquir_name = @enquirName AND enquir_email = @enquirEmail";

            try
            {
                // Execute the DELETE query using ExecuteNonQuery
                int rowsAffected = db.ExecuteNonQuery(query, new MySqlParameter[]
                {
                    new MySqlParameter("@enquirName", MySqlDbType.VarChar) { Value = enquirName },
                    new MySqlParameter("@enquirEmail", MySqlDbType.VarChar) { Value = enquirEmail }
                });

                if (rowsAffected > 0)
                {
                    MessageBox.Show("enquir deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Optionally clear the input fields
                    clr_text();
                }
                else
                {
                    MessageBox.Show("No enquir found with the given name.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting enquir: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            SearchByEnquirName(textBox5.Text);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
