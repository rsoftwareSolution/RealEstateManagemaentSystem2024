using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            string query = "INSERT INTO Users (user_name, user_email, user_contact, user_password) VALUES (@userName, @email, @contact, @password)";

            try
            {
                db.ExecuteNonQuery(query, new SqlParameter[]
                {
                    new SqlParameter("@userName", userName),
                    new SqlParameter("@email", email),
                    new SqlParameter("@contact", contact),
                    new SqlParameter("@password", password)
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
