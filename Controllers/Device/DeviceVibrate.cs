using Buttplug.Core.Messages;
using ButtplugWebBridge.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ButtplugWebBridge.Controllers
{
    public partial class DeviceController
    {
        // GET: /Device/<name>/VibrateCmd?power=0
        // GET: /Device/<name>/VibrateCmd?power=50
        // GET: /Device/<name>/VibrateCmd?power=100
        [HttpGet("{name}/VibrateCmd")]
        public async Task<ActionResult> DeviceVibrate(string name, uint power)
        {
            Type action = typeof(VibrateCmd);

            if (!Register.IsDevice(name))
                return NotFound(new BaseActionResponse(Request, name, action));

            if (!await Register.SendVibrateCmd(name, power))
                return BadRequest(new BaseActionResponse(Request, name, action));

            return Ok(new ActionVibrateResponse(Request, name, power));
        }
    }
}
