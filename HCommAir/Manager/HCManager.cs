using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using HCommAir.Tools;
using System.Linq;

namespace HCommAir.Manager
{
    /// <summary>
    /// HCommAir tool managing class
    /// </summary>
    public class HcManager
    {
        private const int Timeout = 500;

        private HcScanner Scanner { get; } = new HcScanner();
        private List<HcToolInfo> ScannedTools { get; } = new List<HcToolInfo>();
        private List<HcToolInfo> RegisteredTools { get; } = new List<HcToolInfo>();
        private List<HcToolInfo> SubRegisteredTools { get; } = new List<HcToolInfo>();
        private HcManagerHeader ManagerHeader { get; } = new HcManagerHeader();

        /// <summary>
        /// Tool registered event delegate
        /// </summary>
        /// <param name="info">tool information</param>
        public delegate void RegisterToolHandler(HcToolInfo info);
        /// <summary>
        /// Tool registered/unregistered event
        /// </summary>
        public event RegisterToolHandler ToolAdded, ToolRemoved, ToolConnect, ToolDisconnect, ToolAlive;
        /// <summary>
        /// Tool sub tools used state
        /// </summary>
        public bool UseSubTools
        {
            get => ManagerHeader.UseSubRegister;
            set => ManagerHeader.UseSubRegister = value;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public HcManager()
        {
            // set scanner event
            Scanner.ToolDetach += ScannerOnToolDetach;
            Scanner.ToolAlive += ScannerOnToolAlive;
            Scanner.ToolAttach += ScannerOnToolAttach;
        }
        /// <summary>
        /// Tool register
        /// </summary>
        /// <param name="info">Tool information</param>
        /// <returns>result</returns>
        public bool RegisterTool(HcToolInfo info)
        {
            var lockTakenScan = false;
            var lockTakenRegister = false;
            var lockTakenSubRegister = false;
            try
            {
                // lock
                Monitor.TryEnter(ScannedTools, Timeout, ref lockTakenScan);
                Monitor.TryEnter(RegisteredTools, Timeout, ref lockTakenRegister);
                Monitor.TryEnter(SubRegisteredTools, Timeout, ref lockTakenSubRegister);
                // check lock taken
                if (!lockTakenScan || !lockTakenRegister || !lockTakenSubRegister)
                    return false;
               
                // get register tool
                if (FindToolInfo(RegisteredTools, info.Mac) != null ||
                    FindToolInfo(SubRegisteredTools, info.Mac) != null)
                    return false;
                // remove scan tool
                ScannedTools.Remove(FindToolInfo(ScannedTools, info.Mac));
                // add register tool
                RegisteredTools.Add(info);
                // change register count
                ManagerHeader.RegisterCount = RegisteredTools.Count;
                // tool added event
                ToolAdded?.Invoke(info);
                
                return true;
            }
            finally
            {
                // unlock
                if (lockTakenScan)
                    Monitor.Exit(ScannedTools);
                if (lockTakenRegister)
                    Monitor.Exit(RegisteredTools);
                if (lockTakenSubRegister)
                    Monitor.Exit(SubRegisteredTools);
            }
        }
        /// <summary>
        /// Tool unRegister
        /// </summary>
        /// <param name="info">Tool information</param>
        /// <returns>result</returns>
        public bool UnRegisterTool(HcToolInfo info)
        {
            var lockTakenScan = false;
            var lockTakenRegister = false;
            try
            {
                // lock
                Monitor.TryEnter(ScannedTools, Timeout, ref lockTakenScan);
                Monitor.TryEnter(RegisteredTools, Timeout, ref lockTakenRegister);
                // check lock taken
                if (!lockTakenScan || !lockTakenRegister)
                    return false;
               
                // get register tool
                var register = FindToolInfo(RegisteredTools, info.Mac); 
                if (register == null)
                    return false;
                // remove register tool
                RegisteredTools.Remove(register);
                // change register count
                ManagerHeader.RegisterCount = RegisteredTools.Count;
                // tool removed event
                ToolRemoved?.Invoke(info);
                
                return true;
            }
            finally
            {
                // unlock
                if (lockTakenScan)
                    Monitor.Exit(ScannedTools);
                if (lockTakenRegister)
                    Monitor.Exit(RegisteredTools);
            }
        }
        /// <summary>
        /// Sub-Tool register
        /// </summary>
        /// <param name="info">Tool information</param>
        /// <returns>result</returns>
        public bool SubRegisterTool(HcToolInfo info)
        {
            var lockTakenScan = false;
            var lockTakenRegister = false;
            var lockTakenSubRegister = false;
            try
            {
                // lock
                Monitor.TryEnter(ScannedTools, Timeout, ref lockTakenScan);
                Monitor.TryEnter(RegisteredTools, Timeout, ref lockTakenRegister);
                Monitor.TryEnter(SubRegisteredTools, Timeout, ref lockTakenSubRegister);
                // check lock taken
                if (!lockTakenScan || !lockTakenRegister || !lockTakenSubRegister)
                    return false;
               
                // get register tool
                if (FindToolInfo(RegisteredTools, info.Mac) != null ||
                    FindToolInfo(SubRegisteredTools, info.Mac) != null)
                    return false;
                // remove scan tool
                ScannedTools.Remove(FindToolInfo(ScannedTools, info.Mac));
                // add register tool
                SubRegisteredTools.Add(info);
                // change register count
                ManagerHeader.SubRegisterCount = SubRegisteredTools.Count;
                // check use sub tools
                if (ManagerHeader.UseSubRegister)
                    // tool added event
                    ToolAdded?.Invoke(info);
                
                return true;
            }
            finally
            {
                // unlock
                if (lockTakenScan)
                    Monitor.Exit(ScannedTools);
                if (lockTakenRegister)
                    Monitor.Exit(RegisteredTools);
                if (lockTakenSubRegister)
                    Monitor.Exit(SubRegisteredTools);
            }
        }
        /// <summary>
        /// Sub-Tool unRegister
        /// </summary>
        /// <param name="info">Tool information</param>
        /// <returns>result</returns>
        public bool SubUnRegisterTool(HcToolInfo info)
        {
            var lockTakenScan = false;
            var lockTakenSubRegister = false;
            try
            {
                // lock
                Monitor.TryEnter(ScannedTools, Timeout, ref lockTakenScan);
                Monitor.TryEnter(SubRegisteredTools, Timeout, ref lockTakenSubRegister);
                // check lock taken
                if (!lockTakenScan || !lockTakenSubRegister)
                    return false;
               
                // get register tool
                var register = FindToolInfo(SubRegisteredTools, info.Mac); 
                if (register == null)
                    return false;
                // remove register tool
                SubRegisteredTools.Remove(register);
                // change register count
                ManagerHeader.SubRegisterCount = SubRegisteredTools.Count;
                // tool removed event
                ToolRemoved?.Invoke(info);
                
                return true;
            }
            finally
            {
                // unlock
                if (lockTakenScan)
                    Monitor.Exit(ScannedTools);
                if (lockTakenSubRegister)
                    Monitor.Exit(SubRegisteredTools);
            }
        }
        /// <summary>
        /// Registered tools save binary file
        /// </summary>
        /// <param name="path">file path</param>
        /// <returns>result</returns>
        public bool SaveRegisterTools(string path)
        {
            var lockTakenRegister = false;
            var lockTakenSubRegister = false;
            try
            {
                // lock
                Monitor.TryEnter(RegisteredTools, Timeout, ref lockTakenRegister);
                Monitor.TryEnter(SubRegisteredTools, Timeout, ref lockTakenSubRegister);
                // check lock taken
                if (!lockTakenRegister || !lockTakenSubRegister)
                    return false;

                // get directory
                var dir = Path.GetDirectoryName(path);
                // check directory
                if (string.IsNullOrWhiteSpace(dir))
                    return false;
                if (!Directory.Exists(dir))
                    // create directory
                    Directory.CreateDirectory(dir);

                // file stream
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    // binary writer
                    using (var bw = new BinaryWriter(fs))
                    {
                        // write header
                        bw.Write(ManagerHeader.GetValues());
                        // get register tools
                        foreach (var tool in RegisteredTools)
                            // write binary
                            bw.Write(tool.GetValues());
                        // get sub-register tools
                        foreach (var tool in SubRegisteredTools)
                            // write binary
                            bw.Write(tool.GetValues());
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                // debug
                Console.WriteLine($@"{e.Message}");
                // result
                return false;
            }
            finally
            {
                // unlock
                if (lockTakenRegister)
                    Monitor.Exit(RegisteredTools);
                if (lockTakenSubRegister)
                    Monitor.Exit(SubRegisteredTools);
            }
        }
        /// <summary>
        /// Registered tools load binary file
        /// </summary>
        /// <param name="path">file path</param>
        /// <returns>result</returns>
        public bool LoadRegisterTools(string path)
        {
            var lockTakenScan = false;
            var lockTakenRegister = false;
            var lockTakenSubRegister = false;
            try
            {
                // lock
                Monitor.TryEnter(ScannedTools, Timeout, ref lockTakenScan);
                Monitor.TryEnter(RegisteredTools, Timeout, ref lockTakenRegister);
                Monitor.TryEnter(SubRegisteredTools, Timeout, ref lockTakenSubRegister);
                // check lock taken
                if (!lockTakenScan || !lockTakenRegister || !lockTakenSubRegister)
                    return false;

                // file stream
                using (var fs = new FileStream(path, FileMode.Open))
                {
                    // binary writer
                    using (var br = new BinaryReader(fs))
                    {
                        // check header size
                        if (br.BaseStream.Length < HcManagerHeader.Count)
                            return false;
                        // read header
                        ManagerHeader.SetValues(br.ReadBytes(HcManagerHeader.Count));
                        // get total size
                        var totalSize =
                            (ManagerHeader.RegisterCount + ManagerHeader.SubRegisterCount) * HcToolInfo.Count +
                            HcManagerHeader.Count;
                        // check total size
                        if (br.BaseStream.Length != totalSize)
                            return false;
                        // check registered tool count
                        for (var i = 0; i < ManagerHeader.RegisterCount; i++)
                        {
                            // get tool information
                            var tool = new HcToolInfo(br.ReadBytes(HcToolInfo.Count));
                            // find scan tool
                            var scan = FindToolInfo(ScannedTools, tool.Mac);
                            // check scan tool
                            if (scan != null)
                                // remove scan tool
                                ScannedTools.Remove(scan);
                            // find register tool
                            if (FindToolInfo(RegisteredTools, tool.Mac) != null)
                                continue;
                            // register tool
                            RegisteredTools.Add(tool);
                        }
                        // check sub-registered tool count
                        for (var i = 0; i < ManagerHeader.SubRegisterCount; i++)
                        {
                            // get tool information
                            var tool = new HcToolInfo(br.ReadBytes(HcToolInfo.Count));
                            // find scan tool
                            var scan = FindToolInfo(ScannedTools, tool.Mac);
                            // check scan tool
                            if (scan != null)
                                // remove scan tool
                                ScannedTools.Remove(scan);
                            // find sub-register tool
                            if (FindToolInfo(SubRegisteredTools, tool.Mac) != null)
                                continue;
                            // register tool
                            SubRegisteredTools.Add(tool);
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                // debug
                Console.WriteLine($@"{e.Message}");
                // result
                return false;
            }
            finally
            {
                // unlock
                if (lockTakenScan)
                    Monitor.Exit(ScannedTools);
                if (lockTakenRegister)
                    Monitor.Exit(RegisteredTools);
                if (lockTakenSubRegister)
                    Monitor.Exit(SubRegisteredTools);
            }
        }

        /// <summary>
        /// Multicast scanning start
        /// </summary>
        public void Start() => Scanner.Start();
        /// <summary>
        /// Multicast scanning stop
        /// </summary>
        public void Stop() => Scanner.Stop();
        /// <summary>
        /// Multicast scanning interface properties change
        /// </summary>
        /// <param name="p"></param>
        public void ChangeInterfaceProp(IPv4InterfaceProperties p) => Scanner.ChangeInterfaceProp(p);
        /// <summary>
        /// Get registered tools list
        /// </summary>
        /// <returns>tool list</returns>
        public List<HcToolInfo> GetRegisteredTools()
        {
            var lockTakenRegister = false;
            try
            {
                // lock
                Monitor.TryEnter(RegisteredTools, Timeout, ref lockTakenRegister);
                // get registered tools
                return !lockTakenRegister ? null : RegisteredTools;
            }
            catch (Exception e)
            {
                // debug
                Console.WriteLine($@"{e.Message}");
                // result
                return null;
            }
            finally
            {
                // unlock
                if (lockTakenRegister)
                    Monitor.Exit(RegisteredTools);
            }
        }
        /// <summary>
        /// Get sub-registered tools list
        /// </summary>
        /// <returns>tool list</returns>
        public List<HcToolInfo> GetSubRegisteredTools()
        {
            var lockTakenSubRegister = false;
            try
            {
                // lock
                Monitor.TryEnter(SubRegisteredTools, Timeout, ref lockTakenSubRegister);
                // get registered tools
                return !lockTakenSubRegister ? null : SubRegisteredTools;
            }
            catch (Exception e)
            {
                // debug
                Console.WriteLine($@"{e.Message}");
                // result
                return null;
            }
            finally
            {
                // unlock
                if (lockTakenSubRegister)
                    Monitor.Exit(SubRegisteredTools);
            }
        }
        /// <summary>
        /// Get scanned tools list
        /// </summary>
        /// <returns>tool list</returns>
        public List<HcToolInfo> GetScannedTools()
        {
            var lockTakenScan = false;
            try
            {
                // lock
                Monitor.TryEnter(ScannedTools, Timeout, ref lockTakenScan);
                // get registered tools
                return !lockTakenScan ? null : ScannedTools;
            }
            catch (Exception e)
            {
                // debug
                Console.WriteLine($@"{e.Message}");
                // result
                return null;
            }
            finally
            {
                // unlock
                if (lockTakenScan)
                    Monitor.Exit(ScannedTools);
            }
        }
        /// <summary>
        /// Get all network interface
        /// </summary>
        /// <returns>interface list</returns>
        public static List<NetworkInterface> GetAllInterfaces() =>
            (from item in NetworkInterface.GetAllNetworkInterfaces()
                let ipProp = item.GetIPProperties()
                where item.GetIPProperties().MulticastAddresses.Any()
                where item.SupportsMulticast
                where OperationalStatus.Up == item.OperationalStatus
                where item.GetIPProperties().GetIPv4Properties() != null
                select item).ToList();

        private void ScannerOnToolAttach(HcToolInfo info)
        {
            var lockTakenScan = false;
            var lockTakenRegister = false;
            var lockTakenSubRegister = false;
            try
            {
                // lock
                Monitor.TryEnter(ScannedTools, Timeout, ref lockTakenScan);
                Monitor.TryEnter(RegisteredTools, Timeout, ref lockTakenRegister);
                Monitor.TryEnter(SubRegisteredTools, Timeout, ref lockTakenSubRegister);
                // check lock taken
                if (!lockTakenScan || !lockTakenRegister || !lockTakenSubRegister)
                    return;
               
                // get tools
                var scan = FindToolInfo(ScannedTools, info.Mac);
                var register = FindToolInfo(RegisteredTools, info.Mac);
                var subRegister = FindToolInfo(SubRegisteredTools, info.Mac);
                // check scan tools
                if (scan == null && register == null && subRegister == null)
                    // add scanned tool
                    ScannedTools.Add(info);
                else if (register != null && subRegister == null)
                {
                    // set values
                    register.SetValues(info.GetValues());
                    // tool connect event
                    ToolConnect?.Invoke(register);
                }
                else if (register == null && ManagerHeader.UseSubRegister && subRegister != null)
                {
                    // set values
                    subRegister.SetValues(info.GetValues());
                    // tool connect event
                    ToolConnect?.Invoke(subRegister);
                }
            }
            finally
            {
                // unlock
                if (lockTakenScan)
                    Monitor.Exit(ScannedTools);
                if (lockTakenRegister)
                    Monitor.Exit(RegisteredTools);
                if (lockTakenSubRegister)
                    Monitor.Exit(SubRegisteredTools);
            }
        }
        private void ScannerOnToolDetach(HcToolInfo info)
        {
            var lockTakenScan = false;
            var lockTakenRegister = false;
            var lockTakenSubRegister = false;
            try
            {
                // lock
                Monitor.TryEnter(ScannedTools, Timeout, ref lockTakenScan);
                Monitor.TryEnter(RegisteredTools, Timeout, ref lockTakenRegister);
                Monitor.TryEnter(SubRegisteredTools, Timeout, ref lockTakenSubRegister);
                // check lock taken
                if (!lockTakenScan || !lockTakenRegister || !lockTakenSubRegister)
                    return;
               
                // get tools
                var scan = FindToolInfo(ScannedTools, info.Mac);
                var register = FindToolInfo(RegisteredTools, info.Mac);
                var subRegister = FindToolInfo(SubRegisteredTools, info.Mac);
                // check scan tools
                if (scan != null)
                    // remove scan tool
                    ScannedTools.Remove(scan);
                else if (register != null && subRegister == null)
                    // tool disconnect event
                    ToolDisconnect?.Invoke(info);
                else if (register == null && ManagerHeader.UseSubRegister && subRegister != null)
                    // tool disconnect event
                    ToolDisconnect?.Invoke(info);
            }
            finally
            {
                // unlock
                if (lockTakenScan)
                    Monitor.Exit(ScannedTools);
                if (lockTakenRegister)
                    Monitor.Exit(RegisteredTools);
                if (lockTakenSubRegister)
                    Monitor.Exit(SubRegisteredTools);
            }
        }
        private void ScannerOnToolAlive(HcToolInfo info)
        {
            var lockTakenScan = false;
            var lockTakenRegister = false;
            var lockTakenSubRegister = false;
            try
            {
                // lock
                Monitor.TryEnter(ScannedTools, Timeout, ref lockTakenScan);
                Monitor.TryEnter(RegisteredTools, Timeout, ref lockTakenRegister);
                Monitor.TryEnter(SubRegisteredTools, Timeout, ref lockTakenSubRegister);
                // check lock taken
                if (!lockTakenScan || !lockTakenRegister || !lockTakenSubRegister)
                    return;
               
                // get tools
                var scan = FindToolInfo(ScannedTools, info.Mac);
                var register = FindToolInfo(RegisteredTools, info.Mac);
                var subRegister = FindToolInfo(SubRegisteredTools, info.Mac);
                // check scan tool
                if (scan != null)
                    return;
                // check registered tools
                if (register == null && subRegister == null)
                    // add scanned tool
                    ScannedTools.Add(info);
                if (register != null && subRegister == null)
                {
                    // set values
                    register.SetValues(info.GetValues());
                    // tool connect event
                    ToolAlive?.Invoke(register);
                }
                else if (register == null && ManagerHeader.UseSubRegister && subRegister != null)
                {
                    // set values
                    subRegister.SetValues(info.GetValues());
                    // tool connect event
                    ToolAlive?.Invoke(subRegister);
                }
            }
            finally
            {
                // unlock
                if (lockTakenScan)
                    Monitor.Exit(ScannedTools);
                if (lockTakenRegister)
                    Monitor.Exit(RegisteredTools);
                if (lockTakenSubRegister)
                    Monitor.Exit(SubRegisteredTools);
            }
        }
        private static HcToolInfo FindToolInfo(List<HcToolInfo> list, string mac) 
            => list.Find(x => x.Mac == mac);

        private class HcManagerHeader
        {
            private List<byte> Values { get; } = new List<byte>();

            /// <summary>
            /// Header size
            /// </summary>
            public static int Count { get; } = 7;
            /// <summary>
            /// Constructor
            /// </summary>
            public HcManagerHeader()
            {
                // add dummy
                for (var i = 0; i < Count; i++)
                    Values.Add(0);
            }
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="values">values</param>
            public HcManagerHeader(IEnumerable<byte> values)
            {
                // add values
                Values.AddRange(values);
            }
            /// <summary>
            /// Tool management binary file version
            /// </summary>
            public int Version
            {
                get => Values[0] << 8 | Values[1];
                set
                {
                    Values[0] = (byte) ((value >> 8) & 0xFF);
                    Values[1] = (byte) (value & 0xFF);
                }
            }
            public bool UseSubRegister
            {
                get => Values[2] > 0;
                set => Values[2] = value ? (byte) 0x01 : (byte) 0x00;
            }
            /// <summary>
            /// Tool management register tool count
            /// </summary>
            public int RegisterCount
            {
                get => Values[3] << 8 | Values[4];
                set
                {
                    Values[3] = (byte) ((value >> 8) & 0xFF);
                    Values[4] = (byte) (value & 0xFF);
                }
            }
            /// <summary>
            /// Tool management sub register tool count
            /// </summary>
            public int SubRegisterCount
            {
                get => Values[5] << 8 | Values[6];
                set
                {
                    Values[5] = (byte) ((value >> 8) & 0xFF);
                    Values[6] = (byte) (value & 0xFF);
                }
            }
            /// <summary>
            /// Get tool management values 
            /// </summary>
            /// <returns>values</returns>
            public byte[] GetValues() => Values.ToArray();
            /// <summary>
            /// Set tool management values
            /// </summary>
            /// <param name="values">values</param>
            public void SetValues(IEnumerable<byte> values)
            {
                // clear values
                Values.Clear();
                // add values
                Values.AddRange(values);
            }
        }
    }
}