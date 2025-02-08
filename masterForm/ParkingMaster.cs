using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace RealEstateManagemaentSystem2024.MasterForm
{
    public partial class ParkingMaster : Form
    {
        Database db = new Database();
        private AutoIdGenerator idGenerator = new AutoIdGenerator("PARK", 4);

        public ParkingMaster()
        {
            InitializeComponent();
        }

        private void ParkingMaster_Load(object sender, EventArgs e)
        {
            // Assuming 'dataGridView1' is your DataGridView control
            ParkingDataGrid.Font = new Font("Times New Roman", 10, FontStyle.Regular); // Set font name, size, and style

            tbVehicleName.Select();

            //This code represents data load into grid 
            try
            {
                // Initialize your Database helper class
                Database db = new Database();

                // Define the query
                string query = "SELECT * FROM parking_details";

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
            ParkingDataGrid.Rows.Clear();

            // Loop through each row in the DataTable and add it to the DataGridView
            foreach (DataRow row in dataTable.Rows)
            {
                ParkingDataGrid.Rows.Add(
                    row["parking_id"],
                    row["available_parking"],
                    row["total_parking"],
                    row["vehicle_name"]
 
                );
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string parkingid = tbParkingId.Text.Trim();
            string availableparking = tbAvailableParking.Text.Trim();
            string totalparking = tbTotalParking.Text.Trim();
            string vehiclename = tbVehicleName.Text.Trim();  

            string query = "INSERT INTO parking_details (parking_id, available_parking, total_parking, vehicle_name) VALUES (@id, @available, @total, @vehicle)";
            db.ExecuteNonQuery(query,
                new MySqlParameter("@id", parkingid),
                new MySqlParameter("@available", availableparking),
                new MySqlParameter("@total", totalparking),
                new MySqlParameter("@vehicle", vehiclename));
            Console.WriteLine($"Parking details added successfully with ID: {parkingid}");
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            string parkingId = tbParkingId.Text.Trim();

            string query = "SELECT vehicle_name, available_parking, total_parking FROM parking_details WHERE parking_id = @parkingId";

            DataTable result = db.ExecuteQuery(query, new MySqlParameter[]
                 {
                    new MySqlParameter("@parkingId", MySqlDbType.VarChar) { Value = parkingId }
                 });
            if (result.Rows.Count > 0)
            {
                // Populate text boxes with the fetched data
                tbAvailableParking.Text = result.Rows[0]["available_parking"].ToString();
                tbTotalParking.Text = result.Rows[0]["total_parking"].ToString();
                tbVehicleName.Text = result.Rows[0]["vehicle_name"].ToString();

                MessageBox.Show("Record fetched successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No record found for the given enquirname", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string parkingid = tbParkingId.Text.Trim();
            string availableparking = tbAvailableParking.Text.Trim();
            string totalparking = tbTotalParking.Text.Trim();
            string vehiclename = tbVehicleName.Text.Trim();

            string query = "UPDATE parking_details SET available_parking=@available, total_parking=@total, vehicle_name=@vehicle WHERE parking_id=@id";
            db.ExecuteNonQuery(query,
                new MySqlParameter("@id", parkingid),
                new MySqlParameter("@available", availableparking),
                new MySqlParameter("@total", totalparking),
                new MySqlParameter("@vehicle", vehiclename));
            Console.WriteLine($"Parking details updated successfully for ID: {parkingid}");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string parkingid = tbParkingId.Text.Trim();
            string query = "DELETE FROM parking_details WHERE parking_id=@id";

            db.ExecuteNonQuery(query, new MySqlParameter("@id", parkingid));
            Console.WriteLine($"Parking details deleted successfully for ID: {parkingid}");
        }
    }

}