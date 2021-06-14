using System;

namespace HCommAir.Tools
{
    /// <summary>
    /// HCommAir tool timeout class
    /// </summary>
    public class HcTimeout
    {
        private DateTime _checkTime = DateTime.Now;

        /// <summary>
        /// Timeout limit time (ms)
        /// </summary>
        public int Timeout { get; set; } = 10000;
        /// <summary>
        /// Set time now
        /// </summary>
        public void ResetTime()
        {
            _checkTime = DateTime.Now;
        }
        /// <summary>
        /// Check timeout
        /// </summary>
        /// <returns>timeout</returns>
        public bool CheckTime()
        {
            return (DateTime.Now - _checkTime).TotalMilliseconds > Timeout;
        }
    }
}