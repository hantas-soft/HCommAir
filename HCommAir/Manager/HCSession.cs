using System;
using System.Linq;
using HComm;
using HComm.Common;
using HCommAir.Tools;

namespace HCommAir.Manager
{
    /// <summary>
    ///     HCommAir tool session class
    /// </summary>
    public class HcSession
    {
        /// <summary>
        ///     Connection changed event handler delegate
        /// </summary>
        /// <param name="info">tool information</param>
        /// <param name="state">connection state</param>
        public delegate void ConnectionHandler(HcToolInfo info, ConnectionState state);

        /// <summary>
        ///     Received event handler delegate
        /// </summary>
        /// <param name="info">tool information</param>
        /// <param name="cmd">command</param>
        /// <param name="addr">address</param>
        /// <param name="values">values</param>
        public delegate void ReceivedHandler(HcToolInfo info, Command cmd, int addr, int[] values);

        /// <summary>
        ///     Received monitor event handler delegate
        /// </summary>
        public delegate void ReceivedMorHandler(HcToolInfo info, MonitorCommand cmd, byte[] values);

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="info">Tool information</param>
        public HcSession(HcToolInfo info)
        {
            // set tool information
            ToolInfo = info;
            // sessions
            Session = new HCommInterface();
            // EventSession = new HCommInterface();
            // reset state
            State = ConnectionState.Disconnected;
            // set event
            Session.ReceivedMsg = ReceivedMsg;
            Session.ChangedConnection = ChangedConnection;
            Session.ReceivedMorMsg = ReceivedMorMsg;
        }

        private HCommInterface Session { get; }
        // private HCommInterface EventSession { get; }

        /// <summary>
        ///     Tool information
        /// </summary>
        public HcToolInfo ToolInfo { get; }

        /// <summary>
        ///     Connection state
        /// </summary>
        public ConnectionState State { get; private set; }

        /// <summary>
        ///     Session max queue size
        /// </summary>
        public int MaxQueueSize
        {
            get => Session.MaxQueueSize;
            set => Session.MaxQueueSize = value;
        }

        /// <summary>
        ///     Session max block size
        /// </summary>
        public int MaxBlockSize
        {
            get => Session.MaxParamBlock;
            set => Session.MaxParamBlock = value;
        }

        /// <summary>
        ///     Session queue count
        /// </summary>
        public int QueueCount => Session.QueueCount;

        /// <summary>
        ///     Session device information
        /// </summary>
        public HCommInterface.DeviceInfo DeviceInfo => Session?.Information;

        /// <summary>
        ///     Session connection type
        /// </summary>
        public CommType SessionType { get; set; }

        /// <summary>
        ///     Connection changed event
        /// </summary>
        public event ConnectionHandler ConnectionChanged;

        /// <summary>
        ///     Received event
        /// </summary>
        public event ReceivedHandler SessionReceived;

        /// <summary>
        ///     Received monitor event
        /// </summary>
        public event ReceivedMorHandler EventReceived;

        /// <summary>
        ///     SetUp session
        /// </summary>
        /// <param name="type">Session type</param>
        public void SetUp(CommType type)
        {
            // check type
            if (type == CommType.None)
                return;
            // set type
            SessionType = type;
            // set up session
            Session.SetUp(SessionType);
        }

        /// <summary>
        ///     Connect sessions
        /// </summary>
        /// <returns>result</returns>
        public void Connect()
        {
            // check state
            if (Session.State == ConnectionState.Connected)
                return;

            var target = string.Empty;
            var option = 0;
            var id = ToolInfo.GetValues()[34];
            // check type and get target
            switch (Session.Type)
            {
                case CommType.Serial:
                    var ip = ToolInfo.Ip.Split('.');
                    // set target
                    target = $@"COM{(Convert.ToInt32(ip[2]) << 8) | Convert.ToInt32(ip[3])}";
                    // get baudrate values
                    var baud = ToolInfo.GetValues().Skip(26).Take(4).ToArray();
                    // set baudrate
                    option = (baud[0] << 24) | (baud[1] << 16) | (baud[2] << 8) | baud[3];
                    break;
                case CommType.Ethernet:
                    // set target
                    target = ToolInfo.Ip;
                    // set port
                    option = ToolInfo.Port;
                    // check serial
                    if (!string.IsNullOrWhiteSpace(ToolInfo.Serial))
                        // set id
                        id = 1;
                    break;
                case CommType.Usb:
                    // set target
                    target = @"USB";
                    // set option
                    option = 0x55;
                    break;
                case CommType.None:
                    break;
                default:
                    return;
            }

            // try connect session
            Session.Connect(target, option, id);

            // change state
            State = ConnectionState.Connecting;
        }

        /// <summary>
        ///     Disconnect sessions
        /// </summary>
        public void Disconnect()
        {
            // check session state
            if (Session.State == ConnectionState.Connected)
                // close
                Session.Close();

            // change state
            State = ConnectionState.Disconnecting;
        }

        /// <summary>
        ///     Get tool parameter
        /// </summary>
        /// <param name="addr">address</param>
        /// <param name="count">count</param>
        /// <param name="merge">merge state</param>
        /// <returns>result</returns>
        public bool GetParam(ushort addr, ushort count, bool merge = false)
        {
            return State == ConnectionState.Connected && Session.GetParam(addr, count, merge);
        }

        /// <summary>
        ///     Set tool parameter
        /// </summary>
        /// <param name="addr">address</param>
        /// <param name="value">value</param>
        /// <returns>result</returns>
        public bool SetParam(ushort addr, ushort value)
        {
            return State == ConnectionState.Connected && Session.SetParam(addr, value);
        }

        /// <summary>
        ///     Get tool information
        /// </summary>
        /// <returns>result</returns>
        public bool GetInfo()
        {
            return State == ConnectionState.Connected && Session.GetInfo();
        }

        /// <summary>
        ///     Set tool real-time monitoring state
        /// </summary>
        /// <param name="addr">address</param>
        /// <param name="state">state</param>
        /// <returns>result</returns>
        public bool SetRealTime(ushort addr = 4002, ushort state = 1)
        {
            return State == ConnectionState.Connected && Session.SetRealTime(addr, state);
        }

        /// <summary>
        ///     Set tool graph monitoring state
        /// </summary>
        /// <param name="addr">address</param>
        /// <param name="state">state</param>
        /// <returns>result</returns>
        public bool SetGraph(ushort addr = 4100, ushort state = 1)
        {
            return State == ConnectionState.Connected && Session.SetGraph(addr, state);
        }

        /// <summary>
        ///     Get tool graph monitoring state
        /// </summary>
        /// <param name="addr">address</param>
        /// <param name="state">state</param>
        /// <returns>result</returns>
        public bool GetGraph(ushort addr = 4200, ushort state = 1)
        {
            return State == ConnectionState.Connected && Session.GetGraph(addr, state);
        }

        /// <summary>
        ///     Get tool current state
        /// </summary>
        /// <param name="addr">address</param>
        /// <param name="count">count</param>
        /// <returns>result</returns>
        public bool GetState(ushort addr = 3300, ushort count = 14)
        {
            return State == ConnectionState.Connected && Session.GetState(addr, count);
        }

        /// <summary>
        ///     Set tool event monitoring state
        /// </summary>
        /// <param name="addr">address</param>
        /// <param name="state">state</param>
        /// <returns>result</returns>
        public bool SetEventMonitor(ushort addr = 4015, ushort state = 1)
        {
            return State == ConnectionState.Connected && Session.SetParam(addr, state);
        }

        private void ChangedConnection(bool state)
        {
            // check state
            switch (state)
            {
                case true when
                    Session.State == ConnectionState.Connected:
                    // change state
                    State = ConnectionState.Connected;
                    // event
                    ConnectionChanged?.Invoke(ToolInfo, State);
                    break;
                case false when
                    Session.State == ConnectionState.Disconnected:
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

        private void ReceivedMorMsg(MonitorCommand cmd, byte[] packet)
        {
            // event
            EventReceived?.Invoke(ToolInfo, cmd, packet);
        }
    }
}