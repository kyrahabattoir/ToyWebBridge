/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using System.Collections.Generic;

namespace ButtplugWebBridge.Models
{
    public class DeviceResponse : BaseDeviceResponse
    {
        public Dictionary<string, uint> Features { get; }
        public DeviceResponse(string action, string device, Dictionary<string, uint> features) : base(action, device)
        {
            Features = features;
        }
    }
}
