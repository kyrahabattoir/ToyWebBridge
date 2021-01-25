/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using Buttplug;
using ToyWebBridge.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Buttplug.ServerMessage.Types;

// All Stop/start commands automatically clear any playing sequence.
namespace ToyWebBridge.Models
{
    public class DeviceContainer
    {
        private readonly ILogger<DeviceRegister> _logger;

        readonly ButtplugClientDevice _device;
        readonly int _command_delay;
        readonly float _power_multiplier;
        bool _busy = false;

        CancellationTokenSource _runner;

        public DeviceContainer(ButtplugClientDevice device, ILogger<DeviceRegister> logger, uint command_delay, uint power_factor)
        {
            _device = device;
            _logger = logger;
            _command_delay = (int)command_delay;
            _power_multiplier = Math.Clamp(power_factor / 100.0f, 0f, 1f);

            //This is just convenience, should the ButtPlug server crash while the device is on,
            //so when the device comes back we first stop whatever it was doing.
            _ = StopDeviceCmd();
        }
        /// <summary>
        /// Acts like a semaphore, blocking new instructions while it is running.
        /// </summary>
        /// <returns></returns>
        async Task SetBusy()
        {
            _busy = true;
            await Task.Delay(_command_delay);
            _busy = false;
        }

        public async Task<bool> SendVibrateCmd(uint speed)
        {
            if (VibrationMotorCount == 0)
                return false;

            if (_busy)
                return false;

            if (_runner != null)
                _runner.Cancel();

            _ = SetBusy();
            await _device.SendVibrateCmd(Math.Clamp(speed * 0.01f * _power_multiplier, 0f, 1f));
            return true;
        }

        public async Task<bool> SendVibrateCmd(IEnumerable<uint> speed)
        {
            if (VibrationMotorCount != speed.Count())
                return false;

            if (_busy)
                return false;

            List<double> data = new List<double>();
            foreach (uint entry in speed)
                data.Add(Math.Clamp(entry * 0.01f * _power_multiplier, 0f, 1f));

            if (_runner != null)
                _runner.Cancel();

            _ = SetBusy();
            await _device.SendVibrateCmd(data);
            return true;
        }

        public async Task<bool> StopDeviceCmd()
        {
            if (!_device.AllowedMessages.ContainsKey(MessageAttributeType.StopDeviceCmd))
                return false;

            if (_runner != null)
                _runner.Cancel();

            //_ = SetBusy(); Probably a good idea to bypass the delay just for this command.
            await _device.SendStopDeviceCmd();
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
                    var motor_index = i;
                    //if there is only one motor entry, only use that.
                    if (pattern.Speeds.Count == 1)
                        motor_index = 0;

                    //if a motor entry/value was not supplied in the pattern, no big deal, assume zero.
                    var motor_entry = pattern.Speeds.ElementAtOrDefault(motor_index);
                    var motor_speed = motor_entry.ElementAtOrDefault(sequence_index);
                    payload.Add(Math.Clamp(motor_speed * 0.01f * _power_multiplier, 0f, 1f));
                }

                //2. send the payload
                if (!_busy)
                {
                    _ = SetBusy();
                    await _device.SendVibrateCmd(payload);
                }

                //3. sleep
                await Task.Delay(time);

                //4. rinse and repeat
                sequence_index++;
            }
        }

        public uint? VibrationMotorCount => _device.AllowedMessages[MessageAttributeType.VibrateCmd].FeatureCount;
        public Dictionary<MessageAttributeType, ButtplugMessageAttributes> AllowedMessages
        {
            get { return _device.AllowedMessages; }
        }
    }
}
