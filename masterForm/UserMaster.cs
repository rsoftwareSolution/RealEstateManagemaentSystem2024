using MySql.Data.MySqlClient;
using RealStateManagementSystem.mainForm;
using System;
using System.Data;
using System.Windows.Forms;

namespace RealEstateManagemaentSystem2024.masterForm
{
    public partial class UserMaster : Form
    {

        Database db = new Database();

        public UserMaster()
        {
            InitializeComponent();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            // Get the username entered by the user
            string userName = tbName.Text; // Assuming tbName is a TextBox for entering the username

            // Define the SELECT query
            string query = "SELECT user_email, user_contact, user_password FROM user WHERE user_name = @userName";

            try
            {
                // Execute the SELECT query
                DataTable result = db.ExecuteQuery(query, new MySqlParameter[]
                {
                    new MySqlParameter("@userName", MySqlDbType.VarChar) { Value = userName }
                });

                // Check if the record exists
                if (result.Rows.Count > 0)
                {
                    // Populate text boxes with the fetched data
                    tbEmail.Text = result.Rows[0]["user_email"].ToString();
                    tbContact.Text = result.Rows[0]["user_contact"].ToString();
                    tbPassword.Text = result.Rows[0]["user_password"].ToString();

                    MessageBox.Show("Record fetched successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No record found for the given username.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void UserMaster_Load(object sender, EventArgs e)
        {
            tbUserID.ReadOnly = true;
            tbName.Select();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Get user inputs from TextBoxes
            string userName = tbName.Text.Trim();
            string email = tbEmail.Text.Trim();
            string contact = tbContact.Text.Trim();
            string password = tbPassword.Text.Trim();

            // Query to check if user_name or user_email already exists
            string checkQuery = "SELECT COUNT(*) FROM user WHERE user_name = @userName OR user_email = @userEmail";

            try
            {
                // Check for existing record
                object result = db.ExecuteScalar(checkQuery, new MySqlParameter[]
                {
            new MySqlParameter("@userName", MySqlDbType.VarChar) { Value = userName },
            new MySqlParameter("@userEmail", MySqlDbType.VarChar) { Value = email }
                });

                // If the count is greater than 0, a record already exists
                if (Convert.ToInt32(result) > 0)
                {
                    MessageBox.Show("User with the same username or email already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Exit the method without saving
                }

                // Query to save the new user
                string saveQuery = "INSERT INTO user (user_name, user_email, user_contact, user_password) VALUES (@userName, @userEmail, @userContact, @userPassword)";

                // Execute the save query
                db.ExecuteNonQuery(saveQuery, new MySqlParameter[]
                {
            new MySqlParameter("@userName", MySqlDbType.VarChar) { Value = userName },
            new MySqlParameter("@userEmail", MySqlDbType.VarChar) { Value = email },
            new MySqlParameter("@userContact", MySqlDbType.VarChar) { Value = contact },
            new MySqlParameter("@userPassword", MySqlDbType.VarChar) { Value = password }
                });

                MessageBox.Show("User saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear the input fields
                clr_text();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving user: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clr_text()
        {
            tbName.Clear();
            tbEmail.Clear();
            tbContact.Clear();
            tbPassword.Clear();
            tbName.Select();
            this.Hide();
            Login login = new Login();
            login.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Get user inputs
            string userName = tbName.Text;
            string email = tbEmail.Text;
            string contact = tbContact.Text;
            string password = tbPassword.Text;

            // Define the UPDATE query
            string query = "UPDATE user SET user_email = @userEmail, user_contact = @userContact, user_password = @userPassword WHERE user_name = @userName";

            try
            {
                // Execute the UPDATE query using ExecuteNonQuery
                db.ExecuteNonQuery(query, new MySqlParameter[]
                {
            new MySqlParameter("@userEmail", MySqlDbType.VarChar) { Value = email },
            new MySqlParameter("@userContact", MySqlDbType.VarChar) { Value = contact },
            new MySqlParameter("@userPassword", MySqlDbType.VarChar) { Value = password },
            new MySqlParameter("@userName", MySqlDbType.VarChar) { Value = userName }
                });

                MessageBox.Show("Record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                clr_text();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string userName = tbName.Text;
            string userEmail = tbEmail.Text;

            // Define the DELETE query
            string query = "DELETE FROM user WHERE user_name = @userName AND user_email = @userEmail";

            try
            {
                // Execute the DELETE query using ExecuteNonQuery
                int rowsAffected = db.ExecuteNonQuery(query, new MySqlParameter[]
                {
                    new MySqlParameter("@userName", MySqlDbType.VarChar) { Value = userName },
                    new MySqlParameter("@userEmail", MySqlDbType.VarChar) { Value = userEmail }
                });

                if (rowsAffected > 0)
                {
                    MessageBox.Show("User deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Optionally clear the input fields
                    clr_text();
                }
                else
                {
                    MessageBox.Show("No user found with the given name.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting user: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}











