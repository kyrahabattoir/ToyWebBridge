/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using Buttplug.Client;
using Buttplug.Core.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ButtplugWebBridge.Services
{
    public class DeviceRegister
    {
        private readonly ILogger<DeviceRegister> _logger;
        private readonly BridgeSettings _settings;

        public DeviceRegister(ILogger<DeviceRegister> logger, IOptions<BridgeSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
        }

        readonly IDictionary<string, ButtplugClientDevice> devices = new ConcurrentDictionary<string, ButtplugClientDevice>();

        /// <summary>
        /// Called when a device is announced by intiface.
        /// </summary>
        public void AddDevice(ButtplugClientDevice device)
        {
            string name = device.Name;
            string real_name = ".";

            if (_settings.DeviceNameCloaking)
            {
                real_name = string.Format(" ({0}).", device.Name);
                name = CloakDeviceName(name);
            }

            name = DeCollideDeviceName(name);

            devices.Add(name, device);

            _logger.LogInformation(String.Format("New device detected: {0}{1}", name, real_name));
        }

        /// <summary>
        /// Called when a device is removed by intiface.
        /// </summary>
        public void RemoveDevice(ButtplugClientDevice device)
        {
            if (!devices.ContainsKey(device.Name))
            {
                _logger.LogDebug($"Can't find ${device.Name} Trying to remove a nonexisting device.");
                return;
            }

            _logger.LogInformation("Device Removed: " + device.Name);
            devices.Remove(device.Name);
        }

        /// <summary>
        /// Removes all registered devices/
        /// When we get disconnected from intiface (crash usually)
        /// </summary>
        public void RemoveAllDevices()
        {
            devices.Clear();
        }

        public List<string> ListDevices()
        {
            return new List<string>(devices.Keys);
        }

        public bool IsDevice(string deviceName)
        {
            return devices.ContainsKey(deviceName);
        }
        public Dictionary<string, uint> GetSupportedCommands(string deviceName)
        {
            if (!devices.ContainsKey(deviceName))
                return null;

            //TODO Make real commands.
            return devices[deviceName].AllowedMessages.ToDictionary(message => message.Key.Name,
                                                                    message => message.Value.FeatureCount.GetValueOrDefault());
        }
        public async Task<bool> SendVibrateCmd(string device_name, uint speed)
        {
            if (!devices.ContainsKey(device_name))
                return false;

            var device = devices[device_name];

            if (!device.AllowedMessages.ContainsKey(typeof(VibrateCmd)))
                return false;

            await device.SendVibrateCmd(Math.Clamp(speed * 0.01f, 0f, 1f));
            return true;
        }
        public async Task<bool> SendVibrateCmd(string device_name, IEnumerable<uint> speed)
        {
            if (!devices.ContainsKey(device_name))
                return false;

            var device = devices[device_name];
            var VibrateCmd = typeof(VibrateCmd);

            if (!device.AllowedMessages.ContainsKey(VibrateCmd))
                return false;

            if (device.AllowedMessages[VibrateCmd].FeatureCount != speed.Count())
            {
                _logger.LogInformation("SendVibrateCmd: failed featurecount check.");
                return false;
            }

            List<double> data = new List<double>();
            foreach (uint entry in speed)
                data.Add(Math.Clamp(entry * 0.01f, 0f, 1f));

            await device.SendVibrateCmd(data);
            return true;
        }
        public async Task<bool> StopDeviceCmd(string deviceName)
        {
            if (!devices.ContainsKey(deviceName))
                return false;

            var device = devices[deviceName];

            if (!device.AllowedMessages.ContainsKey(typeof(StopDeviceCmd)))
                return false;

            await device.StopDeviceCmd();
            return true;
        }

        /// <summary>
        /// Take the original device name and create a new one.
        /// For those rare cases where identically named devices are detected.
        /// </summary>
        /// <param name="name">Original device name</param>
        /// <returns>Collision-free name</returns>
        private string DeCollideDeviceName(string name)
        {
            //Should we just remove spaces in toy names?
            //name.Replace(" ", "_");

            if (!devices.ContainsKey(name)) return name;

            int suffix = 2;
            string new_name;
            do
            {
                new_name = name + " " + suffix.ToString();
                suffix++;
            }
            while (devices.ContainsKey(new_name));

            return new_name;
        }
        /// <summary>
        /// Cloaks the device's name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string CloakDeviceName(string name)
        {
            if (!_settings.NameCloakingTable.ContainsKey(name))
                return name;

            return _settings.NameCloakingTable[name];
        }
    }
}
