using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;
using TWAIN32;

namespace INFOTECH {

    public class TWAINI : IDisposable {

        public bool ScanStart = true;
        public TWAIN.MSG PendingXfers;
        public TWAIN.TWSX XferMech;
        public int ImageXferCount = 0;
        public TWAIN.STATE AfterScan;
        public bool Exit = false;
        public TWAIN Twain = null;
        public IntPtr intptrHwnd;
        public int UseBitmap = 0;
        public int ImageCount = 0;
        public bool DisableDsSent = false;
        public bool XferReadySent = false;
        public IntPtr ptrXfer = IntPtr.Zero;
        public IntPtr ptrImage = IntPtr.Zero;
        public int ImageBytes = 0;
        public string ProductDirectory = "";
        public bool Indicators = false;

        public TWAINI() { }
        public void Init(IntPtr Handle) {
               this.intptrHwnd = Handle;
               this.Twain = new TWAIN("INFOTECH SE", "ERP.UNO", "MIA-AGENT",
                    (ushort)TWAIN.TWON_PROTOCOL.MAJOR, (ushort)TWAIN.TWON_PROTOCOL.MINOR,
                    ((uint)TWAIN.DG.APP2 | (uint)TWAIN.DG.CONTROL | (uint)TWAIN.DG.IMAGE),
                    TWAIN.TWCY.UKRAINE, "MIA-AGENT", TWAIN.TWLG.UKRAINIAN, 2, 5,
                    true, false, deviceEventCallback, scanCallback, runInUIThreadDelegate, Handle);
        }

        public TWAIN.DeviceEventCallback deviceEventCallback;
        public TWAIN.ScanCallback scanCallback;
        public TWAIN.RunInUiThreadDelegate runInUIThreadDelegate;

        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected void Dispose(bool disposing) { if (disposing && (Twain != null)) { Twain.Dispose(); Twain = null; } }

        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }

        public void Rollback(TWAIN.STATE a_state)
        {
            Console.WriteLine("Rollback to state: {0}", a_state);

            TWAIN.TW_PENDINGXFERS twpendingxfers = default(TWAIN.TW_PENDINGXFERS);
            TWAIN.TW_USERINTERFACE twuserinterface = default(TWAIN.TW_USERINTERFACE);
            TWAIN.TW_IDENTITY twidentity = default(TWAIN.TW_IDENTITY);

            if (Twain == null) { return; }
            if ((Twain.GetState() == TWAIN.STATE.S7) && (a_state < TWAIN.STATE.S7)) { Twain.DatPendingxfers(TWAIN.DG.CONTROL, TWAIN.MSG.ENDXFER, ref twpendingxfers); }
            if ((Twain.GetState() == TWAIN.STATE.S6) && (a_state < TWAIN.STATE.S6)) { Twain.DatPendingxfers(TWAIN.DG.CONTROL, TWAIN.MSG.RESET, ref twpendingxfers); }
            if ((Twain.GetState() == TWAIN.STATE.S5) && (a_state < TWAIN.STATE.S5)) { Twain.DatUserinterface(TWAIN.DG.CONTROL, TWAIN.MSG.DISABLEDS, ref twuserinterface); }
            if ((Twain.GetState() == TWAIN.STATE.S4) && (a_state < TWAIN.STATE.S4)) { TWAIN.CsvToIdentity(ref twidentity, Twain.GetDsIdentity()); Twain.DatIdentity(TWAIN.DG.CONTROL, TWAIN.MSG.CLOSEDS, ref twidentity); }
            if ((Twain.GetState() == TWAIN.STATE.S3) && (a_state < TWAIN.STATE.S3)) { Twain.DatParent(TWAIN.DG.CONTROL, TWAIN.MSG.CLOSEDSM, ref intptrHwnd); }
        }

    }

}