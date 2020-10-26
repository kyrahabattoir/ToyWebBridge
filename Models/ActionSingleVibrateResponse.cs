using Buttplug.Core.Messages;
using Microsoft.AspNetCore.Http;

namespace ButtplugWebBridge.Models
{
    public class ActionSingleVibrateResponse : BaseActionResponse
    {
        public uint[] Power { get; }
        public ActionSingleVibrateResponse(HttpRequest request, string device, uint[] power) : base(request, device, typeof(SingleMotorVibrateCmd))
        {
            Power = power;
        }
    }
}
