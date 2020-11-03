using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ButtplugWebBridge.Models
{
    public class DeviceInstruction
    {
        public uint delay { get; }
        public uint[] speeds { get; }
        public DeviceInstruction(uint[] speeds, uint delay)
        {
            this.speeds = speeds;
            this.delay = delay;
        }
    }
}
