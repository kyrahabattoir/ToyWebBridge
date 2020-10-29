/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */

namespace ButtplugWebBridge.Models
{
    public class BaseDeviceResponse : BasedResponse
    {
        public string Device { get; }
        public BaseDeviceResponse(string action, string device) : base(action)
        {
            Device = device;
        }
    }
}
