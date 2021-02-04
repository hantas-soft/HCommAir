using HComm;
using HComm.Common;
using HCommAir.Tools;

namespace HCommAir.Manager
{
    /// <summary>
    /// HCommAir tool session class
    /// </summary>
    public class HcSession
    {
        private int _maxQueueSize;
        private int _maxBlockSize;
        private HCommInterface Session { get; }
        private HCommInterface EventSession { get; }
        
        public HcToolInfo ToolInfo { get; }
        public ConnectionState State { get; private set; }
        public int MaxQueueSize
        {
            get => _maxQueueSize;
            set
            {
                // set max queue size
                _maxQueueSize = value;
                // set sessions max queue size
                Session.MaxQueueSize = _maxQueueSize;
                EventSession.MaxQueueSize = _maxQueueSize;
            }
        }
        public int MaxBlockSize
        {
            get => _maxBlockSize;
            set
            {
                // set max block size
                _maxBlockSize = value;
                // set sessions max queue size
                Session.MaxParamBlock = _maxBlockSize;
                EventSession.MaxParamBlock = _maxBlockSize;
            }
        }
        public delegate void ConnectionHandler(HcToolInfo info, ConnectionState state);
        public delegate void ReceivedHandler(HcToolInfo info, Command cmd, int addr, int[] values);
        public event ConnectionHandler ConnectionChanged;
        public event ReceivedHandler SessionReceived, EventReceived;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="info">Tool information</param>
        public HcSession(HcToolInfo info)
        {
            // set tool information
            ToolInfo = info;
            // sessions
            Session = new HCommInterface();
            EventSession = new HCommInterface();
            // reset state
            State = ConnectionState.Disconnected;
            // set event
            Session.ReceivedMsg = ReceivedMsg;
            Session.ChangedConnection = ChangedConnection;
            EventSession.ReceivedMsg = ReceivedEventMsg;
            EventSession.ChangedConnection = ChangedConnection;
            // set option
            EventSession.AutoRequestInfo = false;
        }
        /// <summary>
        /// SetUp session
        /// </summary>
        /// <param name="type">Session type</param>
        public void SetUp(CommType type)
        {
            // check type
            if (type == CommType.None || type == CommType.Usb)
                return;
            // set up session
            Session.SetUp(type);
            EventSession.SetUp(type);
        }
        /// <summary>
        /// Connect sessions
        /// </summary>
        /// <returns>result</returns>
        public void Connect()
        {
            // check state
            if (Session.State == ConnectionState.Connected)
                return;
            if (EventSession.Type == CommType.Ethernet && EventSession.State == ConnectionState.Connected)
                return;

            // try connect session
            Session.Connect(ToolInfo.Ip, ToolInfo.Port);
            // check event session type
            if (EventSession.Type == CommType.Ethernet)
                // try connect event session
                EventSession.Connect(ToolInfo.Ip, ToolInfo.Port + 1);
            // change state
            State = ConnectionState.Connecting;
        }
        /// <summary>
        /// Disconnect sessions
        /// </summary>
        public void Disconnect()
        {
            // check session state
            if (Session.State == ConnectionState.Connected)
                // close
                Session.Close();
            // check event session state
            if (EventSession.State == ConnectionState.Connected)
                // close
                EventSession.Close();
            // change state
            State = ConnectionState.Disconnecting;
        }

        /// <summary>
        /// Get tool parameter
        /// </summary>
        /// <param name="addr">address</param>
        /// <param name="count">count</param>
        /// <returns>result</returns>
        public bool GetParam(ushort addr, ushort count) =>
            State == ConnectionState.Connected && Session.GetParam(addr, count);
        /// <summary>
        /// Set tool parameter
        /// </summary>
        /// <param name="addr">address</param>
        /// <param name="value">value</param>
        /// <returns>result</returns>
        public bool SetParam(ushort addr, ushort value) =>
            State == ConnectionState.Connected && Session.SetParam(addr, value);
        /// <summary>
        /// Set tool real-time monitoring state
        /// </summary>
        /// <param name="addr">address</param>
        /// <param name="state">state</param>
        /// <returns>result</returns>
        public bool SetRealTime(ushort addr = 4002, ushort state = 1) =>
            State == ConnectionState.Connected && Session.SetRealTime(addr, state);
        /// <summary>
        /// Set tool graph monitoring state
        /// </summary>
        /// <param name="addr">address</param>
        /// <param name="state">state</param>
        /// <returns>result</returns>
        public bool SetGraph(ushort addr = 4100, ushort state = 1) =>
            State == ConnectionState.Connected && Session.SetGraph(addr, state);
        /// <summary>
        /// Get tool current state
        /// </summary>
        /// <param name="addr">address</param>
        /// <param name="count">count</param>
        /// <returns>result</returns>
        public bool GetState(ushort addr = 3300, ushort count = 14) =>
            State == ConnectionState.Connected && Session.GetState(addr, count);
        /// <summary>
        /// Set tool event monitoring state
        /// </summary>
        /// <param name="addr">address</param>
        /// <param name="state">state</param>
        /// <returns>result</returns>
        public bool SetEventMonitor(ushort addr = 4015, ushort state = 1) =>
            State == ConnectionState.Connected && Session.SetParam(addr, state);
        /// <summary>
        /// Acknowledge tool event monitoring
        /// </summary>
        /// <param name="addr">address</param>
        /// <param name="ack">acknowledge</param>
        /// <returns>result</returns>
        private bool AckEventMonitor(ushort addr = 4016, ushort ack = 1) =>
            State == ConnectionState.Connected && Session.SetParam(addr, ack);
        
        private void ChangedConnection(bool state)
        {
            // check state
            switch (state)
            {
                case true when 
                    Session.State == ConnectionState.Connected && 
                    (EventSession.Type != CommType.Ethernet || EventSession.State == ConnectionState.Connected):
                    // change state
                    State = ConnectionState.Connected;
                    // event
                    ConnectionChanged?.Invoke(ToolInfo, State);
                    break;
                case false when 
                    Session.State == ConnectionState.Disconnected &&
                    (EventSession.Type != CommType.Ethernet || EventSession.State == ConnectionState.Disconnected):
                    // change state
                    State = ConnectionState.Disconnected;
                    // event
                    ConnectionChanged?.Invoke(ToolInfo, State);
                    break;
            }
        }
        private void ReceivedMsg(Command cmd, int addr, int[] values)
        {
            // event
            SessionReceived?.Invoke(ToolInfo, cmd, addr, values);
        }
        private void ReceivedEventMsg(Command cmd, int addr, int[] values)
        {
            // event
            EventReceived?.Invoke(ToolInfo, cmd, addr, values);
            // check cmd
            if (cmd == Command.Mor)
                // acknowledge event monitor
                AckEventMonitor();
        }
    }
}