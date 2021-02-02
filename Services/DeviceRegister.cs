/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using Buttplug;
using ToyWebBridge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Buttplug.ServerMessage.Types;

namespace ToyWebBridge.Services
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

        readonly IDictionary<string, DeviceContainer> devices = new ConcurrentDictionary<string, DeviceContainer>();

        /// <summary>
        /// Called when the server disconnects.
        /// </summary>
        public void OnServerDisconnect()
        {
            devices.Clear();
        }

        /// <summary>
        /// Called when a device is announced by intiface.
        /// </summary>
        public void OnDeviceAdded(ButtplugClientDevice device)
        {
            string name = device.Name;
            string real_name = name;

            ToySettings toy = _settings.GetToy(device.Name);

            if (toy != null && toy.VisibleName != null && toy.VisibleName.Length > 0)
                name = DeCollideDeviceName(toy.VisibleName);
            else
                name = DeCollideDeviceName(name);

            uint toy_delay = BridgeSettings.MIN_COMMAND_RATE;
            uint toy_power = 100;
            if (toy != null)
            {
                if (toy.CommandRate.HasValue)
                    toy_delay = Math.Max(BridgeSettings.MIN_COMMAND_RATE, toy.CommandRate.Value);

                if (toy.PowerFactor.HasValue)
                    toy_power = toy.PowerFactor.Value;
            }

            devices.Add(name, new DeviceContainer(device, _logger, toy_delay, toy_power));

            _logger.LogInformation(String.Format("\nNew device detected\n{0} ({1})\n  Update rate: {2}ms\n  Power: {3}%", name, real_name, toy_delay, toy_power));
        }

        /// <summary>
        /// Called when a device is removed by intiface.
        /// </summary>
        public void OnDeviceRemoved(ButtplugClientDevice device)
        {
            if (!devices.ContainsKey(device.Name))
            {
                _logger.LogDebug($"Can't find ${device.Name} Trying to remove a nonexisting device.");
                return;
            }

            _logger.LogInformation("Device Removed: " + device.Name);

            devices.Remove(device.Name);
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
            Dictionary<string, uint> features = devices[deviceName].AllowedMessages.ToDictionary(message => message.Key.ToString(),
                                                                                                 message => message.Value.FeatureCount);

            //Injecting my own commands in the feature list.
            //Supposedly if we can vibrate, we can sequencevibrate.
            if (features.ContainsKey(MessageAttributeType.VibrateCmd.ToString()))
                features.Add("SequenceVibrateCmd", 0);

            return features;
        }
        public async Task<bool> SendVibrateCmd(string device_name, uint speed)
        {
            if (!devices.ContainsKey(device_name))
                return false;

            var device = devices[device_name];

            if (device.VibrationMotorCount == 0)
                return false;

            await device.SendVibrateCmd(speed);
            return true;
        }
        public async Task<bool> SendVibrateCmd(string device_name, IEnumerable<uint> speed)
        {
            if (!devices.ContainsKey(device_name))
                return false;

            var device = devices[device_name];

            if (device.VibrationMotorCount == 0)
                return false;

            if (device.VibrationMotorCount != speed.Count())
            {
                _logger.LogInformation("SendVibrateCmd: failed featurecount check.");

                return false;
            }

            await device.SendVibrateCmd(speed);
            return true;
        }
        public async Task<bool> StopDeviceCmd(string deviceName)
        {
            if (!devices.ContainsKey(deviceName))
                return false;

            var device = devices[deviceName];

            if (!device.AllowedMessages.ContainsKey(MessageAttributeType.StopDeviceCmd))
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

            if (!devices.ContainsKey(name))
                return name;

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

        public bool SequenceVibrateCmd(string device_name, VibrationPattern pattern)
        {
            if (!devices.ContainsKey(device_name))
                return false;

            return devices[device_name].SendVibrateSequence(pattern);
        }
    }
}
