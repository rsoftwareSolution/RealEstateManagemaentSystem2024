using MySql.Data.MySqlClient;
using RealStateManagementSystem.mainForm;
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

namespace RealEstateManagemaentSystem2024.mainForm
{
    public partial class Dashboard : Form
    {

        private Database db;
        private string userName;
        private string contact;

        public Dashboard(string userName, string contact)
        {
            InitializeComponent();
            // Assign the passed values to the class-level variables
            this.userName = userName;
            this.contact = contact;
        }

        public Dashboard()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            userDetailsPanelMDI.Hide();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            userDetailsPanelMDI.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            userDetailsPanelMDI.Hide();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void Masters_Opening(object sender, CancelEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if(userDetailsPanelMDI.Visible)
            {
                userDetailsPanelMDI.Hide();
            } 
            else
            {
                userDetailsPanelMDI.Show();
            }
            lbmailId.Text = userName;
            lbContact.Text = contact;
        }

        private void FetchUserDetails(string username)
        {
            try
            {
                // Define the SELECT query
                string query = "SELECT user_contact FROM user WHERE user_name = @Username";

                // Use MySqlParameter instead of SqlParameter
                MySqlParameter[] parameters = {
            new MySqlParameter("@Username", MySqlDbType.VarChar) { Value = username }
        };

                // Execute query and get results
                DataTable result = db.ExecuteQuery(query, parameters);

                if (result.Rows.Count > 0)
                {
                    // Display user details
                    lbContact.Text = result.Rows[0]["user_contact"].ToString();
                    lbmailId.Text = username;  // Assuming username is already passed into this method
                }
                else
                {
                    // User not found
                    MessageBox.Show("User not found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
