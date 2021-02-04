using System;
using System.Threading;
using System.Collections.Generic;
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
        /// Constructor
        /// </summary>
        public HCommAirInterface()
        {
            // set manager event
            Manager.ToolAdded += OnToolConnect;
            Manager.ToolRemoved += OnToolDisconnect;
            Manager.ToolConnect += OnToolConnect;
            Manager.ToolDisconnect += OnToolDisconnect;
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
        /// Get registered tool list
        /// </summary>
        /// <returns>too list</returns>
        public List<HcToolInfo> GetRegisteredTools() => Manager.GetRegisteredTools();
        /// <summary>
        /// Get scanned tool list
        /// </summary>
        /// <returns>result</returns>
        public List<HcToolInfo> GetScannedTools() => Manager.GetScannedTools();
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
        /// Get session
        /// </summary>
        /// <param name="info">tool information</param>
        /// <returns>result</returns>
        public HcSession GetSessions(HcToolInfo info)
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
                session.SetUp(CommType.Ethernet);
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
        private void OnConnectionChanged(HcToolInfo info, ConnectionState state)
        {
            // debug
            Console.WriteLine($@"== {info.Ip} / {state}");
            // event
            ChangedConnect?.Invoke(info, state);
        }
        private void OnSessionReceived(HcToolInfo info, Command cmd, int addr, int[] values)
        {
            ReceivedMsg?.Invoke(info, cmd, addr, values);
        }
    }
}
