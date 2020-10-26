/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using Microsoft.AspNetCore.Http;
using System;

namespace ButtplugWebBridge.Models
{
    public class BaseActionResponse : BaseDeviceResponse
    {
        public string DeviceAction { get; }
        public BaseActionResponse(HttpRequest request, string device, Type action) : base(request, device)
        {
            DeviceAction = action.Name;
        }
    }
}
