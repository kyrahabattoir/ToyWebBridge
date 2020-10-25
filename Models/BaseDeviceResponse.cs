using Microsoft.AspNetCore.Http;

namespace ButtplugWebBridge.Models
{
    public class BaseDeviceResponse : BasedResponse
    {
        public string Device { get; }
        public BaseDeviceResponse(HttpRequest request, string device) : base(request)
        {
            Device = device;
        }
    }
}
