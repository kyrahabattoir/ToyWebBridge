/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */

namespace ButtplugWebBridge.Models
{
    public class DeviceListResponse : BasedResponse
    {
        public string[] Devices { get; }
        public DeviceListResponse(string action, string[] devicelist) : base(action)
        {
            Devices = devicelist;
        }
    }
}
