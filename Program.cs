using System;
using System.Windows.Forms;
using Microsoft.FSharp.Core;

namespace INFOTECH
{
    static class Program
    {
        public static FormScan global;
        public static N2O.Types.Msg agentAPI(N2O.Types.Msg m) {
            Console.WriteLine("Access to Scanner here: {0}", global.twain.Twain);
            return N2O.Types.Msg.NewText("HELLO");
        }

        public static FSharpFunc<N2O.Types.Msg,N2O.Types.Msg> router(N2O.Types.Req r) {
            return FSharpFunc<N2O.Types.Msg, N2O.Types.Msg>.FromConverter(agentAPI);
        }

        [STAThread]
        static void Main()
        {
            N2O.Server.proto = FSharpFunc<N2O.Types.Req,FSharpFunc<N2O.Types.Msg,N2O.Types.Msg>>.FromConverter(router);
            N2O.Server.start("0.0.0.0", 40220);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(global = new FormScan());
            Application.Exit();
        }
    }
}
