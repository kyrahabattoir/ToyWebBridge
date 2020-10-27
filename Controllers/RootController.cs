/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ButtplugWebBridge.Controllers
{
    [Route("/")]
    [ApiController]
    public class RootController : ControllerBase
    {
        // GET: /
        [HttpGet]
        public ActionResult Get()
        {
            return NotFound();
        }
    }
}
