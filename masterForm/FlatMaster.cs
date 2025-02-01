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

                // Query to save the new Flat
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

        private void LoadFlatTypeComboBox()
        {
            string query = "SELECT flat_type_name FROM flat_type_details";
            DataTable dt = db.ExecuteQuery(query);

            if (dt.Rows.Count > 0)
            {
                cbFlatType.Items.Clear(); // Clear existing items

                foreach (DataRow row in dt.Rows)
                {
                    cbFlatType.Items.Add(row["flat_type_name"].ToString());
                }
            }
        }

        private void FlatMaster_Load(object sender, EventArgs e)
        {
            LoadFlatTypeComboBox();
            flatDataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
            flatDataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            flatDataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 11, FontStyle.Regular);

            try
            {
                string query = "SELECT * FROM flat_details";
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
            flatDataGrid.Rows.Clear();
            foreach (DataRow row in dataTable.Rows)
            {
                flatDataGrid.Rows.Add(
                    row["flat_id"],
                    row["quatation_date"],
                    row["discription"],
                    row["customer_contact"],
                    row["building_name"],
                    row["flat_type_name"],
                    row["price_per_sq_ft"],
                    row["base_price"],
                    row["additionl_charges"],
                    row["discount"],
                    row["total_amount"],
                    row["down_payment"],
                    row["Payment_mode"]
                );
            }
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }
    }
}
  
