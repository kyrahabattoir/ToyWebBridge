/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using System.Collections.Generic;

namespace ToyWebBridge.Services
{
    public class BridgeSettings
    {
        public const uint MIN_COMMAND_RATE = 50;
        public string SecretKey { get; set; }
        public uint WebSocketPort { get; set; }
        public ToySettings[] ToySettings { get; set; }

        public static BridgeSettings Instance;

        Dictionary<string, ToySettings> _toysettings;
        public BridgeSettings()
        {
            Instance = this;
        }

        public ToySettings GetToy(string name)
        {
            Build();

            if (_toysettings.ContainsKey(name))
                return _toysettings[name];

            return null;
        }

        void Build()
        {
            if (_toysettings != null)
                return;

            _toysettings = new Dictionary<string, ToySettings>();

            foreach (ToySettings entry in ToySettings)
                _toysettings.Add(entry.Name, entry);
        }
    }

    public class ToySettings
    {
        public uint? CommandRate { get; set; }
        public uint? PowerFactor { get; set; }
        public string Name { get; set; }
        public string VisibleName { get; set; }
    }
}
