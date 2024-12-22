using RealEstateManagemaentSystem2024.mainForm;
using RealEstateManagemaentSystem2024.masterForm;
using System;
using System.Windows.Forms;

namespace RealStateManagementSystem.mainForm
{
    public partial class Login : Form
    {
        private Timer timer; // Timer for blinking
        private bool isVisible = true; // To toggle label visibility
        private System.Drawing.Color[] colors = { System.Drawing.Color.Red, System.Drawing.Color.Blue, System.Drawing.Color.Green }; // Colors to cycle through
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
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
            this.Hide();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            UserMaster userMaster = new UserMaster();
            userMaster.Show();
        }
    }
}
