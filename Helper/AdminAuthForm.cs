using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using RealEstateManagemaentSystem2024.masterForm;

namespace RealEstateManagemaentSystem2024
{
    public partial class AdminAuthForm : Form
    {
        public AdminAuthForm()
        {
            InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            await webView21.EnsureCoreWebView2Async(null);

            // Listen for messages from JavaScript
            webView21.CoreWebView2.WebMessageReceived += WebView21_WebMessageReceived;

            // Load the HTML authentication page
            string htmlPath = @"file:///C:/Users/Raghvendra/OneDrive/Desktop/RealEstateManagemaentSystem2024/Html/AdminAuth.html"; // Update with correct path
            webView21.Source = new Uri(htmlPath);
        }

        private void WebView21_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string message = e.TryGetWebMessageAsString(); // Get message from JavaScript

            if (message == "openUserMaster")
            {
                this.Invoke((MethodInvoker)delegate
                {
                    // ✅ Close WebView2 Authentication form
                    this.Close();

                    // ✅ Open User Master form
                    UserMaster userMaster = new UserMaster();
                    userMaster.ShowDialog(); // Ensure form opens in the main UI thread
                });
            }
            else if (message == "closeAuthForm") // Handle cancel button
            {
                this.Invoke((MethodInvoker)delegate
                {
                    this.Close(); // Close only the authentication form
                });
            }
        }
    }
}
