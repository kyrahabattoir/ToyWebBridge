/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using Buttplug.Client;
using Buttplug.Core.Messages;
using Microsoft.Extensions.Logging;
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

        public DeviceRegister(ILogger<DeviceRegister> logger)
        {
            _logger = logger;
        }

        readonly IDictionary<string, ButtplugClientDevice> devices = new ConcurrentDictionary<string, ButtplugClientDevice>();

        /// <summary>
        /// Called when a device is announced by intiface.
        /// </summary>
        public void AddDevice(ButtplugClientDevice device)
        {
            if (devices.ContainsKey(device.Name))
            {
                _logger.LogDebug($"Trying to add already existing device ${device.Name}");
                return;
            }

            devices.Add(device.Name, device);

            _logger.LogInformation(String.Format("AddDevice {0} allowed commands:{1}\t{2}",
                                                                                        device.Name,
                                                                                        Environment.NewLine,
                                                                                        string.Join(Environment.NewLine + "\t", device.AllowedMessages.Select(x => $"{x.Key} {x.Value.FeatureCount}"))));
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

            _logger.LogInformation("RemoveDevice: " + device.Name);
            devices.Remove(device.Name);
        }

        /// <summary>
        /// Removes all registered devices/
        /// When we get disconnected from intiface (crash usually)
        /// </summary>
        public void RemoveAllDevices()
        {
            _logger.LogInformation("RemoveDevice: all devices unregistered.");
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
    }
}