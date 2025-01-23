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

namespace RealStateManagementSystem.masterForm
{
    public partial class FlatMaster : Form
    {
        Database db = new Database();
        private object rate;

        public FlatMaster()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            // Get user inputs from TextBoxes
            string totalfloor = tbFloor.Text.Trim();
            string rate = tbRate.Text.Trim();
            string availableflatscount = tbFlatCount.Text.Trim();
            
            // Query to check if user_name or user_email already exists
            string checkQuery = "SELECT COUNT(*) FROM Flat_details WHERE total_floor = @totalfloor , rate = @rate OR available_flats_count = @availableflatscount";

            try
            {
                // Check for existing record
                object result = db.ExecuteScalar(checkQuery, new MySqlParameter[]
                {
            new MySqlParameter("@totalfloor", MySqlDbType.VarChar) { Value = totalfloor },
            new MySqlParameter("@rate", MySqlDbType.VarChar) { Value = rate },
            new MySqlParameter("@availableflatscount", MySqlDbType.VarChar) { Value = availableflatscount }
                });

                // If the count is greater than 0, a record already exists
                if (Convert.ToInt32(result) > 0)
                {
                    MessageBox.Show("flat with the same totalfloor or rate already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Exit the method without saving
                }

                // Query to save the new user
                string saveQuery = "INSERT INTO flat_details (building_id, total_floor, rate, available_flats_count) VALUES (@buildingId, @totalFloor, @rate, @availableFlatsCount)";


                // Execute the save query
                db.ExecuteNonQuery(saveQuery, new MySqlParameter[]
                {
            new MySqlParameter("@totalfloor", MySqlDbType.VarChar) { Value = totalfloor },
            new MySqlParameter("@rate", MySqlDbType.VarChar) { Value = rate },
            new MySqlParameter("@availableflatscount", MySqlDbType.VarChar) { Value = availableflatscount }
                });

                MessageBox.Show("Flat saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear the input fields
                clr_text();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving Flat: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void clr_text()
        {
            tbFloor.Clear();
            tbRate.Clear();
            tbFlatCount.Clear();
            tbFloor.Select();
        }

        private void FlatMaster_Load(object sender, EventArgs e)
        {

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            // Get the username entered by the user
            string totalfloor = tbFloor.Text; // Assuming tbName is a TextBox for entering the username

            // Define the SELECT query
            string query = "SELECT total_floor, rate, available_flats_count FROM flat_details WHERE total_floor = @totalfloor";

            try
            {
                // Execute the SELECT query
                DataTable result = db.ExecuteQuery(query, new MySqlParameter[]
                {
                    new MySqlParameter("@totalfloor", MySqlDbType.VarChar) { Value = totalfloor }
                });

                // Check if the record exists
                if (result.Rows.Count > 0)
                {
                    // Populate text boxes with the fetched data
                    tbFloor.Text = result.Rows[0]["total_floor"].ToString();
                    tbRate.Text = result.Rows[0]["rate"].ToString();
                    tbFlatCount.Text = result.Rows[0]["available_flats_count"].ToString();

                    MessageBox.Show("Record fetched successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No record found for the given totalfloor", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Get user inputs
            string total_floor = tbFloor.Text;
            string emarateil = tbRate.Text;
            string available_flats_count = tbFlatCount.Text;


            // Define the UPDATE query
            string query = "UPDATE flat_details SET total_floor = @totalfloor, rate = @rate, available_flats_count = @rate WHERE total_floor = @totalfloor";

            try
            {
                // Execute the UPDATE query using ExecuteNonQuery
                db.ExecuteNonQuery(query, new MySqlParameter[]
                {

                new MySqlParameter("@totalfloor", MySqlDbType.VarChar) { Value = total_floor },
            new MySqlParameter("@rate", MySqlDbType.VarChar) { Value = rate },
            new MySqlParameter("@available_flats_count", MySqlDbType.VarChar) { Value = available_flats_count },
          
                });




                MessageBox.Show("Record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                clr_text();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

            string totalfloor = tbFloor.Text;
            string rate = tbRate.Text;
            string available_flats_count = tbFlatCount.Text;

            // Define the DELETE query
            string query = "DELETE FROM flat_details WHERE total_floor = @totalfloor , rate = @rate AND available_flats_count = @available_flats_count";

            try
            {
                // Execute the DELETE query using ExecuteNonQuery
                int rowsAffected = db.ExecuteNonQuery(query, new MySqlParameter[]
                {
                    new MySqlParameter("@totalfloor", MySqlDbType.VarChar) { Value = totalfloor },
                    new MySqlParameter("@rate", MySqlDbType.VarChar) { Value = rate },
                     new MySqlParameter("@available_flats_count", MySqlDbType.VarChar) { Value = available_flats_count }
                });

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Flat deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Optionally clear the input fields
                    clr_text();
                }
                else
                {
                    MessageBox.Show("No Flat found with the given name.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting Flat: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
  
