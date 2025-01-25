using RealEstateManagemaentSystem2024.mainForm;
using RealEstateManagemaentSystem2024.masterForm;
using RealStateManagementSystem.config;
using RealStateManagementSystem.mainForm;
using RealStateManagementSystem.masterForm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealEstateManagemaentSystem2024
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Customer());
        }
    }
}
