using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace RealEstateManagemaentSystem2024.MasterForm
{
    public partial class BuildingMaster : Form
    {
        Database db = new Database();
        private AutoIdGenerator idGenerator = new AutoIdGenerator("BLD", 4); // Unique ID generator for buildings

        public BuildingMaster()
        {
            InitializeComponent();
        }

        private void BuildingMaster_Load(object sender, EventArgs e)
        {
            ClearFields();
            // Formatting DataGridView
            tbBuildingName.Select();

            InitializeBuildingGrid();
            LoadBuildingData();
        }

        private void InitializeBuildingGrid()
        {
            // Clear existing columns to avoid duplication
            buildingDataGrid.Columns.Clear();

            // Disable auto column generation
            buildingDataGrid.AutoGenerateColumns = false;

            buildingDataGrid.Font = new Font("Times New Roman", 10, FontStyle.Regular);


            // Set DataGridView Header Height
            buildingDataGrid.ColumnHeadersHeight = 35;
            buildingDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Set DataGridView row height
            buildingDataGrid.RowTemplate.Height = 30; // Adjust row height for better visibility

            // Define columns with custom widths
            buildingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "BuildingID", HeaderText = "ID", DataPropertyName = "building_id", Width = 100 });
            buildingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "BuildingName", HeaderText = "Project Name", DataPropertyName = "building_or_project_name", Width = 200 });
            buildingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Pincode", HeaderText = "Pincode", DataPropertyName = "building_location_pincode", Width = 100 });
            buildingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "State", HeaderText = "State", DataPropertyName = "building_state", Width = 150 });
            buildingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "District", HeaderText = "District", DataPropertyName = "building_district", Width = 150 });
            buildingDataGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Village", HeaderText = "Village", DataPropertyName = "building_village", Width = 200 });

            // Ensure column headers are displayed properly
            buildingDataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }


        // Method to load data into DataGridView
        private void LoadBuildingData()
        {
            try
            {
                string query = "SELECT * FROM building_details";
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
            buildingDataGrid.Rows.Clear();

            foreach (DataRow row in dataTable.Rows)
            {
                buildingDataGrid.Rows.Add(
                    row["building_id"],
                    row["building_or_project_name"],
                    row["building_location_pincode"],
                    row["building_state"],
                    row["building_district"],
                    row["building_village"]
                );
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string buildingId = idGenerator.GenerateNextId();
            string name = tbBuildingName.Text.Trim();
            string pincode = tbBuildingPincode.Text.Trim();
            string state = tbBuildingState.Text.Trim();
            string district = tbBuildingDistrict.Text.Trim();
            string village = tbBuildingVillage.Text.Trim();

            string query = "INSERT INTO building_details (building_id, building_or_project_name, building_location_pincode, building_state, building_district, building_village) " +
                           "VALUES (@id, @name, @pincode, @state, @district, @village)";

            db.ExecuteNonQuery(query,
                new MySqlParameter("@id", buildingId),
                new MySqlParameter("@name", name),
                new MySqlParameter("@pincode", pincode),
                new MySqlParameter("@state", state),
                new MySqlParameter("@district", district),
                new MySqlParameter("@village", village));

            MessageBox.Show($"Building details added successfully with ID: {buildingId}");
            ClearFields();
            LoadBuildingData(); // Refresh the grid
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            string buildingId = tbBuildingId.Text.Trim();

            string query = "SELECT building_or_project_name, building_location_pincode, building_state, building_district, building_village " +
                           "FROM building_details WHERE building_id = @buildingId";

            DataTable result = db.ExecuteQuery(query, new MySqlParameter("@buildingId", buildingId));

            if (result.Rows.Count > 0)
            {
                tbBuildingName.Text = result.Rows[0]["building_or_project_name"].ToString();
                tbBuildingPincode.Text = result.Rows[0]["building_location_pincode"].ToString();
                tbBuildingState.Text = result.Rows[0]["building_state"].ToString();
                tbBuildingDistrict.Text = result.Rows[0]["building_district"].ToString();
                tbBuildingVillage.Text = result.Rows[0]["building_village"].ToString();

                MessageBox.Show("Record fetched successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No record found for the given ID", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string buildingId = tbBuildingId.Text.Trim();
            string name = tbBuildingName.Text.Trim();
            string pincode = tbBuildingPincode.Text.Trim();
            string state = tbBuildingState.Text.Trim();
            string district = tbBuildingDistrict.Text.Trim();
            string village = tbBuildingVillage.Text.Trim();

            string query = "UPDATE building_details SET building_or_project_name=@name, building_location_pincode=@pincode, " +
                           "building_state=@state, building_district=@district, building_village=@village WHERE building_id=@id";

            db.ExecuteNonQuery(query,
                new MySqlParameter("@id", buildingId),
                new MySqlParameter("@name", name),
                new MySqlParameter("@pincode", pincode),
                new MySqlParameter("@state", state),
                new MySqlParameter("@district", district),
                new MySqlParameter("@village", village));

            MessageBox.Show($"Building details updated successfully for ID: {buildingId}");
            ClearFields();
            LoadBuildingData();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string buildingId = tbBuildingId.Text.Trim();
            string query = "DELETE FROM building_details WHERE building_id=@id";

            db.ExecuteNonQuery(query, new MySqlParameter("@id", buildingId));

            MessageBox.Show($"Building details deleted successfully for ID: {buildingId}");
            ClearFields();
            LoadBuildingData();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ClearFields()
        {
            tbBuildingName.Clear();
            tbBuildingPincode.Clear();
            tbBuildingState.Clear();
            tbBuildingDistrict.Clear();
            tbBuildingVillage.Clear();

            // Generate a new ID for the next entry
            GenerateNewBuildingId();
        }


        private void GenerateNewBuildingId()
        {
            try
            {
                // Corrected query for MySQL
                string query = @"SELECT building_id FROM building_details WHERE building_id LIKE 'BUG%' ORDER BY CONVERT(SUBSTRING(building_id, 5, CHAR_LENGTH(building_id) - 4), UNSIGNED) DESC LIMIT 1";

                // Execute the query
                object maxIdObj = db.ExecuteScalar(query);

                int nextId = 1; // Default if no record exists

                if (maxIdObj != null && maxIdObj != DBNull.Value)
                {
                    string lastId = maxIdObj.ToString();  // Example: "CUST0021"

                    if (lastId.StartsWith("BUG") && int.TryParse(lastId.Substring(4), out int numericPart))
                    {
                        nextId = numericPart + 1; // Increment ID
                    }
                }

                // Generate new ID with zero padding (CUST0001, CUST0002, ...)
                tbBuildingId.Text = $"BUG{nextId:D4}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating building ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbBuildingId.Text = string.Empty;  // Reset textbox on error
            }
        }
    }
}
