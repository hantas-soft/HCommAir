using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using HComm.Common;
using HCommAir.Manager;
using HCommAir.Tools;

namespace HCommAir
{
    /// <summary>
    ///     HCommAir tool interface class
    /// </summary>
    public class HCommAirInterface
    {
        /// <summary>
        ///     HCommAir session connection changed event delegate
        /// </summary>
        /// <param name="info">tool information</param>
        /// <param name="state">connection state</param>
        public delegate void SessionConnectChanged(HcToolInfo info, ConnectionState state);

        /// <summary>
        ///     HCommAir session received event delegate
        /// </summary>
        /// <param name="info"></param>
        /// <param name="cmd"></param>
        /// <param name="addr"></param>
        /// <param name="values"></param>
        public delegate void SessionReceived(HcToolInfo info, Command cmd, int addr, int[] values);

        private const int TickPeriod = 500;

        /// <summary>
        ///     Constructor
        /// </summary>
        public HCommAirInterface()
        {
            // set manager event
            Manager.ToolAdded += OnToolConnect;
            Manager.ToolRemoved += OnToolRemoved;
            Manager.ToolConnect += OnToolConnect;
            Manager.ToolDisconnect += OnToolDisconnect;
        }

        private List<HcSession> Sessions { get; } = new List<HcSession>();
        private HcManager Manager { get; } = new HcManager();

        /// <summary>
        ///     HCommAir session max queue size
        /// </summary>
        public int MaxQueueSize { get; set; } = 30;

        /// <summary>
        ///     HCommAir session max block size
        /// </summary>
        public int MaxBlockSize { get; set; } = 100;

        /// <summary>
        ///     HCommAir session connection changed event
        /// </summary>
        public event SessionConnectChanged ChangedConnect;

        /// <summary>
        ///     HCommAir session received event
        /// </summary>
        public event SessionReceived ReceivedMsg;

        /// <summary>
        ///     Tool scanning start
        /// </summary>
        public void Start()
        {
            Manager.Start();
        }

        /// <summary>
        ///     Tool scanning stop
        /// </summary>
        public void Stop()
        {
            Manager.Stop();
        }

        /// <summary>
        ///     Interface properties change
        /// </summary>
        /// <param name="p">properties</param>
        public void ChangeInterfaceProp(IPv4InterfaceProperties p)
        {
            Manager.ChangeInterfaceProp(p);
        }

        /// <summary>
        ///     Save registered tool list
        /// </summary>
        /// <param name="path">file path</param>
        /// <returns>result</returns>
        public bool SaveRegisterTools(string path)
        {
            return Manager.SaveRegisterTools(path);
        }

        /// <summary>
        ///     Load registered tool list
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool LoadRegisterTools(string path)
        {
            return Manager.LoadRegisterTools(path);
        }

        /// <summary>
        ///     Get registered tool list
        /// </summary>
        /// <returns>too list</returns>
        public List<HcToolInfo> GetRegisteredTools()
        {
            return Manager.GetRegisteredTools();
        }

        /// <summary>
        ///     Get scanned tool list
        /// </summary>
        /// <returns>result</returns>
        public List<HcToolInfo> GetScannedTools()
        {
            return Manager.GetScannedTools();
        }

        /// <summary>
        ///     Register tool
        /// </summary>
        /// <param name="info">tool information</param>
        /// <returns>result</returns>
        public bool RegisterTool(HcToolInfo info)
        {
            return Manager.RegisterTool(info);
        }

        /// <summary>
        ///     UnRegister tool
        /// </summary>
        /// <param name="info">tool information</param>
        /// <returns>result</returns>
        public bool UnRegisterTool(HcToolInfo info)
        {
            return Manager.UnRegisterTool(info);
        }

        /// <summary>
        ///     Get session
        /// </summary>
        /// <param name="info">tool information</param>
        /// <returns>result</returns>
        public HcSession GetSession(HcToolInfo info)
        {
            var lockTaken = false;
            try
            {
                // lock
                Monitor.TryEnter(Sessions, TickPeriod, ref lockTaken);
                // find session
                return !lockTaken ? null : Sessions.Find(x => x.ToolInfo.Mac == info.Mac);
            }
            finally
            {
                // check lock taken
                if (lockTaken)
                    // unlock
                    Monitor.Exit(Sessions);
            }
        }

        /// <summary>
        ///     Get all sessions
        /// </summary>
        /// <returns>result</returns>
        public List<HcSession> GetAllSessions()
        {
            var lockTaken = false;
            try
            {
                // lock
                Monitor.TryEnter(Sessions, TickPeriod, ref lockTaken);
                // return sessions
                return Sessions;
            }
            finally
            {
                // check lock taken
                if (lockTaken)
                    // unlock
                    Monitor.Exit(Sessions);
            }
        }

        /// <summary>
        ///     All sessions stop event monitoring
        /// </summary>
        public void StopAllSessionsEventMonitor()
        {
            var lockTaken = false;
            try
            {
                // lock
                Monitor.TryEnter(Sessions, TickPeriod, ref lockTaken);
                // check lock taken
                if (!lockTaken)
                    return;

                // check sessions
                foreach (var session in Sessions)
                    // stop event monitor
                    session.SetEventMonitor(4015, 0);
            }
            finally
            {
                // check lock taken
                if (lockTaken)
                    // unlock
                    Monitor.Exit(Sessions);
            }
        }

        /// <summary>
        ///     Connect manual tool
        /// </summary>
        /// <param name="target">target</param>
        /// <param name="option">option</param>
        /// <param name="id">id</param>
        /// <param name="type">type</param>
        public void ConnectManualTool(string target, int option = 115200, byte id = 1, CommType type = CommType.Serial)
        {
            var info = new HcToolInfo();
            // get values
            var values = info.GetValues();
            // check type
            switch (type)
            {
                case CommType.None:
                    break;
                case CommType.Serial:
                    var port = Convert.ToInt32(target.Substring(3));
                    // get com port
                    values[22] = Convert.ToByte((port >> 8) & 0xFF);
                    values[23] = Convert.ToByte(port & 0xFF);
                    // set baud rate
                    values[26] = (byte)((option >> 24) & 0xFF);
                    values[27] = (byte)((option >> 16) & 0xFF);
                    values[28] = (byte)((option >> 8) & 0xFF);
                    values[29] = (byte)(option & 0xFF);
                    // set mac number
                    values[31] = values[23];
                    // set id
                    values[34] = id;
                    break;
                case CommType.Ethernet:
                    // get ip address
                    var ip = target.Split('.');
                    for (var i = 0; i < ip.Length; i++)
                        values[20 + i] = Convert.ToByte(ip[i]);
                    // set port
                    values[24] = (byte)((option >> 8) & 0xFF);
                    values[25] = (byte)(option & 0xFF);
                    // set mac number
                    values[31] = values[23];
                    // set id
                    values[34] = id;
                    break;
                case CommType.Usb:
                    // set usb
                    values[23] = 0x55;
                    // set mac number
                    values[31] = 0x55;
                    // set id
                    values[34] = id;
                    break;
                default:
                    return;
            }

            // set type
            values[33] = (byte)type;
            // set values
            info.SetValues(values);
            // connect tool
            OnToolConnect(info);
        }

        /// <summary>
        ///     Disconnect manual tool
        /// </summary>
        /// <param name="target">target</param>
        /// <param name="option">option</param>
        /// <param name="id">id</param>
        /// <param name="type">type</param>
        public void DisConnectManualTool(string target, int option = 115200, byte id = 1,
            CommType type = CommType.Serial)
        {
            var info = new HcToolInfo();
            // get values
            var values = info.GetValues();
            // check type
            switch (type)
            {
                case CommType.None:
                    break;
                case CommType.Serial:
                    var port = Convert.ToInt32(target.Substring(3));
                    // get com port
                    values[22] = Convert.ToByte((port >> 8) & 0xFF);
                    values[23] = Convert.ToByte(port & 0xFF);
                    // set baud rate
                    values[26] = (byte)((option >> 24) & 0xFF);
                    values[27] = (byte)((option >> 16) & 0xFF);
                    values[28] = (byte)((option >> 8) & 0xFF);
                    values[29] = (byte)(option & 0xFF);
                    // set mac number
                    values[31] = values[23];
                    // set id
                    values[34] = id;
                    break;
                case CommType.Ethernet:
                    // get ip address
                    var ip = target.Split('.');
                    for (var i = 0; i < ip.Length; i++)
                        values[20 + i] = Convert.ToByte(ip[i]);
                    // set port
                    values[24] = (byte)((option >> 8) & 0xFF);
                    values[24] = (byte)(option & 0xFF);
                    // set mac number
                    values[31] = values[23];
                    // set id
                    values[34] = id;
                    break;
                case CommType.Usb:
                    // set mac number
                    values[31] = 0x55;
                    // set id
                    values[34] = id;
                    break;
                default:
                    return;
            }

            // set values
            info.SetValues(values);
            // connect tool
            OnToolRemoved(info);
        }

        private void OnToolConnect(HcToolInfo info)
        {
            var lockTaken = false;
            try
            {
                // lock
                Monitor.TryEnter(Sessions, TickPeriod, ref lockTaken);
                // check lock taken
                if (!lockTaken)
                    return;

                // find session
                var session = Sessions.Find(x => x.ToolInfo.Mac == info.Mac);
                // check session
                if (session == null)
                {
                    // new session
                    session = new HcSession(info);
                    // set session event
                    session.ConnectionChanged += OnConnectionChanged;
                    session.SessionReceived += OnSessionReceived;
                    session.EventReceived += OnSessionReceived;
                    // add session
                    Sessions.Add(session);
                }

                // setup
                session.SetUp(info.Serial != string.Empty ? CommType.Ethernet : (CommType)info.GetValues()[33]);
                // set message queue size and block size
                session.MaxQueueSize = MaxQueueSize;
                session.MaxBlockSize = MaxBlockSize;
                // connect session
                session.Connect();
            }
            finally
            {
                // check lock taken
                if (lockTaken)
                    // unlock
                    Monitor.Exit(Sessions);
            }
        }

        private void OnToolDisconnect(HcToolInfo info)
        {
            var lockTaken = false;
            try
            {
                // lock
                Monitor.TryEnter(Sessions, TickPeriod, ref lockTaken);
                // check lock taken
                if (!lockTaken)
                    return;

                // get session
                var session = Sessions.Find(x => x.ToolInfo.Mac == info.Mac);
                // check session
                if (session == null)
                    return;
                // disconnect
                session.Disconnect();
            }
            finally
            {
                // check lock taken
                if (lockTaken)
                    // unlock
                    Monitor.Exit(Sessions);
            }
        }

        private void OnToolRemoved(HcToolInfo info)
        {
            var lockTaken = false;
            try
            {
                // lock
                Monitor.TryEnter(Sessions, TickPeriod, ref lockTaken);
                // check lock taken
                if (!lockTaken)
                    return;

                // get session
                var session = Sessions.Find(x => x.ToolInfo.Mac == info.Mac);
                // check session
                if (session == null)
                    return;
                // disconnect
                session.Disconnect();
                // remove
                Sessions.Remove(session);
            }
            finally
            {
                // check lock taken
                if (lockTaken)
                    // unlock
                    Monitor.Exit(Sessions);
            }
        }

        private void OnConnectionChanged(HcToolInfo info, ConnectionState state)
        {
            // event
            ChangedConnect?.Invoke(info, state);
        }

        private void OnSessionReceived(HcToolInfo info, Command cmd, int addr, int[] values)
        {
            ReceivedMsg?.Invoke(info, cmd, addr, values);
        }
    }
}