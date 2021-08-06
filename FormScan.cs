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

namespace INFOTECH
{
    public partial class FormScan : Form, IMessageFilter
    {
        private bool m_blScanStart = true;
        private TWAIN.MSG m_twainmsgPendingXfers;
        private TWAIN.TWSX m_twsxXferMech;
        private int m_iImageXferCount = 0;
        private TWAIN.STATE m_stateAfterScan;

        public delegate TWAIN.MSG ReportImageDelegate
        (
            string a_szTag,
            string a_szDg,
            string a_szDat,
            string a_szMsg,
            TWAIN.STS a_sts,
            Bitmap a_bitmap,
            string a_szFile,
            string a_szTwimageinfo,
            byte[] a_abImage,
            int a_iImageOffset
        );

        private ReportImageDelegate ReportImage = null;

        private int counter = 77;
        private System.Windows.Forms.NotifyIcon notifyIcon;

        private void SystemTrayIconDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void ContextMenuExit(object sender, EventArgs e)
        {
            this.SystemTrayIcon.Visible = false;
            Application.Exit();
            Environment.Exit(0);
        }

        public void ShowNotification(string Source, string Message, string URL)
        {
            notifyIcon.Icon = Properties.Resources.Default;
            notifyIcon.Text = "МІА: Сповіщення";
            notifyIcon.BalloonTipTitle = Source;
            notifyIcon.BalloonTipText = Message;
            notifyIcon.BalloonTipClicked += delegate { System.Diagnostics.Process.Start(URL); };
            notifyIcon.ShowBalloonTip(100);
            notifyIcon.Visible = true;
        }

        private void Version(object sender, EventArgs e)
        {
            updateCounter();
            ShowNotification("Джерело: МІА:Документообіг",
                             "Повідомлення: Новий документ ЗА-23545",
                             "https://crm.erp.uno");

            MessageBox.Show("Версія: 2.5.1.0\n\nРозробник: ДП «ІНФОТЕХ»", "МІА: Сканування");
        }

        public void OpenUrl(string URL) { System.Diagnostics.Process.Start("https://crm.erp.uno"); }

        private void WindowResize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void WindowClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public FormScan()
        {
            // Build our form...
            InitializeComponent();

            this.Icon = Properties.Resources.Default;
            this.SystemTrayIcon.Icon = Properties.Resources.Default;
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);

            // Change the Text property to the name of your application
            this.SystemTrayIcon.Text = "МІА: Сканування Документів";
            this.SystemTrayIcon.Visible = true;

            // Modify the right-click menu of your system tray icon here
            ContextMenu menu = new ContextMenu();
            menu.MenuItems.Add("Про «МІА: Сканування»", Version);
            menu.MenuItems.Add("Налаштування профілів користувача", m_buttonSetup_Click);
            menu.MenuItems.Add("-");
            menu.MenuItems.Add("Завершити програму", ContextMenuExit);

            this.SystemTrayIcon.ContextMenu = menu;
            this.notifyIcon.Icon = Properties.Resources.Default;
            this.notifyIcon.ContextMenu = menu;
            this.notifyIcon.Visible = true;

            this.Resize += WindowResize;
            this.Text = "MIA: Сканування";
            this.FormClosing += WindowClosing;


            // Open the log in our working folder, and say hi...
            TWAIN32.Log.Open("INFOTECH", ".", 1);
            TWAIN32.Log.Info("INFOTECH v" + System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString());

            // Init other stuff...
            m_blIndicators = false;
            m_blExit = false;
            m_iUseBitmap = 0;

            try
            {
                // Init stuff...
                TWAIN.DeviceEventCallback deviceeventcallback = DeviceEventCallback;
                TWAIN.ScanCallback scancallback = ScanCallbackTrigger;
                TWAIN.RunInUiThreadDelegate runinuithreaddelegate = RunInUiThread;

                // Instantiate TWAIN, and register ourselves...
                m_twain = new TWAIN
                (
                    "INFOTECH SE",
                    "ERP.UNO Open Source",
                    "MIA-SCAN",
                    (ushort)TWAIN.TWON_PROTOCOL.MAJOR,
                    (ushort)TWAIN.TWON_PROTOCOL.MINOR,
                    ((uint)TWAIN.DG.APP2 | (uint)TWAIN.DG.CONTROL | (uint)TWAIN.DG.IMAGE),
                    TWAIN.TWCY.UKRAINE,
                    "MIA-SCAN",
                    TWAIN.TWLG.UKRAINIAN,
                    2,
                    5,
                    true,
                    false,
                    deviceeventcallback,
                    scancallback,
                    runinuithreaddelegate,
                    this.Handle
                );

                Console.WriteLine("{0}", m_twain);
            }
            catch (Exception exception)
            {
                TWAIN32.Log.Error("exception - " + exception.Message);
                m_twain = null;
                m_blExit = true;
                MessageBox.Show
                (
                    "Unable to start, the most likely reason is that the TWAIN\n" +
                    "Data Source Manager is not installed on your system.\n\n" +
                    "An internet search for 'TWAIN DSM' will locate it and once\n" +
                    "installed, you should be able to proceed.\n\n" +
                    "You can also try the following link:\n" +
                    "https://github.com/erpuno/scan",
                    "Error Starting MIA: Scanning"
                );
                return;
            }

            // Init our picture box...
            InitImage();

            // Prep for TWAIN events...
            SetMessageFilter(true);

            // Init our buttons...
            SetButtons(EBUTTONSTATE.CLOSED);
        }

        public void updateCounter()
        {
            Brush brush = new SolidBrush(Color.White);
            Font drawFont = new Font("Arial", 8);
            Bitmap bitmap = new Bitmap(16, 16);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.DrawString((counter++).ToString(), drawFont, brush, 0, 1);
            Icon icon = Icon.FromHandle(bitmap.GetHicon());
            this.SystemTrayIcon.Icon = icon;
        }

        /// <summary>
        /// Our scan callback event, used to drive the engine when scanning...
        /// </summary>
        public delegate void ScanCallbackEvent();

        /// <summary>
        /// Our event handler for the scan callback event.  This will be
        /// called once by ScanCallbackTrigger on receipt of an event
        /// like MSG_XFERREADY, and then will be reissued on every call
        /// into ScanCallback until we're done and get back to state 4.
        ///
        /// This helps to make sure we're always running in the context
        /// of FormMain on Windows, which is critical if we want drivers
        /// to work properly.  It also gives a way to break up the calls
        /// so the message pump is still reponsive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScanCallbackEventHandler(object sender, EventArgs e)
        {
            ScanCallbackNative((m_twain == null) ? true : (m_twain.GetState() <= TWAIN.STATE.S3));
        }

        /// <summary>
        /// Rollback the TWAIN state to whatever is requested...
        /// </summary>
        /// <param name="a_state"></param>
        public void Rollback(TWAIN.STATE a_state)
        {
            Console.WriteLine("Rollback to state: {0}", a_state);

            TWAIN.TW_PENDINGXFERS twpendingxfers = default(TWAIN.TW_PENDINGXFERS);
            TWAIN.TW_USERINTERFACE twuserinterface = default(TWAIN.TW_USERINTERFACE);
            TWAIN.TW_IDENTITY twidentity = default(TWAIN.TW_IDENTITY);

            // Make sure we have something to work with...
            if (m_twain == null)
            {
                return;
            }

            // Walk the states, we don't care about the status returns.  Basically,
            // these need to work, or we're guaranteed to hang...

            // 7 --> 6
            if ((m_twain.GetState() == TWAIN.STATE.S7) && (a_state < TWAIN.STATE.S7))
            {
                m_twain.DatPendingxfers(TWAIN.DG.CONTROL, TWAIN.MSG.ENDXFER, ref twpendingxfers);
            }

            // 6 --> 5
            if ((m_twain.GetState() == TWAIN.STATE.S6) && (a_state < TWAIN.STATE.S6))
            {
                m_twain.DatPendingxfers(TWAIN.DG.CONTROL, TWAIN.MSG.RESET, ref twpendingxfers);
            }

            // 5 --> 4
            if ((m_twain.GetState() == TWAIN.STATE.S5) && (a_state < TWAIN.STATE.S5))
            {
                m_twain.DatUserinterface(TWAIN.DG.CONTROL, TWAIN.MSG.DISABLEDS, ref twuserinterface);
            }

            // 4 --> 3
            if ((m_twain.GetState() == TWAIN.STATE.S4) && (a_state < TWAIN.STATE.S4))
            {
                TWAIN.CsvToIdentity(ref twidentity, m_twain.GetDsIdentity());
                m_twain.DatIdentity(TWAIN.DG.CONTROL, TWAIN.MSG.CLOSEDS, ref twidentity);
            }

            // 3 --> 2
            if ((m_twain.GetState() == TWAIN.STATE.S3) && (a_state < TWAIN.STATE.S3))
            {
                m_twain.DatParent(TWAIN.DG.CONTROL, TWAIN.MSG.CLOSEDSM, ref m_intptrHwnd);
            }
        }

        /// <summary>
        /// Monitor for DG_CONTROL / DAT_NULL / MSG_* stuff (ex MSG_XFERREADY), this
        /// function is only triggered when SetMessageFilter() is called with 'true'...
        /// </summary>
        /// <param name="a_message">Message to process</param>
        /// <returns>Result of the processing</returns>
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public bool PreFilterMessage(ref Message a_message)
        {
            if (m_twain != null)
            {
                return (m_twain.PreFilterMessage(a_message.HWnd, a_message.Msg, a_message.WParam, a_message.LParam));
            }
            return (true);
        }

        /// <summary>
        /// Turn message filtering on or off, we use this to capture stuff
        /// like MSG_XFERREADY.  If it's off, then it's assumed we're getting
        /// this info through DAT_CALLBACK2...
        /// </summary>
        /// <param name="a_blAdd">True to turn it on</param>
        public void SetMessageFilter(bool a_blAdd)
        {
            if (a_blAdd)
            {
                Application.AddMessageFilter(this);
            }
            else
            {
                Application.RemoveMessageFilter(this);
            }
        }

        /// <summary>
        /// Restore a snapshot of driver values...
        /// </summary>
        /// <param name="a_szFile">File to use to restore driver settings</param>
        /// <returns>SUCCESS if the restore succeeded</returns>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust", Unrestricted = false)]
        public TWAIN.STS RestoreSnapshot(string a_szFile)
        {
            TWAIN.STS sts;
            byte[] abSettings;
            UInt32 u32Length;
            IntPtr intptrHandle;
            string szCustomdsdata;
            string szStatus;
            CSV csv = new CSV();
            TWAIN.TW_CAPABILITY twcapability;
            TWAIN.TW_CUSTOMDSDATA twcustomdsdata;

            // Reset the driver, we don't care if it succeeds or fails...
            szStatus = "";
            twcapability = default(TWAIN.TW_CAPABILITY);
            m_twain.CsvToCapability(ref twcapability, ref szStatus, "0,0,0");
            m_twain.DatCapability(TWAIN.DG.CONTROL, TWAIN.MSG.RESETALL, ref twcapability);

            // Get the snapshot from a file...
            FileStream filestream = null;
            try
            {
                filestream = new FileStream(a_szFile, FileMode.Open);
                u32Length = (UInt32)filestream.Length;
                abSettings = new byte[u32Length];
                filestream.Read(abSettings, 0, abSettings.Length);
            }
            finally
            {
                if (filestream != null)
                {
                    filestream.Dispose();
                }
            }

            // Put it in an intptr...
            intptrHandle = Marshal.AllocHGlobal((int)u32Length);
            Marshal.Copy(abSettings, 0, intptrHandle, (int)u32Length);

            // Set the snapshot, if possible...
            csv.Add(u32Length.ToString());
            csv.Add(intptrHandle.ToString());
            szCustomdsdata = csv.Get();
            twcustomdsdata = default(TWAIN.TW_CUSTOMDSDATA);
            m_twain.CsvToCustomdsdata(ref twcustomdsdata, szCustomdsdata);
            sts = m_twain.DatCustomdsdata(TWAIN.DG.CONTROL, TWAIN.MSG.SET, ref twcustomdsdata);

            // Cleanup...
            Marshal.FreeHGlobal(intptrHandle);

            // All done...
            return (sts);
        }

        /// <summary>
        /// Save a snapshot of the driver values...
        /// </summary>
        /// <param name="a_szFile">File to receive driver settings</param>
        /// <returns>SUCCESS if the restore succeeded</returns>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust", Unrestricted = false)]
        public TWAIN.STS SaveSnapshot(string a_szFile)
        {
            TWAIN.STS sts;
            TWAIN.TW_CUSTOMDSDATA twcustomdsdata;

            // Test...
            if ((a_szFile == null) || (a_szFile == ""))
            {
                return (TWAIN.STS.SUCCESS);
            }

            // Get a snapshot, if possible...
            twcustomdsdata = default(TWAIN.TW_CUSTOMDSDATA);
            sts = m_twain.DatCustomdsdata(TWAIN.DG.CONTROL, TWAIN.MSG.GET, ref twcustomdsdata);
            if (sts != TWAIN.STS.SUCCESS)
            {
                TWAIN32.Log.Error("DAT_CUSTOMDSDATA failed...");
                return (sts);
            }

            // Save the data to a file...
            FileStream filestream = null;
            try
            {
                IntPtr intptrInfo;
                filestream = new FileStream(a_szFile, FileMode.Create);
                byte[] abSettings = new byte[twcustomdsdata.InfoLength];
                intptrInfo = m_twain.DsmMemLock(twcustomdsdata.hData);
                Marshal.Copy(intptrInfo, abSettings, 0, (int)twcustomdsdata.InfoLength);
                m_twain.DsmMemUnlock(twcustomdsdata.hData);
                filestream.Write(abSettings, 0, abSettings.Length);
            }
            finally
            {
                if (filestream != null)
                {
                    filestream.Dispose();
                }
            }

            // Free the memory...
            m_twain.DsmMemFree(ref twcustomdsdata.hData);

            // All done...
            return (TWAIN.STS.SUCCESS);
        }
        private TWAIN.STS DeviceEventCallback()
        {
            TWAIN.STS sts;
            TWAIN.TW_DEVICEEVENT twdeviceevent;

            // Drain the event queue...
            while (true)
            {
                // Try to get an event...
                twdeviceevent = default(TWAIN.TW_DEVICEEVENT);
                sts = m_twain.DatDeviceevent(TWAIN.DG.CONTROL, TWAIN.MSG.GET, ref twdeviceevent);
                Console.WriteLine("Device Event: {0}", sts);
                if (sts != TWAIN.STS.SUCCESS)
                {
                    break;
                }
            }

            return (TWAIN.STS.SUCCESS);
        }

        private TWAIN.STS ScanCallbackTrigger(bool a_blClosing)
        {
            BeginInvoke(new MethodInvoker(delegate { ScanCallbackEventHandler(this, new EventArgs()); }));
            return (TWAIN.STS.SUCCESS);
        }

        private TWAIN.STS ScanCallbackNative(bool a_blClosing)
        {
            Console.WriteLine("Scan Callback: {0}", a_blClosing);

            bool blXferDone;
            TWAIN.STS sts;
            string szFilename = "";
            MemoryStream memorystream;
            TWAIN.TW_USERINTERFACE twuserinterface = default(TWAIN.TW_USERINTERFACE);
            TWAIN.MSG twainmsg;
            TWAIN.TW_IMAGEINFO twimageinfo = default(TWAIN.TW_IMAGEINFO);

            // Validate...
            if (m_twain == null)
            {
                Log.Error("m_twain is null...");
                return (TWAIN.STS.FAILURE);
            }

            // We're leaving...
            if (a_blClosing) { return (TWAIN.STS.SUCCESS); }

            // Somebody pushed the Cancel or the OK button...
            if (m_twain.IsMsgCloseDsReq())
            {
                m_twain.Rollback(TWAIN.STATE.S4);
                return (TWAIN.STS.SUCCESS);
            }
            else if (m_twain.IsMsgCloseDsOk())
            {
                m_twain.Rollback(TWAIN.STATE.S4);
                return (TWAIN.STS.SUCCESS);
            }

            // Init ourselves...
            if (m_blScanStart)
            {
                TWAIN.TW_CAPABILITY twcapability;

                // Don't come in here again until the start of the next scan batch...
                m_blScanStart = false;

                // Clear this...
                m_twainmsgPendingXfers = TWAIN.MSG.ENDXFER;

                // Get the current setting for the image transfer...
                twcapability = default(TWAIN.TW_CAPABILITY);
                twcapability.Cap = TWAIN.CAP.ICAP_XFERMECH;
                m_twsxXferMech = TWAIN.TWSX.NATIVE;
            }

            blXferDone = false;

            if (m_twsxXferMech == TWAIN.TWSX.NATIVE)
            {
                Bitmap bitmap = null;
                sts = m_twain.DatImagenativexfer(TWAIN.DG.IMAGE, TWAIN.MSG.GET, ref bitmap);
                Console.WriteLine("ImageNativeXfer(): {0}", sts);
                if (bitmap != null)
                   Console.WriteLine("NATIVE GET: {0} {1} {2}", bitmap.Size, m_iImageCount++, m_formsetup.GetImageFolder());

                string aszFilename = Path.Combine(m_formsetup.GetImageFolder(), "img" + string.Format("{0:D6}", m_iImageCount));
                bitmap.Save(aszFilename + ".tif", ImageFormat.Tiff);

                if (sts != TWAIN.STS.XFERDONE)
                {
                    Console.WriteLine("Scanning error: " + sts + Environment.NewLine);
                    m_twain.Rollback(m_stateAfterScan);
                    return (TWAIN.STS.SUCCESS);
                }
                else
                {
                    twainmsg = TWAIN.MSG.ENDXFER; //ReportImage("ScanCallback: 006", TWAIN.DG.IMAGE.ToString(), TWAIN.DAT.IMAGENATIVEXFER.ToString(), TWAIN.MSG.GET.ToString(), sts, bitmap, null, null, null, 0);
                    if (twainmsg == TWAIN.MSG.STOPFEEDER)
                    {
                        m_twainmsgPendingXfers = TWAIN.MSG.STOPFEEDER;
                    }
                    else if (twainmsg == TWAIN.MSG.RESET)
                    {
                        m_twainmsgPendingXfers = TWAIN.MSG.RESET;
                    }
                    bitmap = null;
                    blXferDone = true;
                }
            }

            else
            {
                Console.WriteLine("Scan: unrecognized ICAP_XFERMECH value..." + m_twsxXferMech + Environment.NewLine);
                m_twain.Rollback(m_stateAfterScan);
                return (TWAIN.STS.SUCCESS);
            }


            //
            // End of the image transfer section...
            //
            ///////////////////////////////////////////////////////////////////////////////


            // Let's get some meta data.  TWAIN only guarantees that this data
            // is accurate in state 7 after TWRC_XFERDONE has been received...
            if (blXferDone)
            {
                if (twimageinfo.BitsPerPixel == 0)
                {
                    twimageinfo = default(TWAIN.TW_IMAGEINFO);
                    sts = m_twain.DatImageinfo(TWAIN.DG.IMAGE, TWAIN.MSG.GET, ref twimageinfo);
                    if (sts != TWAIN.STS.SUCCESS)
                    {
                        Console.WriteLine("ImageInfo failed: " + sts + Environment.NewLine);
                        m_twain.Rollback(m_stateAfterScan);
                        ReportImage("ScanCallback: 052", TWAIN.DG.IMAGE.ToString(), TWAIN.DAT.IMAGEINFO.ToString(), TWAIN.MSG.GET.ToString(), sts, null, null, null, null, 0);
                        return (TWAIN.STS.SUCCESS);
                    }
                }
                Console.WriteLine("ImageInfo: " + TWAIN.ImageinfoToCsv(twimageinfo));
            }

            TWAIN.TW_PENDINGXFERS twpendingxfersEndXfer = default(TWAIN.TW_PENDINGXFERS);
            sts = m_twain.DatPendingxfers(TWAIN.DG.CONTROL, TWAIN.MSG.ENDXFER, ref twpendingxfersEndXfer);
            Console.WriteLine("END XFER: {0} {1}", twpendingxfersEndXfer.Count, twpendingxfersEndXfer.EOJ);
            if (sts != TWAIN.STS.SUCCESS)
            {
                Console.WriteLine("Scanning error: " + sts + Environment.NewLine);
                m_twain.Rollback(m_stateAfterScan);
                return (TWAIN.STS.SUCCESS);
            }

            // Handle DAT_NULL/MSG_CLOSEDSREQ...
            if (m_twain.IsMsgCloseDsReq() && !m_blDisableDsSent)
            {
                m_blDisableDsSent = true;
                Rollback(TWAIN.STATE.S4);
                SetButtons(EBUTTONSTATE.OPEN);
            }

            // Handle DAT_NULL/MSG_CLOSEDSOK...
            if (m_twain.IsMsgCloseDsOk() && !m_blDisableDsSent)
            {
                m_blDisableDsSent = true;
                Rollback(TWAIN.STATE.S4);
                SetButtons(EBUTTONSTATE.OPEN);
            }

            Application.DoEvents();
            BeginInvoke(new MethodInvoker(delegate { ScanCallbackEventHandler(this, new EventArgs()); }));

            if (m_twain.GetState() == TWAIN.STATE.S6)
            {
                switch (m_twainmsgPendingXfers)
                {
                    // No work needed here...
                    default:
                    case TWAIN.MSG.ENDXFER:
                        Console.WriteLine("Pending End");
                        break;

                    // Reset, we're exiting from scanning...
                    case TWAIN.MSG.RESET:
                        Console.WriteLine("Pending Reset");
                        m_twainmsgPendingXfers = TWAIN.MSG.ENDXFER;
//                        m_twain.Rollback(m_stateAfterScan);
//                        ReportImage("ScanCallback: 054", TWAIN.DG.CONTROL.ToString(), TWAIN.DAT.PENDINGXFERS.ToString(), TWAIN.MSG.RESET.ToString(), sts, null, null, null, null, 0);
                        return (TWAIN.STS.SUCCESS);

                    // Stop the feeder...
                    case TWAIN.MSG.STOPFEEDER:
                        m_twainmsgPendingXfers = TWAIN.MSG.ENDXFER;
                        TWAIN.TW_PENDINGXFERS twpendingxfersStopFeeder = default(TWAIN.TW_PENDINGXFERS);
                        sts = m_twain.DatPendingxfers(TWAIN.DG.CONTROL, TWAIN.MSG.STOPFEEDER, ref twpendingxfersStopFeeder);
                        Console.WriteLine("Pending Stop Feeder: {0}", sts);
                       
                        if (sts != TWAIN.STS.SUCCESS)
                        {
                            // If we can't stop gracefully, then just abort...
                            m_twain.Rollback(m_stateAfterScan);
//                            ReportImage("ScanCallback: 055", TWAIN.DG.CONTROL.ToString(), TWAIN.DAT.PENDINGXFERS.ToString(), TWAIN.MSG.RESET.ToString(), sts, null, null, null, null, 0);
                            return (TWAIN.STS.SUCCESS);
                        }
                        break;
                }
            }

            // If count goes to zero, then the session is complete, and the
            // driver goes to state 5, otherwise it goes to state 6 in
            // preperation for the next image.  We'll also return a value of
            // zero if the transfer hits an error, like a paper jam.  And then,
            // just to keep it interesting, we also need to pay attention to
            // whether or not we have a UI running.  If we don't, then state 5
            // is our target, otherwise we want to go to state 4 (programmatic
            // mode)...
            if (twpendingxfersEndXfer.Count == 0)
            {
                Console.WriteLine("Scanning done: " + TWAIN.STS.SUCCESS);
                Rollback(TWAIN.STATE.S4);

                m_blDisableDsSent = true;
                m_twain.DatUserinterface(TWAIN.DG.CONTROL, TWAIN.MSG.DISABLEDS, ref twuserinterface);
                SetButtons(EBUTTONSTATE.OPEN);
                m_blScanStart = true;
            }

            // All done...
            return (TWAIN.STS.SUCCESS);

        }

        private TWAIN.STS ScanCallbackMemory(bool a_blClosing)
        {
            TWAIN.STS sts;

            // Scoot...
            if (m_twain == null)
            {
                return (TWAIN.STS.FAILURE);
            }

            // We're superfluous...
            if (m_twain.GetState() <= TWAIN.STATE.S4)
            {
                return (TWAIN.STS.SUCCESS);
            }

            // We're leaving...
            if (a_blClosing)
            {
                return (TWAIN.STS.SUCCESS);
            }

            // Do this in the right thread, we'll usually be in the
            // right spot, save maybe on the first call...
            if (this.InvokeRequired)
            {
                return
                (
                    (TWAIN.STS)Invoke
                    (
                        (Func<TWAIN.STS>)delegate
                        {
                            return (ScanCallbackMemory(a_blClosing));
                        }
                    )
                );
            }

            // Handle DAT_NULL/MSG_XFERREADY...
            if (m_twain.IsMsgXferReady() && !m_blXferReadySent)
            {
                m_blXferReadySent = true;

                // Get the amount of memory needed...
                m_twsetupmemxfer = default(TWAIN.TW_SETUPMEMXFER);
                sts = m_twain.DatSetupmemxfer(TWAIN.DG.CONTROL, TWAIN.MSG.GET, ref m_twsetupmemxfer);
                if ((sts != TWAIN.STS.SUCCESS) || (m_twsetupmemxfer.Preferred == 0))
                {
                    m_blXferReadySent = false;
                    if (!m_blDisableDsSent)
                    {
                        m_blDisableDsSent = true;
                        Rollback(TWAIN.STATE.S4);
                    }
                }

                // Allocate the transfer memory (with a little extra to protect ourselves)...
                m_intptrXfer = Marshal.AllocHGlobal((int)m_twsetupmemxfer.Preferred + 65536);
                if (m_intptrXfer == IntPtr.Zero)
                {
                    m_blDisableDsSent = true;
                    Rollback(TWAIN.STATE.S4);
                }
            }

            // Handle DAT_NULL/MSG_CLOSEDSREQ...
            if (m_twain.IsMsgCloseDsReq() && !m_blDisableDsSent)
            {
                m_blDisableDsSent = true;
                Rollback(TWAIN.STATE.S4);
                SetButtons(EBUTTONSTATE.OPEN);
            }

            // Handle DAT_NULL/MSG_CLOSEDSOK...
            if (m_twain.IsMsgCloseDsOk() && !m_blDisableDsSent)
            {
                m_blDisableDsSent = true;
                Rollback(TWAIN.STATE.S4);
                SetButtons(EBUTTONSTATE.OPEN);
            }

            // This is where the state machine transfers and optionally
            // saves the images to disk (it also displays them).  It'll go back
            // and forth between states 6 and 7 until an error occurs, or until
            // we run out of images...
            if (m_blXferReadySent && !m_blDisableDsSent)
            {
                CaptureImages();
            }

            // Trigger the next event, this is where things all chain together.
            // We need begininvoke to prevent blockking, so that we don't get
            // backed up into a messy kind of recursion.  We need DoEvents,
            // because if things really start moving fast it's really hard for
            // application events, like button clicks to break through...
            Application.DoEvents();
            BeginInvoke(new MethodInvoker(delegate { ScanCallbackEventHandler(this, new EventArgs()); }));

            // All done...
            return (TWAIN.STS.SUCCESS);
        }

        /// <summary>
        /// Go through the sequence needed to capture images...
        /// </summary>
        private void CaptureImages()
        {
                   TWAIN.STS sts;
            TWAIN.TW_IMAGEINFO twimageinfo = default(TWAIN.TW_IMAGEINFO);
            TWAIN.TW_IMAGEMEMXFER twimagememxfer = default(TWAIN.TW_IMAGEMEMXFER);
            TWAIN.TW_PENDINGXFERS twpendingxfers = default(TWAIN.TW_PENDINGXFERS);
            TWAIN.TW_USERINTERFACE twuserinterface = default(TWAIN.TW_USERINTERFACE);

            // Dispatch on the state...
            switch (m_twain.GetState())
            {
                // Not a good state, just scoot...
                default:
                    return;

                // We're on our way out...
                case TWAIN.STATE.S5:
                    m_blDisableDsSent = true;
                    m_twain.DatUserinterface(TWAIN.DG.CONTROL, TWAIN.MSG.DISABLEDS, ref twuserinterface);
                    SetButtons(EBUTTONSTATE.OPEN);
                    return;

                // Memory transfers...
                case TWAIN.STATE.S6:
                case TWAIN.STATE.S7:
                    TWAIN.CsvToImagememxfer(ref twimagememxfer, "0,0,0,0,0,0,0," + ((int)TWAIN.TWMF.APPOWNS | (int)TWAIN.TWMF.POINTER) + "," + m_twsetupmemxfer.Preferred + "," + m_intptrXfer);
                    sts = m_twain.DatImagememxfer(TWAIN.DG.IMAGE, TWAIN.MSG.GET, ref twimagememxfer);
                    break;
            }

            // Handle problems...
            if ((sts != TWAIN.STS.SUCCESS) && (sts != TWAIN.STS.XFERDONE))
            {
                m_blDisableDsSent = true;
                Rollback(TWAIN.STATE.S4);
                SetButtons(EBUTTONSTATE.OPEN);
                return;
            }

            // Allocate or grow the image memory...
            if (m_intptrImage == IntPtr.Zero)
            {
                m_intptrImage = Marshal.AllocHGlobal((int)twimagememxfer.BytesWritten);
            }
            else
            {
                m_intptrImage = Marshal.ReAllocHGlobal(m_intptrImage, (IntPtr)(m_iImageBytes + twimagememxfer.BytesWritten));
            }

            // Ruh-roh...
            if (m_intptrImage == IntPtr.Zero)
            {
                m_blDisableDsSent = true;
                Rollback(TWAIN.STATE.S4);
                SetButtons(EBUTTONSTATE.OPEN);
                return;
            }

            // Copy into the buffer, and bump up our byte tally...
            TWAIN.MemCpy(m_intptrImage + m_iImageBytes, m_intptrXfer, (int)twimagememxfer.BytesWritten);
            m_iImageBytes += (int)twimagememxfer.BytesWritten;

            // If we saw XFERDONE we can save the image, display it,
            // end the transfer, and see if we have more images...
            if (sts == TWAIN.STS.XFERDONE)
            {
                // Bump up our image counter, this always grows for the
                // life of the entire session...
                m_iImageCount += 1;

                // Get the image info...
                sts = m_twain.DatImageinfo(TWAIN.DG.IMAGE, TWAIN.MSG.GET, ref twimageinfo);
                Console.WriteLine("STS ImageInfo: {0}", sts);
                Console.WriteLine("    PixelSize: {0}", twimageinfo.PixelType);
                Console.WriteLine("    Compression: {0}", twimageinfo.Compression);

                // Add the appropriate header...

                // Bitonal uncompressed...
                if (((TWAIN.TWPT)twimageinfo.PixelType == TWAIN.TWPT.BW) && ((TWAIN.TWCP)twimageinfo.Compression == TWAIN.TWCP.NONE))
                {
                    TWAIN.TiffBitonalUncompressed tiffbitonaluncompressed;
                    tiffbitonaluncompressed = new TWAIN.TiffBitonalUncompressed((uint)twimageinfo.ImageWidth, (uint)twimageinfo.ImageLength, (uint)twimageinfo.XResolution.Whole, (uint)m_iImageBytes);
                    m_intptrImage = Marshal.ReAllocHGlobal(m_intptrImage, (IntPtr)(Marshal.SizeOf(tiffbitonaluncompressed) + m_iImageBytes));
                    TWAIN.MemMove((IntPtr)((UInt64)m_intptrImage + (UInt64)Marshal.SizeOf(tiffbitonaluncompressed)), m_intptrImage, m_iImageBytes);
                    Marshal.StructureToPtr(tiffbitonaluncompressed, m_intptrImage, true);
                    m_iImageBytes += (int)Marshal.SizeOf(tiffbitonaluncompressed);
                }

                // Bitonal GROUP4...
                else if (((TWAIN.TWPT)twimageinfo.PixelType == TWAIN.TWPT.BW) && ((TWAIN.TWCP)twimageinfo.Compression == TWAIN.TWCP.GROUP4))
                {
                    TWAIN.TiffBitonalG4 tiffbitonalg4;
                    tiffbitonalg4 = new TWAIN.TiffBitonalG4((uint)twimageinfo.ImageWidth, (uint)twimageinfo.ImageLength, (uint)twimageinfo.XResolution.Whole, (uint)m_iImageBytes);
                    m_intptrImage = Marshal.ReAllocHGlobal(m_intptrImage, (IntPtr)(Marshal.SizeOf(tiffbitonalg4) + m_iImageBytes));
                    TWAIN.MemMove((IntPtr)((UInt64)m_intptrImage + (UInt64)Marshal.SizeOf(tiffbitonalg4)), m_intptrImage, m_iImageBytes);
                    Marshal.StructureToPtr(tiffbitonalg4, m_intptrImage, true);
                    m_iImageBytes += (int)Marshal.SizeOf(tiffbitonalg4);
                }

                // Gray uncompressed...
                else if (((TWAIN.TWPT)twimageinfo.PixelType == TWAIN.TWPT.GRAY) && ((TWAIN.TWCP)twimageinfo.Compression == TWAIN.TWCP.NONE))
                {
                    TWAIN.TiffGrayscaleUncompressed tiffgrayscaleuncompressed;
                    tiffgrayscaleuncompressed = new TWAIN.TiffGrayscaleUncompressed((uint)twimageinfo.ImageWidth, (uint)twimageinfo.ImageLength, (uint)twimageinfo.XResolution.Whole, (uint)m_iImageBytes);
                    m_intptrImage = Marshal.ReAllocHGlobal(m_intptrImage, (IntPtr)(Marshal.SizeOf(tiffgrayscaleuncompressed) + m_iImageBytes));
                    TWAIN.MemMove((IntPtr)((UInt64)m_intptrImage + (UInt64)Marshal.SizeOf(tiffgrayscaleuncompressed)), m_intptrImage, m_iImageBytes);
                    Marshal.StructureToPtr(tiffgrayscaleuncompressed, m_intptrImage, true);
                    m_iImageBytes += (int)Marshal.SizeOf(tiffgrayscaleuncompressed);
                }

                // Gray JPEG...
                else if (((TWAIN.TWPT)twimageinfo.PixelType == TWAIN.TWPT.GRAY) && ((TWAIN.TWCP)twimageinfo.Compression == TWAIN.TWCP.JPEG))
                {
                    // No work to be done, we'll output JPEG...
                }

                // RGB uncompressed...
                else if (((TWAIN.TWPT)twimageinfo.PixelType == TWAIN.TWPT.RGB) && ((TWAIN.TWCP)twimageinfo.Compression == TWAIN.TWCP.NONE))
                {
                    TWAIN.TiffColorUncompressed tiffcoloruncompressed;
                    tiffcoloruncompressed = new TWAIN.TiffColorUncompressed((uint)twimageinfo.ImageWidth, (uint)twimageinfo.ImageLength, (uint)twimageinfo.XResolution.Whole, (uint)m_iImageBytes);
                    m_intptrImage = Marshal.ReAllocHGlobal(m_intptrImage, (IntPtr)(Marshal.SizeOf(tiffcoloruncompressed) + m_iImageBytes));
                    TWAIN.MemMove((IntPtr)((UInt64)m_intptrImage + (UInt64)Marshal.SizeOf(tiffcoloruncompressed)), m_intptrImage, m_iImageBytes);
                    Marshal.StructureToPtr(tiffcoloruncompressed, m_intptrImage, true);
                    m_iImageBytes += (int)Marshal.SizeOf(tiffcoloruncompressed);
                }

                // RGB JPEG...
                else if (((TWAIN.TWPT)twimageinfo.PixelType == TWAIN.TWPT.RGB) && ((TWAIN.TWCP)twimageinfo.Compression == TWAIN.TWCP.JPEG))
                {
                    // No work to be done, we'll output JPEG...
                }

                // Oh well...
                else
                {
                    TWAIN32.Log.Error("unsupported format <" + twimageinfo.PixelType + "," + twimageinfo.Compression + ">");
                    m_blDisableDsSent = true;
                    Rollback(TWAIN.STATE.S4);
                    SetButtons(EBUTTONSTATE.OPEN);
                    return;
                }

                // Save the image to disk, if we're doing that...
                if (!string.IsNullOrEmpty(m_formsetup.GetImageFolder()))
                {
                    // Create the directory, if needed...
                    if (!Directory.Exists(m_formsetup.GetImageFolder()))
                    {
                        try
                        {
                            Directory.CreateDirectory(m_formsetup.GetImageFolder());
                        }
                        catch (Exception exception)
                        {
                            TWAIN32.Log.Error("CreateDirectory failed - " + exception.Message);
                        }
                    }

                    // Write it out...
                    string szFilename = Path.Combine(m_formsetup.GetImageFolder(), "img" + string.Format("{0:D6}", m_iImageCount));
                    TWAIN.WriteImageFile(szFilename, m_intptrImage, m_iImageBytes, out szFilename);
                }

                // Turn the image into a byte array, and free the original memory...
                byte[] abImage = new byte[m_iImageBytes];
                Marshal.Copy(m_intptrImage, abImage, 0, m_iImageBytes);
                Marshal.FreeHGlobal(m_intptrImage);
                m_intptrImage = IntPtr.Zero;
                m_iImageBytes = 0;

                // Turn the byte array into a stream...
                MemoryStream memorystream = new MemoryStream(abImage);
                Bitmap bitmap = (Bitmap)Image.FromStream(memorystream);

                // Display the image...
                if (m_iImageCount % 2 == 0) //m_iUseBitmap == 0)
                {
                    m_iUseBitmap = 1;
                    LoadImage(ref m_pictureboxImage1, ref m_graphics1, ref m_bitmapGraphic1, bitmap);
                }
                else
                {
                    m_iUseBitmap = 0;
                    LoadImage(ref m_pictureboxImage2, ref m_graphics2, ref m_bitmapGraphic2, bitmap);
                }

                // Cleanup...
                bitmap.Dispose();
                memorystream = null; // disposed by the bitmap
                abImage = null;

                // End the transfer...
                m_twain.DatPendingxfers(TWAIN.DG.CONTROL, TWAIN.MSG.ENDXFER, ref twpendingxfers);

                Console.WriteLine("XFers.Count: {0}", (short)twpendingxfers.Count);
                if ((short)twpendingxfers.Count == 0)
                {

                    // Looks like we're done!
                    m_blDisableDsSent = true;
                    m_twain.DatUserinterface(TWAIN.DG.CONTROL, TWAIN.MSG.DISABLEDS, ref twuserinterface);
                    SetButtons(EBUTTONSTATE.OPEN);
                    return;
                }
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {

                if (m_twain != null)
                {
                    m_twain.Dispose();
                    m_twain = null;
                }
                if (m_bitmapGraphic1 != null)
                {
                    m_bitmapGraphic1.Dispose();
                    m_bitmapGraphic1 = null;
                }
                if (m_bitmapGraphic2 != null)
                {
                    m_bitmapGraphic2.Dispose();
                    m_bitmapGraphic2 = null;
                }
                if (m_brushBackground != null)
                {
                    m_brushBackground.Dispose();
                    m_brushBackground = null;
                }

               components.Dispose();

            }

          base.Dispose(disposing);
        }

        public bool ExitRequested()
        {
            return (m_blExit);
        }

        private void m_buttonSetup_Click(object sender, EventArgs e)
        {
            m_formsetup.StartPosition = FormStartPosition.CenterParent;
            m_formsetup.ShowDialog(this);
        }

        private void m_buttonScan_Click(object sender, EventArgs e)
        {
            m_iUseBitmap = 0;
            string szTwmemref;
            TWAIN.STS sts;

            // Silently start scanning if we detect that customdsdata is supported,
            // otherwise bring up the driver GUI so the user can change settings...
            if (m_formsetup.IsCustomDsDataSupported())
            {
                szTwmemref = "FALSE,FALSE," + this.Handle;
            }
            else
            {
                szTwmemref = "TRUE,FALSE," + this.Handle;
            }

            // Send the command...
            ClearEvents();
            TWAIN.TW_USERINTERFACE twuserinterface = default(TWAIN.TW_USERINTERFACE);
            m_twain.CsvToUserinterface(ref twuserinterface, szTwmemref);
            sts = m_twain.DatUserinterface(TWAIN.DG.CONTROL, TWAIN.MSG.ENABLEDS, ref twuserinterface); // SCAN SEQ
            if (sts == TWAIN.STS.SUCCESS)
            {
                SetButtons(EBUTTONSTATE.SCANNING);
            }
        }

        /// <summary>
        /// Clear our event list, and reset our event...
        /// </summary>

        public void ClearEvents()
        {
            m_blXferReadySent = false;
            m_blDisableDsSent = false;
        }

        /// <summary>
        /// Load an image into a picture box, maintain its aspect ratio...
        /// </summary>
        /// <param name="a_picturebox"></param>
        /// <param name="a_graphics"></param>
        /// <param name="a_bitmapGraphic"></param>
        /// <param name="a_bitmap"></param>
        private void LoadImage(ref PictureBox a_picturebox, ref Graphics a_graphics, ref Bitmap a_bitmapGraphic, Bitmap a_bitmap)
        {
            // We want to maintain the aspect ratio...
            double fRatioWidth = (double)a_bitmapGraphic.Size.Width / (double)a_bitmap.Width;
            double fRatioHeight = (double)a_bitmapGraphic.Size.Height / (double)a_bitmap.Height;
            double fRatio = (fRatioWidth < fRatioHeight) ? fRatioWidth : fRatioHeight;
            int iWidth = (int)(a_bitmap.Width * fRatio);
            int iHeight = (int)(a_bitmap.Height * fRatio);

            // Display the image...
            a_graphics.FillRectangle(m_brushBackground, m_rectangleBackground);
            a_graphics.DrawImage(a_bitmap, new Rectangle(((int)a_bitmapGraphic.Width - iWidth) / 2, ((int)a_bitmapGraphic.Height - iHeight) / 2, iWidth, iHeight));
            a_picturebox.Image = a_bitmapGraphic;
            a_picturebox.Update();
        }

        /// <summary>
        /// Initialize the picture boxes and the graphics to support them, we're
        /// doing this to maximize performance during scanner...
        /// </summary>
        private void InitImage()
        {
            // Make sure our picture boxes don't do much work...
            m_pictureboxImage1.SizeMode = PictureBoxSizeMode.Normal;
            m_pictureboxImage2.SizeMode = PictureBoxSizeMode.Normal;

            m_bitmapGraphic1 = new Bitmap(m_pictureboxImage1.Width, m_pictureboxImage1.Height, PixelFormat.Format32bppPArgb);
            m_graphics1 = Graphics.FromImage(m_bitmapGraphic1);
            m_graphics1.CompositingMode = CompositingMode.SourceCopy;
            m_graphics1.CompositingQuality = CompositingQuality.HighSpeed;
            m_graphics1.InterpolationMode = InterpolationMode.Low;
            m_graphics1.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            m_graphics1.SmoothingMode = SmoothingMode.HighSpeed;

            m_bitmapGraphic2 = new Bitmap(m_pictureboxImage1.Width, m_pictureboxImage1.Height, PixelFormat.Format32bppPArgb);
            m_graphics2 = Graphics.FromImage(m_bitmapGraphic2);
            m_graphics2.CompositingMode = CompositingMode.SourceCopy;
            m_graphics2.CompositingQuality = CompositingQuality.HighSpeed;
            m_graphics2.InterpolationMode = InterpolationMode.Low;
            m_graphics2.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            m_graphics2.SmoothingMode = SmoothingMode.HighSpeed;

            m_brushBackground = new SolidBrush(Color.White);
            m_rectangleBackground = new Rectangle(0, 0, m_bitmapGraphic1.Width, m_bitmapGraphic1.Height);
        }

        /// <summary>
        /// TWAIN needs help, if we want it to run stuff in our main
        /// UI thread...
        /// </summary>
        /// <param name="code">the code to run</param>
        private void RunInUiThread(Action a_action)
        {
            RunInUiThread(this, a_action);
        }

        /// <summary>
        /// TWAIN needs help, if we want it to run stuff in our main
        /// UI thread...
        /// </summary>
        /// <param name="control">the control to run in</param>
        /// <param name="code">the code to run</param>
        static public void RunInUiThread(Object a_object, Action a_action)
        {
            Control control = (Control)a_object;
            if (control.InvokeRequired)
            {
                control.Invoke(new FormScan.RunInUiThreadDelegate(RunInUiThread), new object[] { a_object, a_action });
                return;
            }
            a_action();
        }

        /// <summary>
        /// Configure our buttons to match our current state...
        /// </summary>
        /// <param name="a_ebuttonstate"></param>
        private void SetButtons(EBUTTONSTATE a_ebuttonstate)
        {
            switch (a_ebuttonstate)
            {
                default:
                case EBUTTONSTATE.CLOSED:
                    m_buttonOpen.Enabled = true;
                    m_buttonClose.Enabled = false;
                    m_buttonSetup.Enabled = false;
                    m_buttonScan.Enabled = false;
                    m_buttonStop.Enabled = false;
                    break;

                case EBUTTONSTATE.OPEN:
                    m_buttonOpen.Enabled = false;
                    m_buttonClose.Enabled = true;
                    m_buttonSetup.Enabled = true;
                    m_buttonScan.Enabled = true;
                    m_buttonStop.Enabled = false;
                    break;

                case EBUTTONSTATE.SCANNING:
                    m_buttonOpen.Enabled = false;
                    m_buttonClose.Enabled = false;
                    m_buttonSetup.Enabled = false;
                    m_buttonScan.Enabled = false;
                    m_buttonStop.Enabled = true;
                    break;
            }
        }


        /// <summary>
        /// Select and open a TWAIN driver...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonOpen_Click(object sender, EventArgs e)
        {
            string szIdentity;
            string szDefault = "";
            string szStatus;
            List<string> lszIdentity = new List<string>();
            FormSelect formselect;
            DialogResult dialogresult;
            TWAIN.STS sts;
            TWAIN.TW_CAPABILITY twcapability;
            TWAIN.TW_IDENTITY twidentity = default(TWAIN.TW_IDENTITY);

            // Get the default driver...
            m_intptrHwnd = this.Handle;
            sts = m_twain.DatParent(TWAIN.DG.CONTROL, TWAIN.MSG.OPENDSM, ref m_intptrHwnd);
            Console.WriteLine("STS Token 1: {0}", sts);
            if (sts != TWAIN.STS.SUCCESS)
            {
                MessageBox.Show("OPENDSM failed...");
                return;
            }

            // Get the default driver...
            sts = m_twain.DatIdentity(TWAIN.DG.CONTROL, TWAIN.MSG.GETDEFAULT, ref twidentity);
            if (sts == TWAIN.STS.SUCCESS)
            {
                szDefault = TWAIN.IdentityToCsv(twidentity);
                Console.WriteLine("Identity(Default): {0}", szDefault);
            }

            // Enumerate the drivers...
            for (sts = m_twain.DatIdentity(TWAIN.DG.CONTROL, TWAIN.MSG.GETFIRST, ref twidentity);
                 sts != TWAIN.STS.ENDOFLIST;
                 sts = m_twain.DatIdentity(TWAIN.DG.CONTROL, TWAIN.MSG.GETNEXT, ref twidentity))
            {
                string line = TWAIN.IdentityToCsv(twidentity);
                lszIdentity.Add(line);
                Console.WriteLine("Scanner({0}): {1}", lszIdentity.Count, line);
            }

            // Ruh-roh...
            if (lszIdentity.Count == 0)
            {
                MessageBox.Show("На цій ситемі відсутні TWAIN драйвери...");
                return;
            }

            // Instantiate our form...
            formselect = new FormSelect(lszIdentity, szDefault);
            formselect.StartPosition = FormStartPosition.CenterParent;
            dialogresult = formselect.ShowDialog(this);
            if (dialogresult != System.Windows.Forms.DialogResult.OK)
            {
                m_blExit = true;
                return;
            }

            // Get all the identities...
            szIdentity = formselect.GetSelectedDriver();
            if (szIdentity == null)
            {
                m_blExit = true;
                return;
            }

            // Get the selected identity...
            m_blExit = true;
            foreach (string sz in lszIdentity)
            {
                if (sz.Contains(szIdentity))
                {
                    m_blExit = false;
                    szIdentity = sz;
                    break;
                }
            }
            if (m_blExit)
            {
                return;
            }

            // Make it the default, we don't care if this succeeds...
            twidentity = default(TWAIN.TW_IDENTITY);
            TWAIN.CsvToIdentity(ref twidentity, szIdentity);
            m_twain.DatIdentity(TWAIN.DG.CONTROL, TWAIN.MSG.SET, ref twidentity);

            // Open it...
            sts = m_twain.DatIdentity(TWAIN.DG.CONTROL, TWAIN.MSG.OPENDS, ref twidentity);
            Console.WriteLine("OpenDS: {0}", sts);
            if (sts != TWAIN.STS.SUCCESS)
            {
                MessageBox.Show("Неможливо відкрити сканер (перевірте фізичне підключення та електричне ввімкнення)");
                m_blExit = true;
                return;
            }

            // Update the main form title...
            this.Text = "МІА: Сканування (" + twidentity.ProductName.Get() + ")";

            // Strip off unsafe chars.  Sadly, mono let's us down here...
            m_szProductDirectory = CSV.Parse(szIdentity)[11];
            foreach (char c in new char [41]
                            { '\x00', '\x01', '\x02', '\x03', '\x04', '\x05', '\x06', '\x07',
                              '\x08', '\x09', '\x0A', '\x0B', '\x0C', '\x0D', '\x0E', '\x0F', '\x10', '\x11', '\x12', 
                              '\x13', '\x14', '\x15', '\x16', '\x17', '\x18', '\x19', '\x1A', '\x1B', '\x1C', '\x1D', 
                              '\x1E', '\x1F', '\x22', '\x3C', '\x3E', '\x7C', ':', '*', '?', '\\', '/'
                            }
                    )
            {
                m_szProductDirectory = m_szProductDirectory.Replace(c, '_');
            }

            // We're doing memory transfers...
            szStatus = "";
            twcapability = default(TWAIN.TW_CAPABILITY);
            m_twain.CsvToCapability(ref twcapability, ref szStatus, "ICAP_XFERMECH,TWON_ONEVALUE,TWTY_UINT16,TWSX_NATIVE");
            sts = m_twain.DatCapability(TWAIN.DG.CONTROL, TWAIN.MSG.SET, ref twcapability);
            Console.WriteLine("XferMechanism(Native): {0}", sts);
            if (sts != TWAIN.STS.SUCCESS)
            {
                m_blExit = true;
                return;
            }

            // We're doing memory transfers...
            szStatus = "";
            twcapability = default(TWAIN.TW_CAPABILITY);
            m_twain.CsvToCapability(ref twcapability, ref szStatus, "CAP_AUTOFEED,TWON_ONEVALUE,TWTY_BOOL,TRUE");
            sts = m_twain.DatCapability(TWAIN.DG.CONTROL, TWAIN.MSG.SET, ref twcapability);
            Console.WriteLine("AutoFeed(1): {0}", sts);
            if (sts != TWAIN.STS.SUCCESS)
            {
                m_blExit = true;
                return;
            }

            // We're doing memory transfers...
            szStatus = "";
            twcapability = default(TWAIN.TW_CAPABILITY);
            m_twain.CsvToCapability(ref twcapability, ref szStatus, "CAP_AUTOSCAN,TWON_ONEVALUE,TWTY_BOOL,TRUE");
            sts = m_twain.DatCapability(TWAIN.DG.CONTROL, TWAIN.MSG.SET, ref twcapability);
            Console.WriteLine("AutoScan(1): {0}", sts);
            if (sts != TWAIN.STS.SUCCESS)
            {
                m_blExit = true;
                return;
            }

            // We're doing memory transfers...
            szStatus = "";
            twcapability = default(TWAIN.TW_CAPABILITY);
            m_twain.CsvToCapability(ref twcapability, ref szStatus, "CAP_DUPLEXENABLED,TWON_ONEVALUE,TWTY_BOOL,TRUE");
            sts = m_twain.DatCapability(TWAIN.DG.CONTROL, TWAIN.MSG.SET, ref twcapability);
            Console.WriteLine("Duplex(1): {0}", sts);
            if (sts != TWAIN.STS.SUCCESS)
            {
                m_blExit = true;
                return;
            }

            // Decide whether or not to show the driver's window messages...
            szStatus = "";
            twcapability = default(TWAIN.TW_CAPABILITY);
            m_twain.CsvToCapability(ref twcapability, ref szStatus, "CAP_INDICATORS,TWON_ONEVALUE,TWTY_BOOL," + (m_blIndicators ? "TRUE" : "FALSE"));
            sts = m_twain.DatCapability(TWAIN.DG.CONTROL, TWAIN.MSG.SET, ref twcapability);
            Console.WriteLine("Indicators(1): {0}", sts);
            if (sts != TWAIN.STS.SUCCESS)
            {
                m_blExit = true;
                return;
            }

            // New state...
            SetButtons(EBUTTONSTATE.OPEN);

            // Create the setup form...
            m_formsetup = new FormSetup(this, ref m_twain, m_szProductDirectory);
            Console.WriteLine("OPENED");
        }

        /// <summary>
        /// Close the currently open TWAIN driver...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonClose_Click(object sender, EventArgs e)
        {
            Rollback(TWAIN.STATE.S2);
            SetButtons(EBUTTONSTATE.CLOSED);
            m_formsetup.Dispose();
            m_formsetup = null;
            Console.WriteLine("Close Click");
        }

        /// <summary>
        /// Request that scanning stop (gracefully)...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonStop_Click(object sender, EventArgs e)
        {
            TWAIN.TW_PENDINGXFERS twpendingxfers = default(TWAIN.TW_PENDINGXFERS);
            TWAIN.STS sts = m_twain.DatPendingxfers(TWAIN.DG.CONTROL, TWAIN.MSG.STOPFEEDER, ref twpendingxfers);
            Console.WriteLine("STOP FEEDER: {0}", sts);
        }

        ///////////////////////////////////////////////////////////////////////////////
        // Private Definitons...
        ///////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Our button states...
        /// </summary>
        private enum EBUTTONSTATE
        {
            CLOSED,
            OPEN,
            SCANNING
        }

        ///////////////////////////////////////////////////////////////////////////////
        // Private Attributes...
        ///////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Use if something really bad happens...
        /// </summary>
        private bool m_blExit;

        /// <summary>
        /// Our interface to TWAIN...
        /// </summary>
        private TWAIN m_twain;
        private IntPtr m_intptrHwnd;
        private bool m_blDisableDsSent = false;
        private bool m_blXferReadySent = false;
        private IntPtr m_intptrXfer = IntPtr.Zero;
        private IntPtr m_intptrImage = IntPtr.Zero;
        private int m_iImageBytes = 0;
        private TWAIN.TW_SETUPMEMXFER m_twsetupmemxfer;

        // Setup information...
        private FormSetup m_formsetup;

        /// <summary>
        /// We use this name (modified and made file system safe)
        /// as the place where we'll put customdsdata...
        /// </summary>
        private string m_szProductDirectory;

        /// <summary>
        /// If true, then show the driver's window messages while
        /// we're scanning.  Set this in the constructor...
        /// </summary>
        private bool m_blIndicators;

        /// <summary>
        /// Stuff used to display the images...
        /// </summary>
        private Bitmap m_bitmapGraphic1;
        private Bitmap m_bitmapGraphic2;
        private Graphics m_graphics1;
        private Graphics m_graphics2;
        private Brush m_brushBackground;
        private Rectangle m_rectangleBackground;
        private int m_iUseBitmap;
        private int m_iImageCount = 0;

        /// <summary>
        /// We use this to run code in the context of the caller's UI thread...
        /// </summary>
        /// <param name="a_object">object (really a control)</param>
        /// <param name="a_action">code to run</param>
        public delegate void RunInUiThreadDelegate(Object a_object, Action a_action);

    }
}
