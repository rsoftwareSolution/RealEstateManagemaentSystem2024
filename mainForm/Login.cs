using MySql.Data.MySqlClient;
using RealEstateManagemaentSystem2024.mainForm;
using RealEstateManagemaentSystem2024.masterForm;
using RealEstateManagemaentSystem2024.Settings;
using System;
using System.Windows.Forms;

namespace RealStateManagementSystem.mainForm
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            textBox1.Text = "Username";
            textBox2.Text = "Password";
            textBox2.PasswordChar = '\0'; // Ensure password char is off for placeholder

            textBox1.Enter += TextBox_Enter;
            textBox1.Leave += TextBox_Leave;
            textBox2.Enter += TextBox_Enter;
            textBox2.Leave += TextBox_Leave;
            textBox2.TextChanged += textBox2_TextChanged;
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (textBox.Text == "Username" || textBox.Text == "Password")
                {
                    textBox.Text = "";
                    if (textBox == textBox2) textBox.PasswordChar = '*'; // Mask password
                }
            }
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    if (textBox == textBox1)
                    {
                        textBox.Text = "Username";
                    }
                    else if (textBox == textBox2)
                    {
                        textBox.Text = "Password";
                        textBox.PasswordChar = '\0'; // Remove masking for placeholder
                    }
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "Password")
            {
                textBox2.PasswordChar = '*'; // Ensure masking while typing
            }
        }

        private void clr_txt()
        {
            textBox1.Text = "Username";
            textBox2.Text = "Password";
            textBox2.PasswordChar = '\0';
            textBox1.Select();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string useremail = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();

            if (useremail == "Username" || password == "Password" || string.IsNullOrEmpty(useremail) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "SELECT COUNT(*) FROM user WHERE user_email = @useremail AND user_password = @password";

            try
            {
                string connectionString = "Server=localhost;Port=3306;Database=real_state_db;User Id=root;Password=root;";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@useremail", useremail);
                        command.Parameters.AddWithValue("@password", password);
                        int count = Convert.ToInt32(command.ExecuteScalar());

                        if (count > 0)
                        {
                            logInAndFetchData(connection);
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password.", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                            clr_txt();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void logInAndFetchData(MySqlConnection connection)
        {
            string query = "SELECT user_email, user_contact FROM user WHERE user_email = @Email AND user_password = @Password";

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", textBox1.Text);
                    command.Parameters.AddWithValue("@Password", textBox2.Text);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string userEmail = reader["user_email"]?.ToString() ?? "Unknown Email";
                            string contact = reader["user_contact"]?.ToString() ?? "Unknown Contact";

                            Dashboard dashboard = new Dashboard(userEmail, contact);
                            dashboard.Show();
                            clr_txt();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Invalid email or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            UserMaster userMaster = new UserMaster();
            userMaster.Show();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            ResetPassword resetPassword = new ResetPassword();
            resetPassword.Show();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
