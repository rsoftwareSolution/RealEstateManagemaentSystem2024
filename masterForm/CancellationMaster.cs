using MySql.Data.MySqlClient;
using RealEstateManagemaentSystem2024.Helper;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace RealStateManagementSystem.masterForm
{
    public partial class CancellationMaster : Form
    {

        Database db = new Database();

        public CancellationMaster()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void CancellationMaster_Load(object sender, EventArgs e)
        {
            ClearForm();
            cancelationDataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 11, FontStyle.Regular);

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click_1(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ClearForm()
        {
            tbCancelId.Text = GenerateCancelId();
            tbBookingCustContact.Clear();
            dtpCancelDate.Value = DateTime.Now; // Reset to current date
            tbTotalAmount.Clear();
            tbTotalPaid.Clear();
            tbRefund.Clear();

            tbBookingCustContact.Select();
        }

        private string GenerateCancelId()
        {
            try
            {
                string query = @"SELECT cancel_id FROM cancellation_details WHERE cancel_id LIKE 'CANCEL%' 
                         ORDER BY CONVERT(SUBSTRING(cancel_id, 7, CHAR_LENGTH(cancel_id) - 6), UNSIGNED) 
                         DESC LIMIT 1";

                object maxIdObj = db.ExecuteScalar(query);

                int nextId = 1; // Default if no record exists

                if (maxIdObj != null && maxIdObj != DBNull.Value)
                {
                    string lastId = maxIdObj.ToString();  // Example: "CANCEL0021"

                    if (lastId.StartsWith("CAN") && int.TryParse(lastId.Substring(6), out int numericPart))
                    {
                        nextId = numericPart + 1; // Increment ID
                    }
                }

                return $"CANCEL{nextId:D4}"; // Format ID as CANCEL0001, CANCEL0002, ...
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating Cancel ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }



        private void btnSave_Click(object sender, EventArgs e)
        {
            // Get the customer contact from the TextBox
            long customerContact = Convert.ToInt64(tbBookingCustContact.Text);

            // Fetch the necessary data from form inputs
            double refundAmount = Convert.ToDouble(tbRefund.Text); // This should be the final calculated refund amount
            double totalAmount = Convert.ToDouble(tbTotalAmount.Text);
            double totalPaid = Convert.ToDouble(tbTotalPaid.Text);

            // Initialize the AutoIdGenerator for generating the cancel_id with prefix "CANC" and numeric length of 4
            AutoIdGenerator cancelIdGenerator = new AutoIdGenerator("CANC", 4);

            try
            {
                // Generate the next cancel_id
                string cancelId = cancelIdGenerator.GenerateNextId();

                // Insert cancellation record with generated cancel_id
                string cancellationQuery = "INSERT INTO cancellation_details (cancel_id, booking_cust_contact, cancel_date, total_amount, total_paid, refund) " +
                                           "VALUES (@cancelId, @custContact, @cancelDate, @totalAmount, @totalPaid, @refundAmount)";

                db.ExecuteNonQuery(cancellationQuery, new MySqlParameter[]
                {
                    new MySqlParameter("@cancelId", MySqlDbType.VarChar) { Value = cancelId },
                    new MySqlParameter("@custContact", MySqlDbType.Int64) { Value = customerContact },
                    new MySqlParameter("@cancelDate", MySqlDbType.VarChar) { Value = DateTime.Now.ToString("yyyy-MM-dd") },
                    new MySqlParameter("@totalAmount", MySqlDbType.Double) { Value = totalAmount },
                    new MySqlParameter("@totalPaid", MySqlDbType.Double) { Value = totalPaid },
                    new MySqlParameter("@refundAmount", MySqlDbType.Double) { Value = refundAmount }
                });

                MessageBox.Show("Cancellation processed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // After saving the cancellation, delete the booking (you can perform this in a separate method or here)
                DeleteBooking(customerContact);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving cancellation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteBooking(long customerContact)
        {
            // Query to delete the booking based on customer contact
            string deleteBookingQuery = "DELETE FROM booking_details WHERE cust_contact = @custContact";

            // Execute the delete query
            db.ExecuteNonQuery(deleteBookingQuery, new MySqlParameter[]
            {
        new MySqlParameter("@custContact", MySqlDbType.Int64) { Value = customerContact }
            });

            MessageBox.Show("Booking canceled successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SearchByContact(long bookingCustContact)
        {
            try
            {
                // Define the query
                string query = "SELECT * FROM cancellation_details WHERE booking_cust_contact = @bookingCustContact";

                DataTable dataTable = db.ExecuteQuery(query, new MySqlParameter[]
                {
            new MySqlParameter("@bookingCustContact", MySqlDbType.Int64) { Value = bookingCustContact }
                });

                // Populate the data grid
                PopulateDataGridView(dataTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching record: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateDataGridView(DataTable dataTable)
        {
            try
            {
                // Clear existing rows in the DataGridView
                cancelationDataGrid.Rows.Clear();

                // Loop through each row in the DataTable and add it to the DataGridView
                foreach (DataRow row in dataTable.Rows)
                {
                    cancelationDataGrid.Rows.Add(
                        row["cancel_id"],
                        row["booking_cust_contact"],
                        row["cancel_date"],
                        row["total_amount"],
                        row["total_paid"],
                        row["refund"]
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error populating data grid: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Sorry, we cannot edit/update/delete this master due to validation rules. Please create a new entry with the updated data", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Sorry, we cannot edit/update/delete this master due to validation rules. Please create a new entry with the updated data", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Sorry, we cannot edit/update/delete this master due to validation rules. Please create a new entry with the updated data", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void tbBookingCustContact_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if the user pressed Enter or Tab
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                // Trigger the method to check for bookings and show the popup
                long custContact = Convert.ToInt64(tbBookingCustContact.Text);
                CheckExistingBookings(custContact);
            }
        }

        private void CheckExistingBookings(long custContact)
        {
            try
            {
                // Query to get the customer_id from customer_details based on customer_contact
                string customerQuery = "SELECT cust_id FROM customer_details WHERE cust_contact = @custContact";

                // Execute query to get customer_id
                DataTable customerData = db.ExecuteQuery(customerQuery, new MySqlParameter[]
                {
                    new MySqlParameter("@custContact", MySqlDbType.Int64) { Value = custContact }
                });

                // If no customer is found with the provided contact
                if (customerData.Rows.Count == 0)
                {
                    MessageBox.Show("No customer found with this contact number.", "No Customer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Get customer_id from query result
                int custId = Convert.ToInt32(customerData.Rows[0]["cust_id"]);

                // Now check for bookings associated with this customer_id
                string bookingQuery = "SELECT booking_id, booking_date, total_amount, total_paid, refund FROM booking_details WHERE cust_id = @custId";

                // Execute query to get bookings for this customer_id
                DataTable bookingData = db.ExecuteQuery(bookingQuery, new MySqlParameter[]
                {
                    new MySqlParameter("@custId", MySqlDbType.Int32) { Value = custId }
                });

                // If no bookings are found
                if (bookingData.Rows.Count == 0)
                {
                    MessageBox.Show("No bookings found for this customer.", "No Bookings", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Show the popup form to select a booking
                BookingSelectionPopup bookingForm = new BookingSelectionPopup(bookingData);
                DialogResult result = bookingForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // Get the selected booking details from the popup form
                    int selectedBookingId = bookingForm.SelectedBookingId;
                    double totalAmount = bookingForm.SelectedTotalAmount;
                    double totalPaid = bookingForm.SelectedTotalPaid;
                    double refund = bookingForm.SelectedRefund;

                    // Calculate refund after 25% deduction
                    double adjustedRefund = refund * 0.75;

                    // Calculate 18% GST on the total paid (down payment)
                    double gstDeducted = totalPaid * 0.18;

                    // Calculate the final cancellation refund amount after deduction
                    double finalRefundAmount = adjustedRefund - gstDeducted;

                    // Display the calculated refund amount in the cancellation fields
                    tbTotalAmount.Text = totalAmount.ToString();
                    tbTotalPaid.Text = totalPaid.ToString();
                    tbRefund.Text = finalRefundAmount.ToString(); // Display the final refund after deduction

                    // Proceed with cancellation save (same as before)
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking bookings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tabPage1_DoubleClick(object sender, EventArgs e)
        {

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tbBookingCustContact_TextChanged(object sender, EventArgs e)
        {
            if (tbBookingCustContact.Text.Length == 10) // Check for full 10-digit number
            {
                AutoFillBookingDetails(tbBookingCustContact.Text);
            }
        }

        private void AutoFillBookingDetails(string contactNumber)
        {
            try
            {
                string query = "SELECT grand_total, paid_amount FROM booking_details WHERE cust_contact = @contact";
                DataTable dt = db.ExecuteQuery(query, new MySqlParameter("@contact", contactNumber));

                if (dt.Rows.Count > 0) // Ensure there's at least one row in the result
                {
                    // Safe conversion of database values
                    double totalAmount = Convert.ToDouble(dt.Rows[0]["grand_total"] ?? 0);
                    double paidAmount = Convert.ToDouble(dt.Rows[0]["paid_amount"] ?? 0);

                    // Set values in textboxes
                    tbTotalAmount.Text = totalAmount.ToString("0.00");
                    tbTotalPaid.Text = paidAmount.ToString("0.00");

                    // Calculate Refund Amount: Total Amount - Paid Amount
                    double refundAmount = totalAmount - paidAmount;
                    tbRefund.Text = refundAmount.ToString("0.00");
                }
                else
                {
                    // Default values when no booking details are found
                    tbTotalAmount.Text = "0.00";
                    tbTotalPaid.Text = "0.00";
                    tbRefund.Text = "0.00";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching booking details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
