using Microsoft.AspNetCore.Mvc;

namespace ButtplugWebBridge.Controllers
{
    public partial class DeviceController
    {
        // GET: /Device/<name>/SendVibrateCmd
        [HttpGet("{name}/SendVibrateCmd")]
        public string DeviceVibrate(string name)
        {
            return "SendVibrateCmd";
        }

        // GET: /Device/<name>/SendVibrateCmd/<power>
        [HttpGet("{name}/SendVibrateCmd/{power}")]
        public string DeviceVibrate(string name, uint power)
        {
            return "SendVibrateCmd + power";
        }
    }
}