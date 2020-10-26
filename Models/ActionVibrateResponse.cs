/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using Buttplug.Core.Messages;
using Microsoft.AspNetCore.Http;

namespace ButtplugWebBridge.Models
{
    public class ActionVibrateResponse : BaseActionResponse
    {
        public uint Speed { get; }
        public ActionVibrateResponse(HttpRequest request, string device, uint speed) : base(request, device, typeof(VibrateCmd))
        {
            Speed = speed;
        }
    }
}
