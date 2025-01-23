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
        private object buildaddress;
        private object buildName;

        public BuildingMaster()
        {
            InitializeComponent();
        }

        private void BuildingMaster_Load(object sender, EventArgs e)
        {
            tbBuildId.ReadOnly = true;
            tbBuildName.Select();

            //This code represents data load into grid 
            try
            {
                // Initialize your Database helper class
                Database db = new Database();

                // Define the query
                string query = "SELECT * FROM building_details";

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
            buildingDataGrid.Rows.Clear();

            // Loop through each row in the DataTable and add it to the DataGridView
            foreach (DataRow row in dataTable.Rows)
            {
                buildingDataGrid.Rows.Add(
                    row["build_id"],
                    row["build_name"],
                    row["build_address"]
                );
            }

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
        private void tabPage1_Click(object sender, EventArgs e)
        {
            btnEdit.Enabled = true;
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            // Get the username entered by the building
            string buildName = tbBuildName.Text; // Assuming tbName is a TextBox for entering the  buildName

            // Define the SELECT query
            string query = "SELECT build_address FROM building_details WHERE  build_Name = @buildName";

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
                    tbAddress.Text = result.Rows[0]["build_address"].ToString();


                    MessageBox.Show("Record fetched successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No record found for the given  buildName", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void SearchByUsername(string buildName)
        {
            try
            {
                // Initialize your Database helper class
                Database db = new Database();

                // Define the query to search by buildname
                string query = "SELECT build_id, build_name, build_address FROM building_details WHERE build_name LIKE @buildName";

                // Execute the query with a parameter
                DataTable dataTable = db.ExecuteQuery(query, new MySqlParameter[]
                {
                    new MySqlParameter("@buildName", MySqlDbType.VarChar) { Value = $"%{buildName}%" } // Supports partial search
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
            // Get user inputs from TextBoxes
            string buildName = tbBuildName.Text.Trim();
            string buildaddress = tbAddress.Text.Trim();

            // Query to check if build_name or address already exists
            string checkQuery = "SELECT COUNT(*) FROM building_details WHERE build_name = @buildName OR build_address = @buildaddress";

            try
            {
                // Check for existing record
                object result = db.ExecuteScalar(checkQuery, new MySqlParameter[]
                {
            new MySqlParameter("@buildName", MySqlDbType.VarChar) { Value = buildName },
            new MySqlParameter("@buildaddress", MySqlDbType.VarChar) { Value = buildaddress }
                });

                // If the count is greater than 0, a record already exists
                if (Convert.ToInt32(result) > 0)
                {
                    MessageBox.Show("Build with the same buildname or buildaddress already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Exit the method without saving
                }

                // Query to save the new building
                string saveQuery = "INSERT INTO building_details (build_name, build_address) VALUES (@buildName, @buildaddress)";

                // Execute the save query
                db.ExecuteNonQuery(saveQuery, new MySqlParameter[]
                {
            new MySqlParameter("@buildName", MySqlDbType.VarChar) { Value = buildName },
            new MySqlParameter("@buildaddress", MySqlDbType.VarChar) { Value = buildaddress },
                });

                MessageBox.Show("Building details saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear the input fields
                clr_text();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving building: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clr_text()
        {
            tbBuildName.Clear();
            tbAddress.Clear();
            tbBuildName.Select();
            tbBuildId.Select();

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string buildName = tbBuildName.Text;
            string buildaddress = tbAddress.Text;

            // Define the DELETE query
            string query = "DELETE FROM building_details WHERE build_name = @buildName AND build_address = @buildaddress";

            try
            {
                // Execute the DELETE query using ExecuteNonQuery
                int rowsAffected = db.ExecuteNonQuery(query, new MySqlParameter[]
                {
                    new MySqlParameter("@buildName", MySqlDbType.VarChar) { Value = buildName },
                    new MySqlParameter("@buildaddress", MySqlDbType.VarChar) { Value = buildaddress }
                });

                if (rowsAffected > 0)
                {
                    MessageBox.Show("building deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Optionally clear the input fields
                    clr_text();
                }
                else
                {
                    MessageBox.Show("No building found with the given name.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting building: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Get user inputs
            string buildName = tbBuildName.Text;
            string buildaddress = tbAddress.Text;

            // Define the UPDATE query
            string query = "UPDATE building_details SET buildaddress = @buildaddress  WHERE build_name = @buildName";

            try
            {
                // Execute the UPDATE query using ExecuteNonQuery
                db.ExecuteNonQuery(query, new MySqlParameter[]
                {
            new MySqlParameter("@buildaddress", MySqlDbType.VarChar) { Value =buildaddress },
            new MySqlParameter("@buildName", MySqlDbType.VarChar) { Value = buildName }
                });

                MessageBox.Show("Record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                clr_text();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void buildingDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            SearchBybuildName(textBox5.Text);
        }

        private void SearchBybuildName(string text)
        {
            throw new NotImplementedException();
        }
    }
    
    }
