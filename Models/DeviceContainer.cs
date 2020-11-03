using Buttplug.Client;
using Buttplug.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// All Stop/start commands automatically clear any playing sequence.
namespace ButtplugWebBridge.Models
{
    public class DeviceContainer
    {
        readonly ButtplugClientDevice _device;
        public DeviceContainer(ButtplugClientDevice device)
        {
            _device = device;
        }

        public async Task<bool> SendVibrateCmd(uint speed)
        {
            if (VibrationMotorCount == 0)
                return false;

            await _device.SendVibrateCmd(Math.Clamp(speed * 0.01f, 0f, 1f));
            return true;
        }

        public async Task<bool> SendVibrateCmd(IEnumerable<uint> speed)
        {
            if (VibrationMotorCount != speed.Count())
                return false;

            List<double> data = new List<double>();
            foreach (uint entry in speed)
                data.Add(Math.Clamp(entry * 0.01f, 0f, 1f));

            await _device.SendVibrateCmd(data);
            return true;
        }

        public async Task<bool> StopDeviceCmd()
        {
            if (!_device.AllowedMessages.ContainsKey(typeof(StopDeviceCmd)))
                return false;

            await _device.StopDeviceCmd();
            return true;
        }

        public uint? VibrationMotorCount => _device.AllowedMessages[typeof(VibrateCmd)].FeatureCount;
        public Dictionary<Type, MessageAttributes> AllowedMessages
        {
            get { return _device.AllowedMessages; }
        }
    }
}
