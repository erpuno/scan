using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;
using N2O;
using TWAIN32;

namespace INFOTECH
{   
    class WTwain : Scan.ITwain {
        public static TWAINI self = Program.global.twain;

        public int Start() {return 0;}
        public FSharpList<string> GetDataSources() { 
            Func<string,string> conv = s => CSV.Parse(s)[11].ToString();

            return ListModule.OfSeq(self.GetDataSources().ConvertAll(new Converter<string,string>(conv)));
        }
        public int OpenManager() { return (int) self.OpenManager(); }
        public string DefaultIdentity() { return self.GetDefault(); }
        public string OpenScanner(string id) { return self.OpenScanner(id); }
        public bool Exit{
            get { return self.Exit; }
            set { self.Exit = value; }
        }
        public Scan.Callback ScanCallback {
            get { return new Scan.Callback((b) => (int)self.Twain.m_scancallback(b)); }
            set { self.Twain.m_scancallback = new TWAIN.ScanCallback((b) => (TWAIN.STS)value(b)); }
        }
        public int State { get { return (int)self.Twain.GetState();} }
        public void NativeCallback(bool close) { Program.global.ScanCallbackNative(close); }
        public bool NativeTransfer(){ self.NativeTransfer(); return self.Exit; }
        public bool AutoFeed(){ self.AutoFeed(); return self.Exit; }
        public bool AutoScan(){ self.AutoScan(); return self.Exit; }
        public bool ProgressDriverUi(bool p) { self.ProgressDriverUI(p); return self.Exit; }
        public bool SetCaps() {

            return self.Exit;
        }
        public int EnableDS() {
            string szTwmemref = "FALSE,FALSE,0"; //hide windows and reset handle
            TWAIN.TW_USERINTERFACE twuserinterface = default(TWAIN.TW_USERINTERFACE);            
            self.Twain.CsvToUserinterface(ref twuserinterface, szTwmemref);
            return (int)self.Twain.DatUserinterface(TWAIN.DG.CONTROL, TWAIN.MSG.ENABLEDS, ref twuserinterface);
        }
        public bool EnableDuplex(){ self.EnableDuplex(); return self.Exit;}
        public string FileInfo() {
            string root = Directory.GetCurrentDirectory();
            var files = new DirectoryInfo(root).EnumerateFiles("doc-*.pdf", SearchOption.AllDirectories);
            return "not detected file name";
        }
        public bool Init(int after) {
            self.UseBitmap = 0;
            self.ScanStart = true;
            self.XferReadySent = false;
            self.DisableDsSent = false;
            self.AfterScan = (TWAIN.STATE) after;
            return self.Exit;
        }
        public void Dispose() {self.Dispose();}
    }

    static class Program
    {
        public static FormScan global;

        public static FSharpFunc<N2O.Types.Msg,N2O.Types.Msg> router(N2O.Types.Req r) {
            return Acquire.proto((Scan.ITwain)new WTwain());
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
