/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
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
            var response = new BaseActionResponse(Request, name, action);

            if (speed==null)
                return BadRequest(response);

            uint[] speeds;
            try
            {
                speeds = speed.Split(',').Select(uint.Parse).ToArray();
            }
            catch (FormatException)
            {
                return BadRequest(response);
            }

            response = new ActionSingleVibrateResponse(Request, name, speeds);

            if (!Register.IsDevice(name))
                return NotFound(response);

            if (!await Register.SendVibrateCmd(name, speeds))
                return BadRequest(response);

            return Ok(response);
        }
    }
}
