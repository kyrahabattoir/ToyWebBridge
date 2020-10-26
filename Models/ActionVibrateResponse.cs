/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using Buttplug.Core.Messages;
using Microsoft.AspNetCore.Http;

namespace ButtplugWebBridge.Models
{
    public class ActionVibrateResponse : BaseActionResponse
    {
        public uint Power { get; }
        public ActionVibrateResponse(HttpRequest request, string device, uint power) : base(request, device, typeof(VibrateCmd))
        {
            Power = power;
        }
    }
}
