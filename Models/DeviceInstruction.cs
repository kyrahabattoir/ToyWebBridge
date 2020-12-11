/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */

namespace ToyWebBridge.Models
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
