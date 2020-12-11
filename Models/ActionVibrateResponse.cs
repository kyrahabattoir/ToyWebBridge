/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */

namespace ToyWebBridge.Models
{
    public class ActionVibrateResponse : BaseDeviceResponse
    {
        public uint Speed { get; }
        public ActionVibrateResponse(string action, string device, uint speed) : base(action, device)
        {
            Speed = speed;
        }
    }
}
