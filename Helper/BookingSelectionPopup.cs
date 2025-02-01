using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealEstateManagemaentSystem2024.Helper
{
    public partial class BookingSelectionPopup : Form
    {

        public int SelectedBookingId { get; private set; }
        public double SelectedTotalAmount { get; private set; }
        public double SelectedTotalPaid { get; private set; }
        public double SelectedRefund { get; private set; }

        public BookingSelectionPopup(DataTable bookingData)
        {
            InitializeComponent();
            PopulateDataGridView(bookingData);
        }

        private void BookingSelectionPopup_Load(object sender, EventArgs e)
        {          
                if (bookingListPopupDataGrid.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a booking.", "No Booking Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get the selected booking details
                SelectedBookingId = Convert.ToInt32(bookingListPopupDataGrid.SelectedRows[0].Cells["booking_id"].Value);
                SelectedTotalAmount = Convert.ToDouble(bookingListPopupDataGrid.SelectedRows[0].Cells["total_amount"].Value);
                SelectedTotalPaid = Convert.ToDouble(bookingListPopupDataGrid.SelectedRows[0].Cells["total_paid"].Value);
                SelectedRefund = Convert.ToDouble(bookingListPopupDataGrid.SelectedRows[0].Cells["refund"].Value);

                this.DialogResult = DialogResult.OK; // Close the form and indicate selection was made
                this.Close(); 
        }

        // Populate the DataGridView with booking data
        private void PopulateDataGridView(DataTable bookingData)
        {
            bookingListPopupDataGrid.Rows.Clear();

            foreach (DataRow row in bookingData.Rows)
            {
                bookingListPopupDataGrid.Rows.Add(
                    row["booking_id"],
                    row["booking_date"],
                    row["total_amount"],
                    row["total_paid"],
                    row["refund"]
                );
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
