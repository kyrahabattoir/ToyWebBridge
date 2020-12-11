/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using System.Collections.Generic;

namespace ToyWebBridge.Models
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

        public bool Validate()
        {
            //No timesteps? goodbye
            if (Time == null || Time.Count == 0)
                return false;

            //No speed sequences? goodbye
            if (Speeds == null || Speeds.Count == 0)
                return false;

            return true;
        }
    }
}
