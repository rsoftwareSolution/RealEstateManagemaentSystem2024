using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealStateManagementSystem.config
{
    public partial class BuildingMaster : Form
    {

        Database db = new Database();

        public BuildingMaster()
        {
            InitializeComponent();
        }

        private void BuildingMaster_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {

            // Get the username entered by the user
            string buildName = tbBuildName.Text; // Assuming tbName is a TextBox for entering the username

            // Define the SELECT query
            string query = "SELECT build_adrress, build_FlatNo, buil_floorNo FROM user WHERE build_name = @buildName";

            try
            {
                // Execute the SELECT query
                DataTable result = db.ExecuteQuery(query, new MySqlParameter[]
                {
                    new MySqlParameter("@buildName", MySqlDbType.VarChar) { Value = buildName }
                });

                // Check if the record exists
                if (result.Rows.Count > 0)
                {
                    // Populate text boxes with the fetched data
                    tbAddress.Text = result.Rows[0]["building_address"].ToString();
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

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
    }
