using System;
using System.Windows.Forms;
using Microsoft.FSharp.Core;

namespace INFOTECH
{
    static class Program
    {
        public static FormScan global;
        public static N2O.Types.Msg router(N2O.Types.Msg m) {
            // we have access to global here
            return N2O.Types.Msg.NewText("HELLO");
        }

        public static FSharpFunc<N2O.Types.Msg,N2O.Types.Msg> entry(N2O.Types.Req r) {
            return FSharpFunc<N2O.Types.Msg, N2O.Types.Msg>.FromConverter(router);
        }

        [STAThread]
        static void Main()
        {
            N2O.Server.proto = FSharpFunc<N2O.Types.Req,FSharpFunc<N2O.Types.Msg,N2O.Types.Msg>>.FromConverter(entry);
            N2O.Server.start("0.0.0.0", 40220);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(global = new FormScan());
            Application.Exit();
        }
    }
}
