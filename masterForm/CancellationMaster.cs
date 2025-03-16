using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace RealStateManagementSystem.masterForm
{
    public partial class CancellationMaster : Form
    {

        Database db = new Database();
        // Initialize the AutoIdGenerator for generating the cancel_id with prefix "CANC" and numeric length of 4
        AutoIdGenerator cancelIdGenerator = new AutoIdGenerator("CANC", 4);

        public CancellationMaster()
        {
            InitializeComponent();
        }

        private void CancellationMaster_Load(object sender, EventArgs e)
        {
            ClearForm();
            cancelationDataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 11, FontStyle.Regular);

        }

        private void ClearForm()
        {
            tbBookingCustContact.Clear();
            dtpCancelDate.Value = DateTime.Now; // Reset to current date
            tbTotalAmount.Clear();
            tbTotalPaid.Clear();
            tbRefund.Clear();

            GenerateCancelId();
            tbBookingCustContact.Select();
        }
        private void GenerateCancelId()
        {
            try
            {
                // Fetch the latest cancel_id from the database
                string query = @"SELECT cancel_id FROM cancellation_details 
                         WHERE cancel_id LIKE 'CANCEL%' 
                         ORDER BY CONVERT(SUBSTRING(cancel_id, 7, CHAR_LENGTH(cancel_id) - 6), UNSIGNED) 
                         DESC LIMIT 1";

                object maxIdObj = db.ExecuteScalar(query);

                int lastNumericId = 0;

                if (maxIdObj != null && maxIdObj != DBNull.Value)
                {
                    string lastId = maxIdObj.ToString();  // Example: "CANCEL0021"

                    if (lastId.StartsWith("CANCEL") && int.TryParse(lastId.Substring(6), out int numericPart))
                    {
                        lastNumericId = numericPart; // Get the last numeric ID
                    }
                }
                // Reset counter to the last numeric ID fetched from DB
                cancelIdGenerator.ResetCounter(lastNumericId);

                // Generate the next cancel ID
                tbCancelId.Text = cancelIdGenerator.GenerateNextId();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating Cancel ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbCancelId.Text = string.Empty;  // Reset textbox on error
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                db.BeginTransaction(); // ✅ Start Transaction

                long customerContact = Convert.ToInt64(tbBookingCustContact.Text);

                // ✅ 1. Check if customer exists in booking_details
                string checkBookingQuery = "SELECT COUNT(*) FROM booking_details WHERE cust_contact = @custContact";
                object bookingExists = db.ExecuteScalar(checkBookingQuery, new MySqlParameter[]
                {
            new MySqlParameter("@custContact", MySqlDbType.Int64) { Value = customerContact }
                });

                if (bookingExists == null || Convert.ToInt32(bookingExists) == 0)
                {
                    MessageBox.Show("No booking found for this customer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    db.RollbackTransaction();
                    return;
                }

                // ✅ 2. Check if cancellation already exists
                string checkCancellationQuery = "SELECT COUNT(*) FROM cancellation_details WHERE booking_cust_contact = @custContact";
                object cancellationExists = db.ExecuteScalar(checkCancellationQuery, new MySqlParameter[]
                {
            new MySqlParameter("@custContact", MySqlDbType.Int64) { Value = customerContact }
                });

                if (cancellationExists != null && Convert.ToInt32(cancellationExists) > 0)
                {
                    MessageBox.Show("This customer has already been canceled. Multiple cancellations are not allowed.",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    db.RollbackTransaction();
                    return;
                }

                // ✅ 3. Get Refund & Total Amount
                double refundAmount = Convert.ToDouble(tbRefund.Text);
                double totalAmount = Convert.ToDouble(tbTotalAmount.Text);
                double totalPaid = Convert.ToDouble(tbTotalPaid.Text);

                // ✅ 4. Generate Next Cancel ID
                string cancelId = cancelIdGenerator.GenerateNextId();

                // ✅ 5. Insert into cancellation_details
                string insertCancellationQuery = "INSERT INTO cancellation_details (cancel_id, booking_cust_contact, cancel_date, total_amount, total_paid, refund) " +
                                                 "VALUES (@cancelId, @custContact, @cancelDate, @totalAmount, @totalPaid, @refundAmount)";

                db.ExecuteNonQuery(insertCancellationQuery, new MySqlParameter[]
                {
            new MySqlParameter("@cancelId", MySqlDbType.VarChar) { Value = cancelId },
            new MySqlParameter("@custContact", MySqlDbType.Int64) { Value = customerContact },
            new MySqlParameter("@cancelDate", MySqlDbType.VarChar) { Value = DateTime.Now.ToString("yyyy-MM-dd") },
            new MySqlParameter("@totalAmount", MySqlDbType.Double) { Value = totalAmount },
            new MySqlParameter("@totalPaid", MySqlDbType.Double) { Value = totalPaid },
            new MySqlParameter("@refundAmount", MySqlDbType.Double) { Value = refundAmount }
                });

                // ✅ 6. Delete booking_details entry
                string deleteBookingQuery = "DELETE FROM booking_details WHERE cust_contact = @custContact";
                db.ExecuteNonQuery(deleteBookingQuery, new MySqlParameter[]
                {
            new MySqlParameter("@custContact", MySqlDbType.Int64) { Value = customerContact }
                });

                // ✅ 7. Commit Transaction if everything is successful
                db.CommitTransaction();
                MessageBox.Show("Cancellation processed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // ✅ 8. Clear Form
                ClearForm();
            }
            catch (Exception ex)
            {
                // ✅ If any error occurs, rollback transaction
                db.RollbackTransaction();
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

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (long.TryParse(textBox5.Text, out long contact))
            {
                SearchByContact(contact);
            }
        }
    }
}
