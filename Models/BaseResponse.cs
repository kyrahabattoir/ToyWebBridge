/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using Microsoft.AspNetCore.Http;

namespace ButtplugWebBridge.Models
{
    public class BasedResponse //yes.
    {
        public string Query { get; }
        public BasedResponse(HttpRequest request)
        {
            Query = request.Path;
        }
    }
}
