/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ButtplugWebBridge.Services;
using ButtplugWebBridge.Models;
using Microsoft.Extensions.Options;

namespace ButtplugWebBridge.Controllers
{
    [Route("Device")]
    [ApiController]
    public partial class DeviceController : ControllerBase
    {
        private readonly ILogger<DeviceController> _logger;
        private readonly BridgeSettings _settings;

        private DeviceRegister Register { get; }
        public DeviceController(ILogger<DeviceController> logger, DeviceRegister register, IOptions<BridgeSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
            Register = register;
        }

        [HttpGet]
        public ActionResult Device(string pw)
        {
            if (!HasAccess(pw)) return Unauthorized();

            return Ok(new DeviceListResponse(Request, Register.ListDevices().Select(d => d).ToArray()));
        }

        [HttpGet("{name}")]
        public ActionResult DeviceName(string name, string pw)
        {
            if (!HasAccess(pw)) return Unauthorized();

            if (!Register.IsDevice(name))
                return NotFound(new BaseDeviceResponse(Request, name));

            return Ok(new DeviceResponse(Request, name, Register.GetSupportedCommands(name)));
        }

        bool HasAccess(string pw)
        {
            if (_settings.Password == "")
                return true;
            if (_settings.Password == pw)
                return true;
            return false;
        }
    }
}
