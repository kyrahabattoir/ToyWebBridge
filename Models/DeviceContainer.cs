using Buttplug.Client;
using Buttplug.Core.Messages;
using ButtplugWebBridge.Controllers;
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
        /// <param name="values">a list that alternates speed (%) and time (ms)</param>
        /// <param name="loop">whether the sequence should loop</param>
        /// <returns></returns>
        public bool SendVibrateSequence(List<uint> values, bool loop)
        {
            if (VibrationMotorCount == 0)
                return false;

            if (values.Count % 2 != 0)
                return false;

            List<DeviceInstruction> sequence = new List<DeviceInstruction>();

            for (var i = 0; i < values.Count; i += 2)
            {
                var speed = values[i];
                var time = values[i + 1];

                var speeds = new List<uint>();
                for (var j = 0; j < VibrationMotorCount; j++)
                    speeds.Add(speed);

                sequence.Add(new DeviceInstruction(speeds.ToArray(), time));
            }

            if (_runner != null)
                _runner.Cancel();

            _runner = new CancellationTokenSource();

            _ = SequenceRunner(sequence, loop, _runner.Token);
            return true;
        }

        async Task SequenceRunner(List<DeviceInstruction> sequence, bool loop, CancellationToken token)
        {
            int sequence_index = 0;

            while (!token.IsCancellationRequested)
            {
                if (sequence_index >= sequence.Count)
                {
                    if (!loop)
                        break;

                    sequence_index = 0;
                }

                var instruction = sequence[sequence_index];

                List<double> data = new List<double>();
                foreach (uint entry in instruction.speeds)
                    data.Add(Math.Clamp(entry * 0.01f, 0f, 1f));

                await _device.SendVibrateCmd(data);
                await Task.Delay((int)instruction.delay);

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
