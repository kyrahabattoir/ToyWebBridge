using Microsoft.AspNetCore.Mvc;

namespace ButtplugWebBridge.Controllers
{
    public partial class DeviceController
    {
        // GET: /Device/<name>/StopDeviceCmd
        [HttpGet("{name}/StopDeviceCmd")]
        public string DeviceStop(string name)
        {
            return "StopDeviceCmd";
        }
    }
}