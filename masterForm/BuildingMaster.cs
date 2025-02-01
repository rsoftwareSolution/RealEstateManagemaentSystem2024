using MySql.Data.MySqlClient;
using RealEstateManagemaentSystem2024.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace RealStateManagementSystem.config
{
    public partial class BuildingMaster : Form
    {

        Database database = new Database();

        public BuildingMaster()
        {
            InitializeComponent();
        }

        private void BuildingMaster_Load(object sender, EventArgs e)
        {
            // Assuming 'dataGridView1' is your DataGridView control
            buildingDataGrid.Font = new Font("Times New Roman", 11, FontStyle.Regular); // Set font name, size, and style
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private async void tbPinCode_Leave(object sender, EventArgs e)
        {
            string pincode = tbPinCode.Text;  // Assuming you have a TextBox for the pincode

            // Validate that pincode is entered
            if (string.IsNullOrEmpty(pincode))
            {
                MessageBox.Show("Please enter a valid pincode.");
                return;
            }

            // Fetch location details (state, district, village) using Pincode API
            var location = await PincodeAPI.GetLocationDetailsFromPincodeAsync(pincode);

            if (string.IsNullOrEmpty(location.state) || string.IsNullOrEmpty(location.district) || string.IsNullOrEmpty(location.village))
            {
                MessageBox.Show("Failed to retrieve location details.");
                return;  // Exit if location details are invalid
            }

            // Auto-fill the State, District, and Village fields
            tbState.Text = location.state;        // Assuming you have a TextBox for the state
            tbDist.Text = location.district;  // Assuming you have a TextBox for the district
            tbTaluka.Text = location.village;    // Assuming you have a TextBox for the village

            MessageBox.Show("Location details fetched successfully.");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }
    }
 }
