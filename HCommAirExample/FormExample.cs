using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using HComm.Common;
using HCommAir;
using HCommAir.Manager;
using HCommAir.Tools;

namespace HCommAirExample
{
    public partial class FormExample : Form
    {
        public FormExample()
        {
            InitializeComponent();
        }

        private HCommAirInterface HCommAir { get; } = new HCommAirInterface();
        private BindingList<HcToolInfo> RegisterTools { get; } = new BindingList<HcToolInfo>();
        private BindingList<HcToolInfo> ScanTools { get; } = new BindingList<HcToolInfo>();
        private HcSession SelectedSession { get; set; }
        private bool GraphState { get; set; }
        private DateTime GraphTime { get; set; } = DateTime.Now;
        private StringBuilder Logger { get; } = new StringBuilder();
        
        private void FormExample_Load(object sender, EventArgs e)
        {
            // set path
            tbPath.Text = Properties.Settings.Default.Path;
            // binding list
            lbRegisteredTools.DataSource = RegisterTools;
            lbScannedTools.DataSource = ScanTools;
            // check port list
            foreach (var item in HComm.Device.HcSerial.GetPortNames())
                // add port name
                cbPorts.Items.Add(item);
            // check interface list
            foreach (var item in HcManager.GetAllInterfaces())
                // add interface item
                cbInterface.Items.Add($@"{item.Name}:{item.Id}");
            // check list
            if (cbInterface.Items.Count > 0)
                cbInterface.SelectedIndex = 0;

            // set event
            HCommAir.ChangedConnect += OnChangedConnect;
            HCommAir.ReceivedMsg += OnReceivedMsg;
            // load register tools
            HCommAir.LoadRegisterTools(tbPath.Text);
            // start timer
            timer.Start();
            // start scanner
            HCommAir.Start();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            var scanned = HCommAir.GetScannedTools();
            var registered = HCommAir.GetRegisteredTools();
            // check scanned tools count
            if (scanned.Count != ScanTools.Count)
            {
                // clear scanned tools item
                ScanTools.Clear();
                // add item list
                foreach (var info in scanned)
                    // add
                    ScanTools.Add(info);
                // refresh
                lbScannedTools.Refresh();
            }
            // check registered tools count
            if (registered.Count != RegisterTools.Count)
            {
                // clear register tools item
                RegisterTools.Clear();
                // add item list
                foreach (var info in registered)
                    // add
                    RegisterTools.Add(info);
                // refresh
                lbRegisteredTools.Refresh();
            }
            // debug
            if (SelectedSession != null)
            {
                SelectedSession.GetParam(1, 500);
                SelectedSession.GetParam(1001, 500);
                SelectedSession.GetParam(2001, 500);
            }
            // check graph state
            if (!GraphState || SelectedSession == null || SelectedSession.State != ConnectionState.Connected ||
                !((DateTime.Now - GraphTime).TotalSeconds > 5))
                return;
            // set
            SelectedSession.SetGraph();
            // reset time
            GraphTime = DateTime.Now;
        }
        private void btPath_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                // show dialog
                if (dlg.ShowDialog() != DialogResult.OK)
                    return;
                // set path
                tbPath.Text = $@"{dlg.SelectedPath}\RegisteredTools.bin";
                // save path
                Properties.Settings.Default.Path = tbPath.Text;
                Properties.Settings.Default.Save();
            }
        }
        private void btRegister_Click(object sender, EventArgs e)
        {
            var scanned = HCommAir.GetScannedTools();
            var registered = HCommAir.GetRegisteredTools();
            // check sender
            if (sender == btRegister)
            {
                // check index
                if (lbScannedTools.SelectedIndex < 0 || lbScannedTools.SelectedIndex > scanned.Count)
                    return;
                // get item
                var info = scanned.Find(x => x.Mac == scanned[lbScannedTools.SelectedIndex].Mac);
                // add item
                HCommAir.RegisterTool(info);
                // save tool list
                HCommAir.SaveRegisterTools(tbPath.Text);
            }
            else
            {
                // check index
                if (lbRegisteredTools.SelectedIndex < 0 || lbRegisteredTools.SelectedIndex > registered.Count)
                    return;
                // get item
                var info = registered.Find(x => x.Mac == registered[lbRegisteredTools.SelectedIndex].Mac);
                // remove item
                HCommAir.UnRegisterTool(info);
                // save tool list
                HCommAir.SaveRegisterTools(tbPath.Text);
            }
        }
        private void lbRegisteredTools_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get item
            var item = (HcToolInfo)lbRegisteredTools.SelectedItem;
            // check item
            if (item == null)
                return;
            // stop all sessions event monitoring
            HCommAir.StopAllSessionsEventMonitor();
            // select session
            SelectedSession = HCommAir.GetSession(item);
            // check selected session
            if (SelectedSession == null)
                return;
            // update information
            lbIp.Text = $@"IP: {SelectedSession.ToolInfo.Ip}";
            lbMac.Text = $@"MAC: {SelectedSession.ToolInfo.Mac}";
            lbSerial.Text = $@"S/N: {SelectedSession.ToolInfo.Serial}";
            lbState.Text = $@"STATE: {SelectedSession.State}";
        }
        private void btParamAction_Click(object sender, EventArgs e)
        {
            // check selected session
            if (SelectedSession == null || SelectedSession.State != ConnectionState.Connected)
                return;
            // get info
            var addr = Convert.ToUInt16(nmAddr.Value);
            var count = Convert.ToUInt16(nmCount.Value);
            // check sender
            if (sender == btGetParam)
                // get param
                SelectedSession.GetParam(addr, count);
            else if (sender == btSetParam)
                // set param
                SelectedSession.SetParam(addr, count);
        }
        private void btMonitorAction_Click(object sender, EventArgs e)
        {
            // check mac address
            if (SelectedSession == null || SelectedSession.State != ConnectionState.Connected)
                return;
            // check sender
            if (sender == btStartMonitor)
                SelectedSession.SetEventMonitor();
            else if (sender == btStopMonitor)
                SelectedSession.SetEventMonitor(4015, 0);
        }
        private void btGraphAction_Click(object sender, EventArgs e)
        {
            // check selected session
            if (SelectedSession == null || SelectedSession.State != ConnectionState.Connected)
                return;
            // check sender
            if (sender == btGraphSet)
            {
                // check state
                if (GraphState)
                    return;
                // get graph setting
                var ch1 = (ushort)cbCh1.SelectedIndex;
                var ch2 = (ushort)cbCh2.SelectedIndex;
                var sampling = (ushort)cbSampling.SelectedIndex;
                var option = (ushort)cbOption.SelectedIndex;
                // set graph setting
                SelectedSession.SetParam(4101, ch1);
                SelectedSession.SetParam(4102, ch2);
                SelectedSession.SetParam(4103, sampling);
                SelectedSession.SetParam(4104, option);
            }
            else if (sender == btGraphStart)
            {
                GraphState = true;
            }
            else if (sender == btGraphStop)
            {
                // stop
                SelectedSession.SetGraph(4100, 0);
                // reset state
                GraphState = false;
            }
        }
        private void btClear_Click(object sender, EventArgs e)
        {
            // clear
            Logger.Clear();
            // update
            tbLog.Text = $@"{Logger}";
        }
        private void btOpen_Click(object sender, EventArgs e)
        {
            // check text
            if (btOpen.Text == @"Open")
            {
                // get port name
                // var port = $@"{cbPorts.SelectedItem}";
                var port = $@"{tbIpAddr.Text}";
                // check port name
                if (port == string.Empty)
                    return;
                // Connect manual tool
                HCommAir.ConnectManualTool(port, 7762, 1, CommType.Ethernet);
            }
            else
            {
                // get port name
                // var port = $@"{cbPorts.SelectedItem}";
                var port = $@"{tbIpAddr.Text}";
                // check port name
                if (port == string.Empty)
                    return;
                // Disconnect manual tool
                HCommAir.DisConnectManualTool(port, 5000, 1, CommType.Ethernet);
                // set text
                btOpen.Text = @"Open";
            }
        }

        private void cbInterface_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get selected item
            var item = $@"{cbInterface.SelectedItem}".Split(':')[1];
            // find interface
            var inf = HcManager.GetAllInterfaces().Find(x => x.Id == item);
            // check interface
            if (inf == null)
                return;
            // change interface
            HCommAir.ChangeInterfaceProp(inf.GetIPProperties().GetIPv4Properties());
        }
        private void OnChangedConnect(HcToolInfo info, ConnectionState state)
        {
            // check tool serial
            if (info.Serial != string.Empty)
            {
                // check mac address
                if (SelectedSession == null || SelectedSession.ToolInfo.Mac != info.Mac)
                    return;
            }
            else
                // set selected tool
                SelectedSession = HCommAir.GetSession(info);

            
            Invoke(new EventHandler(delegate
            {
                // set state
                lbState.Text = $@"STATE: {state}";
            }));
        }
        private void OnReceivedMsg(HcToolInfo info, Command cmd, int addr, int[] values)
        {
            // check mac address
            //if (SelectedSession == null || SelectedSession.ToolInfo.Mac != info.Mac)
                //return;
            // add log
            AddLog($@"== {info.Ip} : Cmd:{cmd} / Addr:{addr} / Len:{values.Length}");
            // check command
            switch (cmd)
            {
                case Command.Read:
                    Invoke(new EventHandler(delegate
                    {
                        nmCount.Value = values[0];
                    }));
                    break;
                case Command.Mor:
                    break;
                case Command.Write:
                    break;
                case Command.Info:
                    // check tool serial
                    if (info.Serial != string.Empty)
                        break;
                    Invoke(new EventHandler(delegate
                    {
                        lbIp.Text = $@"IP: {info.Ip}";
                        btOpen.Text = @"CLOSE";
                    }));
                    break;
                case Command.Graph:
                    break;
                case Command.GraphRes:
                    break;
                case Command.GraphAd:
                    break;
                case Command.Error:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cmd), cmd, null);
            }
        }
        private void AddLog(string log, bool lineFeed = false)
        {
            // add time
            Logger.Append($@"[{DateTime.Now:HH:mm:ss.fff}] ");
            // check line feed
            if (lineFeed)
                // add line feed
                Logger.AppendLine();
            // add log
            Logger.AppendLine($@"{log}");
            // print
            tbLog.BeginInvoke(new Action(() =>
            {
                // set text
                tbLog.Text = Logger.ToString();
                // set scroll position end
                tbLog.SelectionStart = Logger.Length;
                tbLog.ScrollToCaret();
            }));
        }
    }
}