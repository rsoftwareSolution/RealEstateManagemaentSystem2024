using MySql.Data.MySqlClient;
using RealEstateManagemaentSystem2024.mainForm;
using RealEstateManagemaentSystem2024.MainForm;
using RealEstateManagemaentSystem2024.masterForm;
using RealEstateManagemaentSystem2024.Settings;
using System;
using System.Drawing;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RealStateManagementSystem.mainForm
{
    public partial class Login : Form
    {

        private Label lblDateTime;
        private Timer dateTimeTimer;


        public Login()
        {
            InitializeComponent();

            // Create a label to display date and time
            lblDateTime = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 11, FontStyle.Bold),
                ForeColor = Color.Black,
                BackColor = Color.Transparent,
                Location = new Point(590, 520) // Adjust position as needed
            };

            // Add Label to the form
            this.Controls.Add(lblDateTime);

            // Initialize Timer
            dateTimeTimer = new Timer
            {
                Interval = 1000 // Update every second
            };

            dateTimeTimer.Tick += DateTimeTimer_Tick;
            dateTimeTimer.Start();
        }

        // Event handler to update date & time
        private void DateTimeTimer_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm:ss tt");
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
            ShowProgressBarAndExit();
        }

        private void ShowProgressBarAndExit()
        {
            // Create a new form to display the progress bar
            Form progressForm = new Form
            {
                Width = 300,
                Height = 100,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.White,
                ControlBox = false // Remove close button
            };

            // Create a label for "Exiting..."
            Label label = new Label
            {
                Text = "Exiting, please wait...",
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Times New Roman", 11, FontStyle.Regular)
            };

            // Create a ProgressBar control
            ProgressBar progressBar = new ProgressBar
            {
                Dock = DockStyle.Bottom,
                Style = ProgressBarStyle.Marquee, // Indeterminate progress
                MarqueeAnimationSpeed = 50
            };

            // Add label and progress bar to the form
            progressForm.Controls.Add(label);
            progressForm.Controls.Add(progressBar);

            // Show the progress form
            progressForm.Show();

            // Use a Timer to delay exit
            Timer timer = new Timer
            {
                Interval = 500 // 3 seconds delay
            };

            timer.Tick += (s, args) =>
            {
                timer.Stop();
                progressForm.Close(); // Close the progress bar
                Application.Exit(); // Exit the application
            };

            timer.Start();
        }

        private void label4_MouseEnter(object sender, EventArgs e)
        {
            label4.ForeColor = Color.DarkRed;
        }

        private void label4_MouseLeave(object sender, EventArgs e)
        {
            label4.ForeColor = Color.Black;
        }

        private void label2_Click(object sender, EventArgs e)
        {
            AboutUs aboutUs = new AboutUs();
            aboutUs.Show();
        }
    }
}
