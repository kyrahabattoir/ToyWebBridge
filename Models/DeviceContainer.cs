using Buttplug.Client;
using Buttplug.Core.Messages;
using ButtplugWebBridge.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// All Stop/start commands automatically clear any playing sequence.
namespace ButtplugWebBridge.Models
{
    public class DeviceContainer
    {
        private readonly ILogger<DeviceRegister> _logger;

        readonly ButtplugClientDevice _device;

        CancellationTokenSource _runner;

        public DeviceContainer(ButtplugClientDevice device, ILogger<DeviceRegister> logger)
        {
            _logger = logger;
            _device = device;
        }

        public async Task<bool> SendVibrateCmd(uint speed)
        {
            if (VibrationMotorCount == 0)
                return false;

            if (_runner != null)
                _runner.Cancel();

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

            if (_runner != null)
                _runner.Cancel();

            await _device.SendVibrateCmd(data);
            return true;
        }

        public async Task<bool> StopDeviceCmd()
        {
            if (!_device.AllowedMessages.ContainsKey(typeof(StopDeviceCmd)))
                return false;

            if (_runner != null)
                _runner.Cancel();

            await _device.StopDeviceCmd();
            return true;
        }

        /// <summary>
        /// Plays a vibration sequence on the device.
        /// </summary>
        public bool SendVibrateSequence(VibrationPattern pattern)
        {
            if (pattern.MotorCount == 0)
                return false;

            if (!pattern.Validate())
                return false;

            if (_runner != null)
                _runner.Cancel();

            _runner = new CancellationTokenSource();

            _ = SequenceRunner(pattern, _runner.Token);
            return true;
        }

        async Task SequenceRunner(VibrationPattern pattern, CancellationToken token)
        {
            int sequence_index = 0;

            while (!token.IsCancellationRequested)
            {
                //either loop the sequence or end.
                if (sequence_index >= pattern.Time.Count)
                {
                    if (!pattern.Loop)
                        break;

                    sequence_index = 0;
                }

                int time = (int)pattern.Time[sequence_index];

                //1. build payload.
                List<double> payload = new List<double>();
                for (var i = 0; i < VibrationMotorCount; i++)
                {
                    //if a motor entry was not supplied in the pattern, no big deal, assume zero.
                    var motor_entry = pattern.Speeds[i];
                    var motor_speed = motor_entry.ElementAtOrDefault(sequence_index);
                    payload.Add(Math.Clamp(motor_speed * 0.01f, 0f, 1f));
                }

                //2. send the payload
                await _device.SendVibrateCmd(payload);

                //3. sleep
                await Task.Delay(time);

                //4. rinse and repeat
                sequence_index++;
            }
        }

        public uint? VibrationMotorCount => _device.AllowedMessages[typeof(VibrateCmd)].FeatureCount;
        public Dictionary<Type, MessageAttributes> AllowedMessages
        {
            get { return _device.AllowedMessages; }
        }
    }
}
