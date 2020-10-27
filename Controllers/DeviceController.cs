/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ButtplugWebBridge.Services;
using ButtplugWebBridge.Models;
using ButtplugWebBridge.Filter;

namespace ButtplugWebBridge.Controllers
{
    [Route("Device")]
    [ApiController]
    [CheckKey]
    public partial class DeviceController : ControllerBase
    {
        private readonly ILogger<DeviceController> _logger;
        private DeviceRegister Register { get; }
        public DeviceController(ILogger<DeviceController> logger, DeviceRegister register)
        {
            _logger = logger;
            Register = register;
        }

        [HttpGet]
        public ActionResult Device()
        {
            return Ok(new DeviceListResponse(Request, Register.ListDevices().Select(d => d).ToArray()));
        }

        [HttpGet("{name}")]
        public ActionResult DeviceName(string name)
        {
            if (!Register.IsDevice(name))
                return NotFound(new BaseDeviceResponse(Request, name));

            return Ok(new DeviceResponse(Request, name, Register.GetSupportedCommands(name)));
        }
    }
}
