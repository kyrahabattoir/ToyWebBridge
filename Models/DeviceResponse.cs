using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace ButtplugWebBridge.Models
{
    public abstract class BasedResponse //yes.
    {
        public string Query { get; }

        public BasedResponse(HttpRequest request) { Query = request.Path; }
    }
    public class DeviceList : BasedResponse
    {
        public string[] Devices { get; set; }
        public DeviceList(HttpRequest request, string[] devices) : base(request) { Devices = devices; }
    }
    public class DeviceResponse : BasedResponse
    {
        public string Device { get; }
        public string[] Capabilities { get; set; }
        public DeviceResponse(HttpRequest request, string device) : base(request) { Device = device; }
    }
    public class DeviceActionResponse : BasedResponse
    {
        public string Device { get; }
        public string Action { get; }
        public string Param { get; }
        public DeviceActionResponse(HttpRequest request, string device, string action, string param = "n/a") : base(request)
        {
            Device = device;
            Action = action;
            Param = param;
        }
    }
}
