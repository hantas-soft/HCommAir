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

        private HCommAirInterface Interface { get; } = new HCommAirInterface();
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
            
            // set event
            Interface.ChangedConnect += InterfaceOnChangedConnect;
            Interface.ReceivedMsg += InterfaceOnReceivedMsg;
            // load register tools
            Interface.LoadRegisterTools(tbPath.Text);
            // start timer
            timer.Start();
            // start scanner
            Interface.Start();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            var scanned = Interface.GetScannedTools();
            var registered = Interface.GetRegisteredTools();
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
            var scanned = Interface.GetScannedTools();
            var registered = Interface.GetRegisteredTools();
            // check sender
            if (sender == btRegister)
            {
                // check index
                if (lbScannedTools.SelectedIndex < 0 || lbScannedTools.SelectedIndex > scanned.Count)
                    return;
                // get item
                var info = scanned.Find(x => x.Mac == scanned[lbScannedTools.SelectedIndex].Mac);
                // add item
                Interface.RegisterTool(info);
                // save tool list
                Interface.SaveRegisterTools(tbPath.Text);
            }
            else
            {
                // check index
                if (lbRegisteredTools.SelectedIndex < 0 || lbRegisteredTools.SelectedIndex > registered.Count)
                    return;
                // get item
                var info = registered.Find(x => x.Mac == registered[lbRegisteredTools.SelectedIndex].Mac);
                // remove item
                Interface.UnRegisterTool(info);
                // save tool list
                Interface.SaveRegisterTools(tbPath.Text);
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
            Interface.StopAllSessionsEventMonitor();
            // select session
            SelectedSession = Interface.GetSessions(item);
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
        private void InterfaceOnChangedConnect(HcToolInfo info, ConnectionState state)
        {
            // check mac address
            if (SelectedSession == null || SelectedSession.ToolInfo.Mac != info.Mac)
                return;
            
            Invoke(new EventHandler(delegate
            {
                // set state
                lbState.Text = $@"STATE: {state}";
            }));
        }
        private void InterfaceOnReceivedMsg(HcToolInfo info, Command cmd, int addr, int[] values)
        {
            // check mac address
            if (SelectedSession == null || SelectedSession.ToolInfo.Mac != info.Mac)
                return;
            var msg = $@"== {info.Ip} : Cmd:{cmd} / Addr:{addr} / Len:{values.Length}";
            // add log
            Logger.AppendLine(msg);
            // update
            Invoke(new EventHandler(delegate { tbLog.Text = $@"{Logger}"; }));
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
    }
}
