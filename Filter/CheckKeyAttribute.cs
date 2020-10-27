using ButtplugWebBridge.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace ButtplugWebBridge.Filter
{
    [AttributeUsage(validOn: AttributeTargets.Class)]
    public class CheckKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string Key = "SecretKey";
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var settings = BridgeSettings.Instance;
            IHeaderDictionary headers = context.HttpContext.Request.Headers;

            if (settings.AccessControl)
            {
                string supplied_key = string.Empty;

                if (headers.ContainsKey(Key))
                    supplied_key = headers[Key];

                if (settings.SecretKey == string.Empty || !settings.SecretKey.Equals(supplied_key))
                {
                    context.Result = new ContentResult() { StatusCode = 401 };
                    return;
                }
            }

            await next();
        }
    }
}
