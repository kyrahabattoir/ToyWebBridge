using Microsoft.AspNetCore.Http;

namespace ButtplugWebBridge.Models
{
    public class BasedResponse //yes.
    {
        public string Query { get; }

        public BasedResponse(HttpRequest request) { Query = request.Path; }
    }
}
