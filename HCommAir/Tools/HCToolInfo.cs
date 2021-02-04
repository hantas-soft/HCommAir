using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace HCommAir.Tools
{
    /// <summary>
    /// HCommAir tool information
    /// </summary>
    public class HcToolInfo : HcTimeout
    {
        private List<byte> Values { get; }
        /// <summary>
        /// Tool information value count
        /// </summary>
        public static int Count { get; } = 36;
        /// <summary>
        /// Constructor
        /// </summary>
        public HcToolInfo()
        {
            // create value list
            Values = new List<byte>();
            // add dummy
            for (var i = 0; i < Count; i++)
                Values.Add(0x00);
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="values">tool data</param>
        public HcToolInfo(IEnumerable<byte> values)
        {
            // add values
            Values = new List<byte>(values);
        }
        /// <summary>
        /// Get raw values
        /// </summary>
        /// <returns>values</returns>
        public byte[] GetValues()
        {
            return Values.ToArray();
        }
        /// <summary>
        /// Set raw values
        /// </summary>
        /// <param name="values">values</param>
        public void SetValues(byte[] values)
        {
            // clear values
            Values.Clear();
            // add values
            Values.AddRange(values);
        }

        /// <summary>
        /// Tool serial
        /// </summary>
        public string Serial => Encoding.Default.GetString(Values.Take(16).Where(x => x != '\0').ToArray());
        /// <summary>
        /// Tool model number
        /// </summary>
        public int Model => Values[16] << 24 | Values[17] << 16 | Values[18] << 8 | Values[19];
        /// <summary>
        /// Tool ip address
        /// </summary>
        public string Ip => $@"{Values[20]}.{Values[21]}.{Values[22]}.{Values[23]}";
        /// <summary>
        /// Tool port number
        /// </summary>
        public int Port => Values[24] << 8 | Values[25];
        /// <summary>
        /// Tool mac address
        /// </summary>
        public string Mac =>
            $@"{Values[26]:X2}:{Values[27]:X2}:{Values[28]:X2}:{Values[29]:X2}:{Values[30]:X2}:{Values[31]:X2}";
        /// <summary>
        /// Tool available connection check
        /// </summary>
        public bool AvailableConnection => Values[32] == 0;
        /// <summary>
        /// Tool firmware version
        /// </summary>
        public string Version => $@"{Values[33] << 8 | Values[34]:D4}";
        /// <summary>
        /// Tool model type 
        /// </summary>
        public ToolModelType ToolType => (ToolModelType) Values[35];
        /// <summary>
        /// HComm tool model type
        /// </summary>
        public enum ToolModelType
        {
            Mdc, Bm, Bmt, Mdtc
        }
    }
}