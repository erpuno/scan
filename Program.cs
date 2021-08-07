using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace INFOTECH
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            N2O.Server.start("0.0.0.0", 1900);
            Application.Run(new FormScan());
            Application.Exit();
        }
    }
}
