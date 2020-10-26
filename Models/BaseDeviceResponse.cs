/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using Microsoft.AspNetCore.Http;

namespace ButtplugWebBridge.Models
{
    public class BaseDeviceResponse : BasedResponse
    {
        public string Device { get; }
        public BaseDeviceResponse(HttpRequest request, string device) : base(request)
        {
            Device = device;
        }
    }
}
