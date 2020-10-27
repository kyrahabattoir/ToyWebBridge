using System.Collections.Generic;

namespace ButtplugWebBridge.Services
{
    public class BridgeSettings
    {
        public string SecretKey { get; set; }
        public bool DeviceNameCloaking { get; set; }
        public Dictionary<string, string> NameCloakingTable { get; set; }

        public static BridgeSettings Instance;
        public BridgeSettings()
        {
            Instance = this;
        }
    }
}
