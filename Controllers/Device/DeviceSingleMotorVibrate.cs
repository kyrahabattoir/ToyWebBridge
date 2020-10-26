using Buttplug.Core.Messages;
using ButtplugWebBridge.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ButtplugWebBridge.Controllers
{
    public partial class DeviceController
    {
        // GET: /Device/<name>/SingleMotorVibrateCmd?speed=100,0
        // GET: /Device/<name>/SingleMotorVibrateCmd?speed=50,10
        // GET: /Device/<name>/SingleMotorVibrateCmd?speed=0,100
        [HttpGet("{name}/SingleMotorVibrateCmd")]
        public async Task<ActionResult> SingleMotorVibrate(string name, string speed)
        {
            Type action = typeof(SingleMotorVibrateCmd);

            if (!Register.IsDevice(name))
            {
                return NotFound(new BaseActionResponse(Request, name, action));
            }

            uint[] speeds;
            try
            {
                speeds = speed.Split(',').Select(uint.Parse).ToArray();
            }
            catch (FormatException)
            {
                return BadRequest(new BaseActionResponse(Request, name, action));
            }

            if (!await Register.SendVibrateCmd(name, speeds))
                return BadRequest(new BaseActionResponse(Request, name, action));

            return Ok(new ActionSingleVibrateResponse(Request, name, speeds));
        }
    }
}
