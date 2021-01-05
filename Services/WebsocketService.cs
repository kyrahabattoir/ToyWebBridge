/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using Buttplug;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using UniqueKey;

namespace ToyWebBridge.Services
{
    public class WebsocketService : IHostedService, IDisposable
    {
        private readonly ILogger<WebsocketService> _logger;
        private readonly BridgeSettings _settings;
        private Timer _timer;
        private bool isScanning;

        private DeviceRegister Register { get; }
        private ButtplugClient client;
        private string _websocket_url;

        public WebsocketService(ILogger<WebsocketService> logger, DeviceRegister register, IOptions<BridgeSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;

            Register = register;
        }
        /********************************
         * Service start/stop
         ********************************/
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Toy Web Bridge is starting.");

            if (_settings.SecretKey == string.Empty)
            {
                _settings.SecretKey = KeyGenerator.GetUniqueKey(20);
                _logger.LogWarning($"\n /!\\ Web bridge (generated) SecretKey: " + _settings.SecretKey + " /!\\\n");
            }
            else
                _logger.LogWarning($"\n /!\\ Web bridge SecretKey: " + _settings.SecretKey + " /!\\\n");

            client = new ButtplugClient("Simple HTTP Bridge");
            client.DeviceAdded += (source, args) => Register.OnDeviceAdded(args.Device);
            client.DeviceRemoved += (source, args) => Register.OnDeviceRemoved(args.Device);
            client.ErrorReceived += OnErrorReceived;
            client.PingTimeout += OnPingTimeout;
            client.ScanningFinished += OnScanningFinished;
            client.ServerDisconnect += OnServerDisconnect;
            client.ServerDisconnect += (source, args) => Register.OnServerDisconnect();

            _websocket_url = string.Format("ws://localhost:{0}/buttplug", _settings.WebSocketPort);
            _logger.LogInformation("Websocket url is: " + _websocket_url);

            _timer = new Timer(MonitorWebsocket, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Toy Web Bridge Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
        /********************************
        * Websocket Monitoring
        ********************************/
        /// <summary>
        /// Periodically checks if the websocket is still connected. If not, reconnect.
        /// </summary>
        /// <param name="state"></param>
        private async void MonitorWebsocket(object sender)
        {
            if (client.Connected)
                return;

            await Disconnect();
            await Connect();
        }
        /********************************
        * Buttplug Client Events
        ********************************/
        void OnErrorReceived(object s, ButtplugExceptionEventArgs args)
        {
            _logger.LogError($"Stuff fucked up! '{0}'", args.Exception.Message);
        }
        void OnPingTimeout(object s, EventArgs args)
        {
            //FIXME Someday
        }
        void OnScanningFinished(object s, EventArgs args)
        {
            _logger.LogInformation("Scan complete.");

            isScanning = false;
        }
        void OnServerDisconnect(object s, EventArgs args)
        {
            _logger.LogInformation("Disconnected from Intiface.");

            isScanning = false;
            Disconnect().Wait();
        }
        /********************************
        * Websocket Connect/Disconnect
        ********************************/
        public async Task Connect()
        {
            if (client.Connected)
                return;

            _logger.LogInformation("Connecting...");

            try
            {
                await client.ConnectAsync(new ButtplugWebsocketConnectorOptions(new Uri(_websocket_url)));
            }
            catch (ButtplugException)
            {
                _logger.LogError("Connection failed.");
                return;
            }

            _logger.LogInformation("Connected to intiface!");

            await StartScanning();
        }
        public async Task Disconnect()
        {
            if (!client.Connected)
                return;

            await StopScanning();
            await client.DisconnectAsync();
        }
        /********************************
        * Etc, etc...
        ********************************/
        async Task StartScanning()
        {
            if (isScanning)
                return;

            isScanning = true;

            if (!client.Connected)
                return;

            _logger.LogInformation("Scanning for devices...");

            await client.StartScanningAsync();
        }
        async Task StopScanning()
        {
            if (!isScanning)
                return;

            isScanning = false;

            if (!client.Connected)
                return;

            _logger.LogInformation("Stop Scanning...");

            await client.StopScanningAsync();
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
