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
        public int DeviceCount()
        {
            return devices.Count;
        }
        public List<string> ListDevices()
        {
            return new List<string>(devices.Keys);
        }
        public bool IsDevice(string deviceName)
        {
            return devices.ContainsKey(deviceName);
        }
        public List<string> GetSupportedCommands(string deviceName)
        {
            if (!devices.ContainsKey(deviceName))
                return null;

            //TODO Make real commands.
            return devices[deviceName].AllowedMessages.Select(message => message.Key.Name).ToList();
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