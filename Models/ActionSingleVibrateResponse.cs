/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using Buttplug.Core.Messages;
using Microsoft.AspNetCore.Http;

namespace ButtplugWebBridge.Models
{
    public class ActionSingleVibrateResponse : BaseActionResponse
    {
        public uint[] Speed { get; }
        public ActionSingleVibrateResponse(HttpRequest request, string device, uint[] speed) : base(request, device, typeof(SingleMotorVibrateCmd))
        {
            Speed = speed;
        }
    }
}
