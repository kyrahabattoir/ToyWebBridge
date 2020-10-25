using Microsoft.AspNetCore.Http;

namespace ButtplugWebBridge.Models
{
    public class DeviceListResponse : BasedResponse
    {
        public string[] DeviceList { get; }
        public DeviceListResponse(HttpRequest request, string[] devicelist) : base(request)
        {
            DeviceList = devicelist;
        }
    }
}
