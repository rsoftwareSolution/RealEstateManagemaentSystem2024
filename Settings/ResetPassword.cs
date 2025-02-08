using MySql.Data.MySqlClient;
using RealEstateManagemaentSystem2024.Helper;
using System;
using System.Data;
using System.Windows.Forms;

namespace RealEstateManagemaentSystem2024.Settings
{
    public partial class ResetPassword : Form
    {
        Database db = new Database();

        public ResetPassword()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            string email = tbEmail.Text.Trim();
            string contact = tbContact.Text.Trim();
            DateTime birthDate = dtpBirthDate.Value;
            string newPassword = tbNewPassword.Text.Trim();
            string confirmPassword = tbConfirmPassword.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(contact))
            {
                MessageBox.Show("Please fill all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "SELECT COUNT(*) FROM user WHERE user_email = @Email AND user_contact = @Contact AND user_dob = @BirthDate";
            object result = db.ExecuteScalar(query, new MySqlParameter[]
            {
                new MySqlParameter("@Email", MySqlDbType.VarChar) { Value = email },
                new MySqlParameter("@Contact", MySqlDbType.VarChar) { Value = contact },
                new MySqlParameter("@BirthDate", MySqlDbType.Date) { Value = birthDate }
            });

            if (Convert.ToInt32(result) == 0)
            {
                MessageBox.Show("Invalid details. Please check your email, contact, or birthdate.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Please enter a new password and confirm password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!newPassword.Equals(confirmPassword))
            {
                MessageBox.Show("New Password and Confirm Password do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string updateQuery = "UPDATE user SET user_password = @NewPassword WHERE user_email = @Email";
            db.ExecuteNonQuery(updateQuery, new MySqlParameter[]
            {
                new MySqlParameter("@NewPassword", MySqlDbType.VarChar) { Value = newPassword },
                new MySqlParameter("@Email", MySqlDbType.VarChar) { Value = email }
            });

            MessageBox.Show("Password reset successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearFields();
            this.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            tbEmail.Clear();
            tbContact.Clear();
            dtpBirthDate.Value = DateTime.Now;
            tbNewPassword.Clear();
            tbConfirmPassword.Clear();
        }

        private void ResetPassword_Load(object sender, EventArgs e)
        {

        }
    }
}
