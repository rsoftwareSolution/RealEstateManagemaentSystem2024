using MySql.Data.MySqlClient;
using System;
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

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            tbUserID.Visible = true;
        }

        private void UserMaster_Load(object sender, EventArgs e)
        {
            tbUserID.ReadOnly = true;
            tbName.Select();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Get user inputs
            string userName = tbName.Text;
            string email = tbEmail.Text;
            string contact = tbContact.Text;
            string password = tbPassword.Text;

            // Define the INSERT query
            string query = "INSERT INTO user (user_name, user_email, user_contact, user_password) VALUES (@userName, @userEmail, @userContact, @userPassword)";

            try
            {
                // Assuming db is your Database object with MySQL connection methods
                db.ExecuteNonQuery(query, new MySqlParameter[]  // Use MySqlParameter here
                {
            new MySqlParameter("@userName", MySqlDbType.VarChar) { Value = userName },
            new MySqlParameter("@userEmail", MySqlDbType.VarChar) { Value = email },
            new MySqlParameter("@userContact", MySqlDbType.VarChar) { Value = contact },
            new MySqlParameter("@userPassword", MySqlDbType.VarChar) { Value = password }
                });

                MessageBox.Show("User inserted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inserting user: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
