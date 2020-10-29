/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */

namespace ButtplugWebBridge.Models
{
    public class ActionSingleVibrateResponse : BaseDeviceResponse
    {
        public uint[] Speed { get; }
        public ActionSingleVibrateResponse(string action, string device, uint[] speed) : base(action, device)
        {
            Speed = speed;
        }
    }
}
