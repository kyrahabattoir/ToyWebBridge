/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using ButtplugWebBridge.Filter;
using ButtplugWebBridge.Models;
using ButtplugWebBridge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ButtplugWebBridge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CheckKey]
    public class DeviceController : Controller
    {
        private readonly ILogger<DeviceController> _logger;
        private DeviceRegister Register { get; }
        public DeviceController(ILogger<DeviceController> logger, DeviceRegister register)
        {
            _logger = logger;
            Register = register;
        }
        [Route("[action]")]
        [HttpGet]
        public ActionResult List()
        {
            return Ok(new DeviceListResponse("List", Register.ListDevices().Select(d => d).ToArray()));
        }

        [Route("[action]/{name}")]
        [HttpGet]
        public ActionResult Info(string name)
        {
            if (!Register.IsDevice(name))
                return NotFound(new BaseDeviceResponse("Info", name));

            return Ok(new DeviceResponse("Info", name, Register.GetSupportedCommands(name)));
        }

        [Route("[action]/{name}")]
        [HttpGet]
        public async Task<ActionResult> StopDeviceCmd(string name)
        {
            BasedResponse output = new BaseDeviceResponse("StopDeviceCmd", name);

            if (!Register.IsDevice(name))
                return NotFound(output);

            if (!await Register.StopDeviceCmd(name))
                return BadRequest(output);

            return Ok(output);
        }

        [Route("[action]/{name}/{speed}")]
        [HttpGet]
        public async Task<ActionResult> VibrateCmd(string name, uint speed)
        {
            var response = new ActionVibrateResponse("VibrateCmd", name, speed);

            if (!Register.IsDevice(name))
                return NotFound(response);

            if (!await Register.SendVibrateCmd(name, speed))
                return BadRequest(response);

            return Ok(response);
        }

        [Route("[action]/{name}/{speed}")]
        [HttpGet]
        public async Task<ActionResult> SingleMotorVibrateCmd(string name, string speed)
        {
            var response = new BaseDeviceResponse("SingleMotorVibrateCmd", name);

            if (speed == null)
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

            response = new ActionSingleVibrateResponse("SingleMotorVibrateCmd", name, speeds);

            if (!Register.IsDevice(name))
                return NotFound(response);

            if (!await Register.SendVibrateCmd(name, speeds))
                return BadRequest(response);

            return Ok(response);
        }

        [Route("[action]/{name}")]
        [HttpPost]
        public ActionResult SequenceVibrateCmd(string name, [FromBody] VibrationPattern pattern)
        {
            var response = new BaseDeviceResponse("SequenceVibrateCmd", name);

            if (!Register.IsDevice(name))
                return NotFound(response);

            //FIXME Need something better as return.

            if (pattern == null)
                return BadRequest(response);

            if (!Register.SequenceVibrateCmd(name, pattern))
                return BadRequest(response);

            return Ok(response);
        }
    }
}
