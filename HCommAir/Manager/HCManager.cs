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
            try
            {
                // lock
                Monitor.TryEnter(ScannedTools, Timeout, ref lockTakenScan);
                Monitor.TryEnter(RegisteredTools, Timeout, ref lockTakenRegister);
                // check lock taken
                if (!lockTakenScan || !lockTakenRegister)
                    return false;
               
                // get register tool
                if (FindToolInfo(RegisteredTools, info.Mac) != null)
                    return false;
                // remove scan tool
                ScannedTools.Remove(FindToolInfo(ScannedTools, info.Mac));
                // add register tool
                RegisteredTools.Add(info);
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
        /// Registered tools save binary file
        /// </summary>
        /// <param name="path">file path</param>
        /// <returns>result</returns>
        public bool SaveRegisterTools(string path)
        {
            var lockTakenRegister = false;
            try
            {
                // lock
                Monitor.TryEnter(RegisteredTools, Timeout, ref lockTakenRegister);
                // check lock taken
                if (!lockTakenRegister)
                    return false;

                // get directory
                var dir = Path.GetDirectoryName(path);
                // check directory
                if (dir != null && !Directory.Exists(dir))
                    // create directory
                    Directory.CreateDirectory(dir);

                // file stream
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    // binary writer
                    using (var bw = new BinaryWriter(fs))
                    {
                        // get register tools
                        foreach (var tool in RegisteredTools)
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
            try
            {
                // lock
                Monitor.TryEnter(ScannedTools, Timeout, ref lockTakenScan);
                Monitor.TryEnter(RegisteredTools, Timeout, ref lockTakenRegister);
                // check lock taken
                if (!lockTakenRegister)
                    return false;

                // file stream
                using (var fs = new FileStream(path, FileMode.Open))
                {
                    // binary writer
                    using (var br = new BinaryReader(fs))
                    {
                        do
                        {
                            // get tool information
                            var tool = new HcToolInfo(br.ReadBytes(HcToolInfo.Count));
                            // find scan tool
                            var scan = FindToolInfo(ScannedTools, tool.Mac);
                            // check scan tool
                            if (scan != null)
                                // remove
                                ScannedTools.Remove(scan);
                            // find register tool
                            if (FindToolInfo(RegisteredTools, tool.Mac) != null)
                                continue;
                            // register tool
                            RegisteredTools.Add(tool);

                            // check file length
                        } while (br.BaseStream.Position < br.BaseStream.Length);
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
                where item.GetIPProperties().MulticastAddresses.Any()
                where item.SupportsMulticast
                where OperationalStatus.Up == item.OperationalStatus
                where item.GetIPProperties().GetIPv4Properties() != null
                select item).ToList();

        private void ScannerOnToolAttach(HcToolInfo info)
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
                    return;
               
                // get tools
                var register = FindToolInfo(RegisteredTools, info.Mac);
                var scan = FindToolInfo(ScannedTools, info.Mac);
                // check scan tool
                switch (scan)
                {
                    case null when register == null:
                        // add scan tool
                        ScannedTools.Add(info);
                        break;
                    case null:
                        // set values
                        register.SetValues(info.GetValues());
                        // tool connect event
                        ToolConnect?.Invoke(register);
                        break;
                }
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
        private void ScannerOnToolDetach(HcToolInfo info)
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
                    return;
               
                // get tools
                var register = FindToolInfo(RegisteredTools, info.Mac);
                var scan = FindToolInfo(ScannedTools, info.Mac);
                // check scan/register tool
                if (scan != null && register == null)
                    // remove scan tool
                    ScannedTools.Remove(scan);
                else if (scan == null && register != null)
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
            }
        }
        private void ScannerOnToolAlive(HcToolInfo info)
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
                    return;
               
                // get tools
                var register = FindToolInfo(RegisteredTools, info.Mac);
                var scan = FindToolInfo(ScannedTools, info.Mac);
                // check scan/register tool
                if (scan == null && register == null)
                    // add scan tool
                    ScannedTools.Add(info);
                else if (register != null)
                    // event
                    ToolAlive?.Invoke(info);
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
        private static HcToolInfo FindToolInfo(List<HcToolInfo> list, string mac) 
            => list.Find(x => x.Mac == mac);
    }
}