/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace ButtplugWebBridge.Models
{
    public class DeviceResponse : BaseDeviceResponse
    {
        public Dictionary<string, uint> Capabilities { get; }
        public DeviceResponse(HttpRequest request, string device, Dictionary<string, uint> capabilities) : base(request, device)
        {
            Capabilities = capabilities;
        }
    }
}
