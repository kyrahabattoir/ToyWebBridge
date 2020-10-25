using ButtplugWebBridge.Models;
using Microsoft.AspNetCore.Mvc;
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
            DeviceActionResponse output = new DeviceActionResponse(Request, name, "VibrateCmd", power.ToString());

            if (!Register.IsDevice(name))
                return NotFound(output);

            if (!await Register.SendVibrateCmd(name, power))
                return BadRequest(output);

            return Ok(output);
        }
    }
}
