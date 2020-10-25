using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ButtplugWebBridge.Services;
using ButtplugWebBridge.Models;

namespace ButtplugWebBridge.Controllers
{
    [Route("Device")]
    [ApiController]
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
        public ActionResult Get()
        {
            return Ok(new DeviceList(Request, Register.ListDevices().Select(d => d).ToArray()));
        }

        [HttpGet("{name}")]
        public ActionResult Device(string name)
        {
            DeviceResponse output = new DeviceResponse(Request, name);

            if (!Register.IsDevice(name))
                return NotFound(output);

            output.Capabilities = Register.GetSupportedCommands(name);
            return Ok(output);
        }
    }
}
