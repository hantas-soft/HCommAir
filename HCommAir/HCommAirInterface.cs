using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using HComm.Common;
using HCommAir.Manager;
using HCommAir.Tools;

namespace HCommAir
{
    /// <summary>
    /// HCommAir tool interface class
    /// </summary>
    public class HCommAirInterface
    {
        private const int TickPeriod = 500;
        private List<HcSession> Sessions { get; } = new List<HcSession>();
        private HcManager Manager { get; } = new HcManager();

        /// <summary>
        /// HCommAir session connection changed event delegate
        /// </summary>
        /// <param name="info">tool information</param>
        /// <param name="state">connection state</param>
        public delegate void SessionConnectChanged(HcToolInfo info, ConnectionState state);
        /// <summary>
        /// HCommAir session received event delegate
        /// </summary>
        /// <param name="info"></param>
        /// <param name="cmd"></param>
        /// <param name="addr"></param>
        /// <param name="values"></param>
        public delegate void SessionReceived(HcToolInfo info, Command cmd, int addr, int[] values);
        /// <summary>
        /// HCommAir session connection changed event
        /// </summary>
        public event SessionConnectChanged ChangedConnect;
        /// <summary>
        /// HCommAir session received event
        /// </summary>
        public event SessionReceived ReceivedMsg;
        /// <summary>
        /// HCommAir session max queue size
        /// </summary>
        public int MaxQueueSize { get; set; } = 30;
        /// <summary>
        /// HCommAir session max block size
        /// </summary>
        public int MaxBlockSize { get; set; } = 100;
        /// <summary>
        /// HCommAir used sub tool manager
        /// </summary>
        public bool UseSubTool
        {
            get => Manager.UseSubTools;
            set => Manager.UseSubTools = value;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public HCommAirInterface()
        {
            // set manager event
            Manager.ToolAdded += OnToolConnect;
            Manager.ToolRemoved += OnToolRemoved;
            Manager.ToolConnect += OnToolConnect;
            Manager.ToolDisconnect += OnToolDisconnect;
            Manager.ToolAlive += OnToolAlive;
        }
        /// <summary>
        /// Tool scanning start
        /// </summary>
        public void Start() => Manager.Start();
        /// <summary>
        /// Tool scanning stop
        /// </summary>
        public void Stop() => Manager.Stop();
        /// <summary>
        /// Interface properties change
        /// </summary>
        /// <param name="p">properties</param>
        public void ChangeInterfaceProp(IPv4InterfaceProperties p) => Manager.ChangeInterfaceProp(p);
        /// <summary>
        /// Save registered tool list
        /// </summary>
        /// <param name="path">file path</param>
        /// <returns>result</returns>
        public bool SaveRegisterTools(string path) => Manager.SaveRegisterTools(path);
        /// <summary>
        /// Load registered tool list
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool LoadRegisterTools(string path) => Manager.LoadRegisterTools(path);
        /// <summary>
        /// Get scanned tool list
        /// </summary>
        /// <returns>result</returns>
        public List<HcToolInfo> GetScannedTools() => Manager.GetScannedTools();
        /// <summary>
        /// Get registered tool list
        /// </summary>
        /// <returns>too list</returns>
        public List<HcToolInfo> GetRegisteredTools() => Manager.GetRegisteredTools();
        /// <summary>
        /// Get sub-registered tool list
        /// </summary>
        /// <returns>too list</returns>
        public List<HcToolInfo> GetSubRegisteredTools() => Manager.GetSubRegisteredTools();
        /// <summary>
        /// Register tool
        /// </summary>
        /// <param name="info">tool information</param>
        /// <returns>result</returns>
        public bool RegisterTool(HcToolInfo info) => Manager.RegisterTool(info);
        /// <summary>
        /// UnRegister tool
        /// </summary>
        /// <param name="info">tool information</param>
        /// <returns>result</returns>
        public bool UnRegisterTool(HcToolInfo info) => Manager.UnRegisterTool(info);
        /// <summary>
        /// Sub-Register tool
        /// </summary>
        /// <param name="info">tool information</param>
        /// <returns>result</returns>
        public bool SubRegisterTool(HcToolInfo info) => Manager.SubRegisterTool(info);
        /// <summary>
        /// Sub-UnRegister tool
        /// </summary>
        /// <param name="info">tool information</param>
        /// <returns>result</returns>
        public bool SubUnRegisterTool(HcToolInfo info) => Manager.SubUnRegisterTool(info);
        /// <summary>
        /// Get session
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
        /// Get all sessions
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
        /// All sessions stop event monitoring
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
        /// Connect manual tool
        /// </summary>
        /// <param name="portName">port name</param>
        public void ConnectManualTool(string portName)
        {
            var info = new HcToolInfo();
            // get values
            var values = info.GetValues();
            // change com port
            values[23] = Convert.ToByte(portName.Substring(3));
            // change baud rate
            values[24] = (57600 >> 8) & 0xFF;
            values[25] = 57600 & 0xFF;
            // change mac number
            values[31] = values[23];
            // set values
            info.SetValues(values);
            // connect tool
            OnToolConnect(info);
        }
        /// <summary>
        /// Disconnect manual tool
        /// </summary>
        /// <param name="portName"></param>
        public void DisConnectManualTool(string portName)
        {
            var info = new HcToolInfo();
            // get values
            var values = info.GetValues();
            // change com port
            values[23] = Convert.ToByte(portName.Substring(3));
            // change mac number
            values[31] = values[23];
            // set values
            info.SetValues(values);
            // disconnect tool
            OnToolRemoved(info);
        }
        /// <summary>
        /// Stop sub tools 
        /// </summary>
        public void StopSubTools()
        {
            var lockTaken = false;
            try
            {
                // lock
                Monitor.TryEnter(Sessions, TickPeriod, ref lockTaken);
                // check lock taken
                if (!lockTaken)
                    return;

                foreach (var session in GetSubRegisteredTools()
                    .Select(tool => Sessions.Find(x => x.ToolInfo.Mac == tool.Mac)))
                {
                    // check session
                    if (session == null)
                        continue;
                    // disconnect
                    session.Disconnect();
                    // remove
                    Sessions.Remove(session);
                }
            }
            finally
            {
                // check lock taken
                if (lockTaken)
                    // unlock
                    Monitor.Exit(Sessions);
            }
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
                // check tool connect available
                if (!info.AvailableConnection)
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
                session.SetUp(info.Serial != string.Empty ? CommType.Ethernet : CommType.Serial);
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
        private void OnToolAlive(HcToolInfo info)
        {
            var lockTaken = false;
            try
            {
                // lock
                Monitor.TryEnter(Sessions, TickPeriod, ref lockTaken);
                // check lock taken
                if (!lockTaken)
                    return;
                // check tool connect available
                if (!info.AvailableConnection)
                    return;
                // find session
                var session = Sessions.Find(x => x.ToolInfo.Mac == info.Mac);
                // check session
                if (session != null)
                    return;
                // get registered tools
                var register = GetRegisteredTools().Find(x => x.Mac == info.Mac);
                var subRegister = GetSubRegisteredTools().Find(x => x.Mac == info.Mac);
                // check session
                if (register != null && subRegister == null)
                {
                    // new session
                    session = new HcSession(info);
                    // set session event
                    session.ConnectionChanged += OnConnectionChanged;
                    session.SessionReceived += OnSessionReceived;
                    session.EventReceived += OnSessionReceived;
                    // setup
                    session.SetUp(info.Serial != string.Empty ? CommType.Ethernet : CommType.Serial);
                    // set message queue size and block size
                    session.MaxQueueSize = MaxQueueSize;
                    session.MaxBlockSize = MaxBlockSize;
                    // connect session
                    session.Connect();
                    // add session
                    Sessions.Add(session);
                }
                else if (register == null && UseSubTool && subRegister != null)
                {
                    // new session
                    session = new HcSession(info);
                    // set session event
                    session.ConnectionChanged += OnConnectionChanged;
                    session.SessionReceived += OnSessionReceived;
                    session.EventReceived += OnSessionReceived;
                    // setup
                    session.SetUp(info.Serial != string.Empty ? CommType.Ethernet : CommType.Serial);
                    // set message queue size and block size
                    session.MaxQueueSize = MaxQueueSize;
                    session.MaxBlockSize = MaxBlockSize;
                    // connect session
                    session.Connect();
                    // add session
                    Sessions.Add(session);
                }
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