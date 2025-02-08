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
        private Timer timer; // Timer for blinking
        private bool isVisible = true; // To toggle label visibility
        private System.Drawing.Color[] colors = { System.Drawing.Color.Black, System.Drawing.Color.White }; // Colors to cycle through
        private int colorIndex = 0; // Index to track current color

        public Login()
        {
            InitializeComponent();
            // Initialize the Timer
            timer = new Timer();
            timer.Interval = 700; // Set interval in milliseconds (e.g., 500ms = 0.5 seconds)
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Toggle visibility
            isVisible = !isVisible;
            label5.Visible = isVisible;

            // Change color
            label5.ForeColor = colors[colorIndex];
            colorIndex = (colorIndex + 1) % colors.Length; // Cycle through the colors
        }

        private void Login_Load(object sender, EventArgs e)
        {
            textBox1.Select();
            clr_txt();
        }

        private void clr_txt()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox1.Select();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Get user inputs from TextBoxes
            string useremail = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();

            // Check if username or password is empty
            if (string.IsNullOrEmpty(useremail) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Define the SELECT query to verify login credentials
            string query = "SELECT COUNT(*) FROM user WHERE user_email = @useremail AND user_password = @password";

            try
            {
                string connectionString = "Server=localhost;Port=3306;Database=real_state_db;User Id=root;Password=root;";

                // Open a connection to the database
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // Pass parameters to prevent SQL injection
                        command.Parameters.AddWithValue("@useremail", useremail);
                        command.Parameters.AddWithValue("@password", password);

                        // Execute the query and get the result
                        int count = Convert.ToInt32(command.ExecuteScalar());

                        if (count > 0)
                        {
                           logInAndFetchData(connection);
                        }
                        else
                        {
                            // Login failed
                            MessageBox.Show("Invalid username or password.", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                            clr_txt();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Show error message in case of exception
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
                            // Retrieve user details
                            string userEmail = reader["user_email"]?.ToString() ?? "Unknown Email";
                            string contact = reader["user_contact"]?.ToString() ?? "Unknown Contact";

                            // Show Dashboard and pass user details
                            Dashboard dashboard = new Dashboard(userEmail, contact);
                            dashboard.Show();
                            clr_txt();
                            this.Hide(); // Hide the login form
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

        private void button2_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            ResetPassword resetPassword = new ResetPassword();
            resetPassword.Show();
        }
    }
}
