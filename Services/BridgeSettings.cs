using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ButtplugWebBridge.Services
{
    public class BridgeSettings
    {
        public string SecretKey { get; set; }
        public static BridgeSettings Instance;
        public BridgeSettings()
        {
            Instance = this;
        }
    }
}
