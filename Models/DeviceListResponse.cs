/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using Microsoft.AspNetCore.Http;

namespace ButtplugWebBridge.Models
{
    public class DeviceListResponse : BasedResponse
    {
        public string[] Devices { get; }
        public DeviceListResponse(HttpRequest request, string[] devicelist) : base(request)
        {
            Devices = devicelist;
        }
    }
}
