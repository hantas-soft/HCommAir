using System;
using System.Net;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using HCommAir.Tools;

namespace HCommAir.Manager
{
    /// <summary>
    /// HCommAir Tool scanner
    /// </summary>
    public class HcScanner
    {
        private readonly TimeSpan _timeoutSpan = new TimeSpan(0, 0, 0, 0, 100);
        private readonly IPAddress _mcIpAddr = IPAddress.Parse("239.66.77.43");
        private const int McPort = 53256;
        private const int ScanPeriod = 1000;
        private UdpClient Client { get; }
        private Timer ScanTimer { get; }
        private int TransactionId { get; set; }
        private List<HcToolInfo> SearchTools { get; } = new List<HcToolInfo>();
        
        /// <summary>
        /// HCommAir tool scanning status
        /// </summary>
        public bool IsScanning { get; set; }
        /// <summary>
        /// Tool searching delegate
        /// </summary>
        /// <param name="info">tool information</param>
        public delegate void MulticastEventHandler(HcToolInfo info);
        /// <summary>
        /// Tool searching event
        /// </summary>
        public event MulticastEventHandler ToolAttach, ToolDetach, ToolAlive;
        
        /// <summary>
        /// Constructor
        /// </summary>
        public HcScanner()
        {
            Client = new UdpClient(new IPEndPoint(IPAddress.Any, McPort));
            ScanTimer = new Timer(ScanTimer_Tick, null, Timeout.Infinite, Timeout.Infinite);
        }
        /// <summary>
        /// HCommAir tool scanning start
        /// </summary>
        public void Start()
        {
            // clear searched tool list
            SearchTools.Clear();
            // join multicast group
            Client.JoinMulticastGroup(_mcIpAddr);
            // start scan timer
            ScanTimer.Change(0, ScanPeriod);
            // begin receive
            Client.BeginReceive(ClientReceived, null);
            // set scan status
            IsScanning = true;
        }
        /// <summary>
        /// HCommAir tool scanning stop
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
        /// HCommAir interface properties change
        /// </summary>
        /// <param name="p">interface properties</param>
        public void ChangeInterfaceProp(IPv4InterfaceProperties p)
        {
            Client.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface,
                IPAddress.HostToNetworkOrder(p.Index));
        }
        
        private void ScanTimer_Tick(object state)
        {
            // lock searching tool list
            if(!Monitor.TryEnter(SearchTools, _timeoutSpan))
                return;
            try
            {
                // update id
                TransactionId += 1;
                // create packet
                var packet = new byte[]
                    {(byte) ((TransactionId >> 8) & 0xFF), (byte) (TransactionId & 0xFF), 0x00, 0x01};
                // send packet
                Client.Send(packet, packet.Length, new IPEndPoint(_mcIpAddr, McPort));

                // check timeout
                for (var i = 0; i < SearchTools.Count; i++)
                {
                    // check state update
                    if (!SearchTools[i].CheckTime())
                        continue;
                    // debug console
                    Console.WriteLine($@"Detach Tool: {SearchTools[i].Ip}");
                    Console.WriteLine($@"Detach Tool: {SearchTools[i].Mac}");
                    // detach
                    ToolDetach?.Invoke(SearchTools[i]);
                    // remove tool
                    SearchTools.Remove(SearchTools[i]);
                }
            }
            catch (Exception e)
            {
                // debug
                Console.WriteLine($@"{e.Message}");
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
            var recv = Client.EndReceive(ar, ref endPoint);
            // begin receive
            Client.BeginReceive(ClientReceived, null);
            // check length
            if (recv.Length < 4)
                return;
            // check header
            var id = recv[0] << 8 | recv[1];
            var cmd = (ScanCommand) (recv[2] << 8 | recv[3]);
            // check scan acknowledge and id and length
            if (cmd != ScanCommand.ScanAck || id != TransactionId || recv.Length - 4 != HcToolInfo.Count)
                return;
            // set tool information
            var info = new HcToolInfo(recv.Skip(4));
            // check MD/MDTC
            if (info.ToolType == HcToolInfo.ToolModelType.None ||
                info.ToolType == HcToolInfo.ToolModelType.MD || info.ToolType == HcToolInfo.ToolModelType.MDT)
                return;
            // lock searched tool list
            if (!Monitor.TryEnter(SearchTools, _timeoutSpan))
                return;
            try
            {
                // find tool
                var tool = SearchTools.Find(x => x.Mac == info.Mac);
                // check find tool
                if (tool == null)
                {
                    // set timeout time
                    info.Timeout = ScanPeriod + 1000;
                    // debug console
                    Console.WriteLine($@"Attach Tool: {info.Ip}");
                    Console.WriteLine($@"Attach Tool: {info.Mac}");
                    // add tool
                    SearchTools.Add(info);
                    // attached
                    ToolAttach?.Invoke(info);
                }
                else
                {
                    // change values
                    tool.SetValues(info.GetValues());
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