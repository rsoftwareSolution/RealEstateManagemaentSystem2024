using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealEstateManagemaentSystem2024.MainForm
{
    public partial class AboutUs : Form
    {
        public AboutUs()
        {
            InitializeComponent();
        }

        private void webView21_Click(object sender, EventArgs e)
        {

        }

        private void AboutUs_Load(object sender, EventArgs e)
        {
            webView21.Source = new Uri(@"file:///C:/Users/Raghvendra/OneDrive/Desktop/RealEstateManagemaentSystem2024/Html/AboutUs.html");
        }
    }
}
