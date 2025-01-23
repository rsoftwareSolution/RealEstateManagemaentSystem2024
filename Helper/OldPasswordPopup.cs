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
    public partial class OldPasswordPopup : Form
    {

        public string OldPassword { get; private set; } // To store the entered old password

        public OldPasswordPopup()
        {
            InitializeComponent();
        }

        private void OldPasswordPopup_Load(object sender, EventArgs e)
        {
            tbOldPassword.Select();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            // Capture the entered password and close the dialog
            OldPassword = tbOldPassword.Text; // tbOldPassword is the name of the password TextBox
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            // Close the dialog without capturing the password
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
