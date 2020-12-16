/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using System.Collections.Generic;

namespace ToyWebBridge.Services
{
    public class BridgeSettings
    {
        public string SecretKey { get; set; }
        public bool DeviceNameCloaking { get; set; }
        public Dictionary<string, string> NameCloakingTable { get; set; }

        public uint WebSocketPort { get; set; }

        public static BridgeSettings Instance;
        public BridgeSettings()
        {
            Instance = this;
        }
    }
}
