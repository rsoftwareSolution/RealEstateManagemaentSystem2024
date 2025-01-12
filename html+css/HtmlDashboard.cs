using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealEstateManagemaentSystem2024.html_css
{
    public partial class HtmlDashboard : Form
    {
        public HtmlDashboard()
        {
            InitializeComponent();
            // Define the path to the HTML file
            string htmlPath = @"file:///E:/appu bcs/RealEstateManagemaentSystem2024/html+css/Dashboard.html";

            // Load the HTML file into the WebView2 control
            webView21.Source = new Uri(htmlPath);
        }

        private void webView21_Click(object sender, EventArgs e)
        {

        }
    }
}
