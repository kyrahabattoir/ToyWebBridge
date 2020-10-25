using ButtplugWebBridge.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ButtplugWebBridge.Controllers
{
    public partial class DeviceController
    {
        // GET: /Device/<name>/StopDeviceCmd
        [HttpGet("{name}/StopDeviceCmd")]
        public async Task<ActionResult> DeviceStop(string name)
        {
            DeviceActionResponse output = new DeviceActionResponse(Request, "StopDeviceCmd", name);

            if (!await Register.StopDeviceCmd(name))
                return StatusCode(405, output);

            return Ok(output);
        }
    }
}
