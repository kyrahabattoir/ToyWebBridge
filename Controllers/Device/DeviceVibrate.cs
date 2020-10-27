/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using ButtplugWebBridge.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ButtplugWebBridge.Controllers
{
    public partial class DeviceController
    {
        // GET: /Device/<name>/VibrateCmd?speed=0
        // GET: /Device/<name>/VibrateCmd?speed=50
        // GET: /Device/<name>/VibrateCmd?speed=100
        [HttpGet("{name}/VibrateCmd")]
        public async Task<ActionResult> DeviceVibrate(string name, uint speed)
        {
            var response = new ActionVibrateResponse(Request, name, speed);

            if (!Register.IsDevice(name))
                return NotFound(response);

            if (!await Register.SendVibrateCmd(name, speed))
                return BadRequest(response);

            return Ok(response);
        }
    }
}
