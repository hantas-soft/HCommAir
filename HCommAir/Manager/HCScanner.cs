using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using HCommAir.Tools;

namespace HCommAir.Manager
{
    /// <summary>
    ///     HCommAir Tool scanner
    /// </summary>
    public class HcScanner
    {
        /// <summary>
        ///     Tool searching delegate
        /// </summary>
        /// <param name="info">tool information</param>
        public delegate void MulticastEventHandler(HcToolInfo info);

        private const int McPort = 53256;
        private const int ScanPeriod = 1000;
        private const int TimeoutTime = 100;
        private readonly IPAddress _mcIpAddr = IPAddress.Parse("239.66.77.43");

        /// <summary>
        ///     Constructor
        /// </summary>
        public HcScanner()
        {
            try
            {
                // check port numbers
                for (var port = 50000; port < 60000; port++)
                {
                    // check port
                    if (!IsPortAvailable(port))
                        continue;
                    // client
                    Client = new UdpClient(new IPEndPoint(IPAddress.Any, port));
                    // check client
                    if (Client != null)
                        break;
                }

                // set scan timer
                ScanTimer = new Timer(ScanTimer_Tick, null, Timeout.Infinite, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"HCommAir_HCScanner_Constructor");
                Console.WriteLine($@"{ex.Message}");
            }
        }

        private UdpClient Client { get; }
        private Timer ScanTimer { get; }
        private int TransactionId { get; set; }
        private DateTime TransactionTime { get; set; }
        private List<HcToolInfo> SearchTools { get; } = new List<HcToolInfo>();

        /// <summary>
        ///     HCommAir tool scanning status
        /// </summary>
        public bool IsScanning { get; set; }

        /// <summary>
        ///     Tool searching event
        /// </summary>
        public event MulticastEventHandler ToolAttach, ToolDetach, ToolAlive;

        /// <summary>
        ///     HCommAir tool scanning start
        /// </summary>
        public void Start()
        {
            // check client
            if (Client == null)
                return;
            // clear searched tool list
            SearchTools.Clear();
            // join multicast group
            Client.JoinMulticastGroup(_mcIpAddr);
            // start scan timer
            ScanTimer.Change(0, ScanPeriod);
            // begin receive
            Client.BeginReceive(ClientReceived, Client);
            // set scan status
            IsScanning = true;
        }

        /// <summary>
        ///     HCommAir tool scanning stop
        /// </summary>
        public void Stop()
        {
            // stop scan timer
            ScanTimer.Change(Timeout.Infinite, Timeout.Infinite);
            // drop multicast group
            Client.DropMulticastGroup(_mcIpAddr);
            // set scan status
            IsScanning = false;
        }

        /// <summary>
        ///     HCommAir interface properties change
        /// </summary>
        /// <param name="p">interface properties</param>
        public void ChangeInterfaceProp(IPv4InterfaceProperties p)
        {
            // check socket option
            Client?.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface,
                IPAddress.HostToNetworkOrder(p.Index));
        }

        private void ScanTimer_Tick(object state)
        {
            // check transaction time
            if ((DateTime.Now - TransactionTime).TotalMilliseconds > 3000)
            {
                // update id
                TransactionId += 1;
                // create packet
                var packet = new byte[]
                    { (byte)((TransactionId >> 8) & 0xFF), (byte)(TransactionId & 0xFF), 0x00, 0x01 };

                // try catch
                try
                {
                    // debug
                    // Console.WriteLine($@"Send time: {DateTime.Now:hh:mm:ss:fff}");
                    // send packet
                    Client.Send(packet, packet.Length, new IPEndPoint(_mcIpAddr, McPort));
                    // reset transaction time
                    TransactionTime = DateTime.Now;
                }
                catch (Exception ex)
                {
                    // debug
                    Debug.WriteLine(ex.Message);
                }
            }

            // lock searching tool list
            if (!Monitor.TryEnter(SearchTools, TimeoutTime))
                return;
            try
            {
                // check timeout
                for (var i = 0; i < SearchTools.Count; i++)
                {
                    // check state update
                    if (!SearchTools[i].CheckTime())
                        continue;
                    // debug console
                    Console.WriteLine($@"[{DateTime.Now:hh:mm:ss:fff}] Detach Tool: {SearchTools[i].Ip}");
                    // detach
                    ToolDetach?.Invoke(SearchTools[i]);
                    // remove tool
                    SearchTools.Remove(SearchTools[i]);
                }
            }
            finally
            {
                // unlock
                Monitor.Exit(SearchTools);
            }
        }

        private void ClientReceived(IAsyncResult ar)
        {
            // end receive point
            var endPoint = new IPEndPoint(IPAddress.Any, McPort);
            // receive data
            var receive = Client.EndReceive(ar, ref endPoint);
            // begin receive
            Client.BeginReceive(ClientReceived, Client);
            // check length
            if (receive.Length < 4)
                return;
            // check header
            var id = (receive[0] << 8) | receive[1];
            var cmd = (ScanCommand)((receive[2] << 8) | receive[3]);

            // check scan acknowledge and id and length
            if (cmd != ScanCommand.ScanAck || id != TransactionId || receive.Length - 4 != HcToolInfo.Count)
                return;
            // set tool information
            var info = new HcToolInfo(receive.Skip(4));
            // check MD/MDTC
            if (info.ToolType == HcToolInfo.ToolModelType.None)
                return;
            // lock searched tool list
            if (!Monitor.TryEnter(SearchTools, TimeoutTime))
                return;
            try
            {
                // debug
                //Console.WriteLine($@"Acknowledge...");
                // find tool
                var tool = SearchTools.Find(x => x.Mac == info.Mac);
                // check find tool
                if (tool == null)
                {
                    // debug console
                    Console.WriteLine($@"[{DateTime.Now:hh:mm:ss:fff}] Attach Tool: {info.Ip}");
                    // add tool
                    SearchTools.Add(info);
                    // attached
                    ToolAttach?.Invoke(info);
                }
                else
                {
                    // change state
                    tool.SetValues(receive.Skip(4));
                    // refresh tool
                    tool.ResetTime();
                    // alive
                    ToolAlive?.Invoke(tool);
                }
            }
            finally
            {
                // unlock
                Monitor.Exit(SearchTools);
            }
        }

        private static bool IsPortAvailable(int port)
        {
            var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            // get active connections
            var tcpConnections = ipProperties.GetActiveTcpConnections();
            // check connections
            if (tcpConnections.Any(conInfo => conInfo.LocalEndPoint.Port == port))
                // used
                return false;
            // check TCP listening port
            var tcpEndPoint = ipProperties.GetActiveUdpListeners();
            // check connections
            if (tcpEndPoint.Any(conInfo => conInfo.Port == port))
                // used
                return false;
            // check UDP listening port
            var udpEndPoint = ipProperties.GetActiveUdpListeners();
            // check connections
            return udpEndPoint.All(conInfo => conInfo.Port != port);
        }

        private enum ScanCommand
        {
            Scan = 0x01,
            Search = 0x02,
            Advertise = 0x81,
            ScanAck = 0x82,
            SearchAck = 0x83
        }

        private enum CauseType
        {
            Connected = 0x01,
            Changed = 0x02,
            Sleep = 0x03,
            ReConnected = 0x04
        }
    }
}