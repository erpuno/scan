using System;
using System.Windows.Forms;

namespace INFOTECH
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            N2O.Server.start("0.0.0.0",1900);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormScan());
            Application.Exit();
        }
    }
}
