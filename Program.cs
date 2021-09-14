using System;
using System.Windows.Forms;
using Microsoft.FSharp.Core;
using N2O;
using TWAIN32;

namespace INFOTECH
{   
    class WTwain : Scan.ITwain {
        public static TWAINI self = Program.global.twain;
        
    }

    static class Program
    {
        public static FormScan global;

        [STAThread]
        static void Main()
        {
            Scan.ITwain wrap = (Scan.ITwain)new WTwain();
            N2O.Server.proto = FSharpFunc<N2O.Types.Req,FSharpFunc<N2O.Types.Msg,N2O.Types.Msg>>.FromConverter(Acquire.proto(wrap));
            N2O.Server.start("0.0.0.0", 40220);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(global = new FormScan());
            Application.Exit();
        }
    }
}
