using ToyWebBridge.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;
using System.Net;

namespace ToyWebBridge.Filter
{
    [AttributeUsage(validOn: AttributeTargets.Class)]
    public class CheckKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string Key = "SecretKey";
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var settings = BridgeSettings.Instance;

            var remoteIp = context.HttpContext.Connection.RemoteIpAddress;
            bool ip_not_allowed = false;
            if (settings.AllowedIPs != null && settings.AllowedIPs.Length > 0)
            {
                ip_not_allowed = true;

                if (remoteIp.IsIPv4MappedToIPv6)
                    remoteIp = remoteIp.MapToIPv4();

                foreach (var address in settings.AllowedIPs)
                {
                    var testIp = IPAddress.Parse(address);
                    if (testIp.Equals(remoteIp))
                    {
                        ip_not_allowed = false;
                        break;
                    }
                }
            }
            if (ip_not_allowed)
            {
                //_logger.LogWarning("Forbidden Request from IP: {RemoteIp}", remoteIp);
                context.Result = new ContentResult() { StatusCode = 403 };
                return;
            }

            IHeaderDictionary headers = context.HttpContext.Request.Headers;
            string supplied_key = string.Empty;

            if (headers.ContainsKey(Key))
                supplied_key = headers[Key];

            if (!settings.SecretKey.Equals(supplied_key))
            {
                context.Result = new ContentResult() { StatusCode = 401 };
                return;
            }

            await next();
        }
    }
}
