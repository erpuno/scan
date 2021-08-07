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

    }

}