/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using System.Collections.Generic;

namespace ButtplugWebBridge.Models
{
    public class VibrationPattern
    {
        //dual motor example
        //Speeds[0] : 100,10, 100,0
        //Speeds[1] : 10, 100,10, 0
        //Time      : 100,500,100,1
        //Loop      : true/false
        public List<List<uint>> Speeds { get; set; }
        public List<uint> Time { get; set; }
        public bool Loop { get; set; }
        public uint MotorCount
        {
            get
            {
                return (uint)Speeds.Count;
            }
        }

        /// <summary>
        /// Checks that the VibrationPattern contains sane data (all lists are of equal length).
        /// </summary>
        public bool Validate()
        {
            if (Speeds == null || Time == null)
                return false;

            var expected_entries = Time.Count;
            foreach (List<uint> speed in Speeds)
            {
                if (speed.Count != expected_entries)
                    return false;
            }
            return true;
        }
    }
}
