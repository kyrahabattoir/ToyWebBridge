/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using Buttplug.Core.Messages;
using ButtplugWebBridge.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ButtplugWebBridge.Controllers
{
    public partial class DeviceController
    {
        // GET: /Device/<name>/StopDeviceCmd
        [HttpGet("{name}/StopDeviceCmd")]
        public async Task<ActionResult> DeviceStop(string name, string pw)
        {
            if (!HasAccess(pw)) return Unauthorized();

            BaseActionResponse output = new BaseActionResponse(Request, name, typeof(StopDeviceCmd));

            if (!Register.IsDevice(name))
                return NotFound(output);

            if (!await Register.StopDeviceCmd(name))
                return BadRequest(output);

            return Ok(output);
        }
    }
}
