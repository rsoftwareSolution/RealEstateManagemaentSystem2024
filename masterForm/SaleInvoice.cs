using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace RealEstateManagemaentSystem2024.Registers
{
    public partial class SaleInvoice : Form
    {
        Database db = new Database();

        public SaleInvoice()

        {
            InitializeComponent();

        }

        


        private void LoadSaleInvoices()
        {

        }
        private DataTable GetBookingData()
        {
            string query = "SELECT flat_type_name, COUNT(*) as count FROM quatation_details GROUP BY flat_type_name";
            DataTable result = db.ExecuteQuery(query); // Assuming db.ExecuteQuery() returns DataTable

            // Check if data is returned
            if (result.Rows.Count == 0)
            {
                MessageBox.Show("No data found for chart.");
            }

            return result;
        }




        private void LoadCustomerContacts()
        {
            try
            {
                string query = "SELECT cust_contact FROM customer_details";
                DataTable dt = db.ExecuteQuery(query);

                AutoCompleteStringCollection contactCollection = new AutoCompleteStringCollection();
                foreach (DataRow row in dt.Rows)
                {
                    contactCollection.Add(row["cust_contact"].ToString());
                }

                tbCustContact.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                tbCustContact.AutoCompleteSource = AutoCompleteSource.CustomSource;
                tbCustContact.AutoCompleteCustomSource = contactCollection;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customer contacts: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AutoFillCustomerName(string contactNumber)
        {
            try
            {
                string query = "SELECT cust_name FROM customer_details WHERE cust_contact = @contact";
                DataTable dt = db.ExecuteQuery(query, new MySqlParameter("@contact", contactNumber));

                if (dt.Rows.Count > 0)
                {
                    tbCustName.Text = dt.Rows[0]["cust_name"].ToString();
                }
                else
                {
                    tbCustName.Text = string.Empty; // Clear if no match found
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching customer name: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


     

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

       

        private void btn_Delete_Click(object sender, EventArgs e)
        {

            string query = "DELETE FROM saleinvoice_details WHERE sale_id=@sale_id";

            db.ExecuteNonQuery(query, new MySqlParameter("@sale_id", Convert.ToInt32(tbSaleId.Text.Trim())));

            MessageBox.Show("Sale Invoice deleted successfully");
            LoadSaleInvoices();
        }

        private void SaleInvoice_Load(object sender, EventArgs e)
        {
            LoadCustomerContacts();
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            string saleDate = dtpSaleDate.Value.ToString("yyyy-MM-dd");

            string bookingNumber = tbBookingNumber.Text.Trim();
            string alreadyBooked = cbAlreadyBooked.Text.Trim();
            string custContact = tbCustContact.Text.Trim();
            string custName = tbCustName.Text.Trim();
            string projectName = cbProjectName.Text.Trim();
            string flatType = cbFlatType.Text.Trim();
            string flatNumber = tbFlatNo.Text.Trim();
            string vehicleName = cbVehicle.Text.Trim();
            double parkingCharges = Convert.ToDouble(Text.Trim());
            double igst = Convert.ToDouble(tbIGST.Text.Trim());
            double cgst = Convert.ToDouble(tbCGST.Text.Trim());
            double sgst = Convert.ToDouble(tbSGST.Text.Trim());
            double subTotal = Convert.ToDouble(tbSubTotal.Text.Trim());
            double totalAmount = Convert.ToDouble(tbTotalAmount.Text.Trim());
            double bookingAmount = Convert.ToDouble(tbBookingAmount.Text.Trim());
            double paidAmount = Convert.ToDouble(tbBookingAmount.Text.Trim());
            double roundOff = Convert.ToDouble(tbRoundOff.Text.Trim());
            double grandTotal = Convert.ToDouble(tbGrandTotal.Text.Trim());

            string query = @"INSERT INTO saleinvoice_details (sale_date, booking_number, already_booked, cust_contact, cust_name, 
                            project_name, flat_type, flat_number, vehicle_name, parking_charges, igst, cgst, sgst, sub_total, 
                            total_amount, booking_amount, round_off, grand_total) 
                            VALUES (@sale_date, @booking_number, @already_booked, @cust_contact, @cust_name, @project_name, 
                            @flat_type, @flat_number, @vehicle_name, @parking_charges, @igst, @cgst, @sgst, @sub_total, @total_amount, 
                            @booking_amount, @round_off, @grand_total)";

            db.ExecuteNonQuery(query,
                new MySqlParameter("@sale_date", dtpSaleDate.Value.ToString("yyyy-MM-dd")),
                new MySqlParameter("@booking_number", tbBookingNumber.Text.Trim()),
                new MySqlParameter("@already_booked", cbAlreadyBooked.Text.Trim()),
                new MySqlParameter("@cust_contact", tbCustContact.Text.Trim()),
                new MySqlParameter("@cust_name", tbCustName.Text.Trim()),
                new MySqlParameter("@project_name", cbProjectName.Text.Trim()),
                new MySqlParameter("@flat_type", cbFlatType.Text.Trim()),
                new MySqlParameter("@flat_number", tbFlatNo.Text.Trim()),
                new MySqlParameter("@vehicle_name", cbVehicle.Text.Trim()),
                new MySqlParameter("@parking_charges", Convert.ToDouble(tbParkingCharges.Text.Trim())),
                new MySqlParameter("@igst", Convert.ToDouble(tbIGST.Text.Trim())),
                new MySqlParameter("@cgst", Convert.ToDouble(tbCGST.Text.Trim())),
                new MySqlParameter("@sgst", Convert.ToDouble(tbSGST.Text.Trim())),
                new MySqlParameter("@sub_total", Convert.ToDouble(tbSubTotal.Text.Trim())),
                new MySqlParameter("@total_amount", Convert.ToDouble(tbTotalAmount.Text.Trim())),
                new MySqlParameter("@booking_amount", Convert.ToDouble(tbBookingAmount.Text.Trim())),
                new MySqlParameter("@round_off", Convert.ToDouble(tbRoundOff.Text.Trim())),
                new MySqlParameter("@grand_total", Convert.ToDouble(tbGrandTotal.Text.Trim())));

            MessageBox.Show("Sale Invoice saved successfully");
            LoadSaleInvoices();
        }

        private void btn_Update_Click(object sender, EventArgs e)
        {

            string query = @"UPDATE saleinvoice_details SET sale_date=@sale_date, booking_number=@booking_number, already_booked=@already_booked, 
                            cust_contact=@cust_contact, cust_name=@cust_name, project_name=@project_name, flat_type=@flat_type, flat_number=@flat_number, 
                            vehicle_name=@vehicle_name, parking_charges=@parking_charges, igst=@igst, cgst=@cgst, sgst=@sgst, sub_total=@sub_total, 
                            total_amount=@total_amount, booking_amount=@booking_amount, round_off=@round_off, grand_total=@grand_total 
                            WHERE sale_id=@sale_id";

            db.ExecuteNonQuery(query,
                new MySqlParameter("@sale_id", Convert.ToInt32(tbSaleId.Text.Trim())),
                new MySqlParameter("@sale_date", dtpSaleDate.Value.ToString("yyyy-MM-dd")),
                new MySqlParameter("@booking_number", tbBookingNumber.Text.Trim()),
                new MySqlParameter("@already_booked", cbAlreadyBooked.Text.Trim()),
                new MySqlParameter("@cust_contact", tbCustContact.Text.Trim()),
                new MySqlParameter("@cust_name", tbCustName.Text.Trim()),
                new MySqlParameter("@project_name", cbProjectName.Text.Trim()),
                new MySqlParameter("@flat_type", cbFlatType.Text.Trim()),
                new MySqlParameter("@flat_number", tbFlatNo.Text.Trim()),
                new MySqlParameter("@vehicle_name", cbVehicle.Text.Trim()),
                new MySqlParameter("@parking_charges", Convert.ToDouble(tbParkingCharges.Text.Trim())),
                new MySqlParameter("@igst", Convert.ToDouble(tbIGST.Text.Trim())),
                new MySqlParameter("@cgst", Convert.ToDouble(tbCGST.Text.Trim())),
                new MySqlParameter("@sgst", Convert.ToDouble(tbSGST.Text.Trim())),
                new MySqlParameter("@sub_total", Convert.ToDouble(tbSubTotal.Text.Trim())),
                new MySqlParameter("@total_amount", Convert.ToDouble(tbTotalAmount.Text.Trim())),
                new MySqlParameter("@booking_amount", Convert.ToDouble(tbBookingAmount.Text.Trim())),
                new MySqlParameter("@round_off", Convert.ToDouble(tbRoundOff.Text.Trim())),
                new MySqlParameter("@grand_total", Convert.ToDouble(tbGrandTotal.Text.Trim())));

            MessageBox.Show("Sale Invoice updated successfully");
            LoadSaleInvoices();
        }

        private void tbCustContact_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbCustContact.Text))
            {
                FetchBookingDetails(tbCustContact.Text);
            }

            if (tbCustContact.Text.Length == 10) // Check for full 10-digit number
            {
                AutoFillCustomerName(tbCustContact.Text);
            }
        }

        private void FetchBookingDetails(string contactNumber)
        {
            try
            {
                string query = "SELECT * FROM booking_details WHERE cust_contact = @contact";
                DataTable dt = db.ExecuteQuery(query, new MySqlParameter("@contact", contactNumber));

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    cbAlreadyBooked.Text = "Yes";
                    tbCustName.Text = row["cust_name"].ToString();
                    cbProjectName.Text = row["building_or_project_name"].ToString();
                    cbFlatType.Text = row["flat_type"].ToString();
                    cbVehicle.Text = row["vehicle_name"].ToString();
                    tbParkingCharges.Text = row["parking_charges"].ToString();
                    tbIGST.Text = row["igst"].ToString();
                    tbCGST.Text = row["cgst"].ToString();
                    tbSGST.Text = row["sgst"].ToString();
                    tbSubTotal.Text = row["sub_total"].ToString();
                    tbTotalAmount.Text = row["total_amount"].ToString();
                    tbBookingAmount.Text = row["paid_amount"].ToString();
                    tbRoundOff.Text = row["round_off"].ToString();
                    tbGrandTotal.Text = row["grand_total"].ToString();
                }
                else
                {
                    cbAlreadyBooked.Text = "No";
                    tbCustName.Text = string.Empty;
                    cbProjectName.Text = string.Empty;
                    cbFlatType.Text = string.Empty;
                    cbVehicle.Text = string.Empty;
                    tbParkingCharges.Text = "0";
                    tbIGST.Text = "0";
                    tbCGST.Text = "0";
                    tbSGST.Text = "0";
                    tbSubTotal.Text = "0";
                    tbTotalAmount.Text = "0";
                    tbBookingAmount.Text = "0";
                    tbRoundOff.Text = "0";
                    tbGrandTotal.Text = "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching booking details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

