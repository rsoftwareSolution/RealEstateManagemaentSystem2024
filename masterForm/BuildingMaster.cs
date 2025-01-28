using MySql.Data.MySqlClient;
using RealEstateManagemaentSystem2024.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        private async void btnSave_Click(object sender, EventArgs e)
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

        private async void tbPinCode_TabIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void tbPinCode_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbPinCode_Enter(object sender, EventArgs e)
        {

        }

        private void BuildingMaster_Load(object sender, EventArgs e)
        {

        }
    }
 }
