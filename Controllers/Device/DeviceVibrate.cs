using Buttplug.Core.Messages;
using ButtplugWebBridge.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ButtplugWebBridge.Controllers
{
    public partial class DeviceController
    {
        // GET: /Device/<name>/VibrateCmd
        [HttpGet("{name}/VibrateCmd")]
        public string DeviceVibrate(string name)
        {
            return "SendVibrateCmd";
        }

        // GET: /Device/<name>/VibrateCmd/<power>
        [HttpGet("{name}/VibrateCmd/{power}")]
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
