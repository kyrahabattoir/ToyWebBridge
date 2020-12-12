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
    enum ConnState { NO, CONNECTING, YES }
    public class WebsocketService : IHostedService, IDisposable
    {
        private readonly ILogger<WebsocketService> _logger;
        private readonly BridgeSettings _settings;
        private Timer _timer;
        private ConnState isConnected = ConnState.NO;
        private bool isScanning;

        private DeviceRegister Register { get; }
        private ButtplugClient client;
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
            _logger.LogInformation("ButtplugWebsocket Service started.");

            if (_settings.SecretKey == string.Empty)
            {
                _settings.SecretKey = KeyGenerator.GetUniqueKey(20);
                _logger.LogWarning($"\n /!\\ Web bridge (generated) SecretKey: " + _settings.SecretKey + " /!\\\n");
            }
            else
                _logger.LogWarning($"\n /!\\ Web bridge SecretKey: " + _settings.SecretKey + " /!\\\n");

            _timer = new Timer(MonitorWebsocket, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ButtplugWebsocket Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        /********************************
        * Websocket Monitoring
        ********************************/
        private async void MonitorWebsocket(object state)
        {
            if (client != null)
            {
                if (isConnected != ConnState.NO) return;

                await Disconnect();
            }
            await Connect();
        }
        /********************************
        * Buttplug Client Events
        ********************************/
        void OnDisconnected()
        {
            _logger.LogInformation("Disconnected from Intiface.");
            Register.RemoveAllDevices();

            isConnected = ConnState.NO; //We are alreadyn disconnected, skip,disconnect process.
            isScanning = false;
            Disconnect().Wait();
        }
        void OnScanFinished()
        {
            _logger.LogInformation("Scan complete.");
            isScanning = false;
        }
        /********************************
        * Websocket Connect/Disconnect
        ********************************/
        public async Task Connect()
        {
            if (isConnected != ConnState.NO) return;

            _logger.LogInformation("Connecting...");

            client = new ButtplugClient("Simple HTTP Bridge");
            isConnected = ConnState.CONNECTING;
            try
            {
                await client.ConnectAsync(new ButtplugWebsocketConnectorOptions(new Uri("ws://localhost:12345/buttplug")));
            }
            catch (ButtplugException)
            {
                _logger.LogError("Connection failed.");
                isConnected = ConnState.NO;
                client.Dispose();
                client = null;
                return;
            }
            isConnected = ConnState.YES;
            _logger.LogInformation("Connected to intiface!");

            client.DeviceAdded += (aObj, args) => Register.AddDevice(args.Device);
            client.DeviceRemoved += (aObj, args) => Register.RemoveDevice(args.Device);
            client.ErrorReceived += (aObj, args) => _logger.LogError($"Stuff fucked up {args.Exception.Message}", args.Exception);
            client.ScanningFinished += (aObj, args) => OnScanFinished();
            client.ServerDisconnect += (aObj, args) => OnDisconnected();

            await StartScanning();
        }
        public async Task Disconnect()
        {
            if (client != null)
            {
                if (isConnected == ConnState.YES)
                    await StopScanning();

                if (isConnected != ConnState.NO)
                    await client.DisconnectAsync();

                client.Dispose();
                client = null;
            }
            isScanning = false;
            isConnected = ConnState.NO;
        }
        /********************************
        * Etc, etc...
        ********************************/
        async Task StartScanning()
        {
            if (isScanning) return;
            isScanning = true;

            if (client == null) return;

            _logger.LogInformation("Scanning for devices...");
            await client.StartScanningAsync();
        }
        async Task StopScanning()
        {
            if (!isScanning) return;
            isScanning = false;

            if (client == null) return;

            _logger.LogInformation("Stop Scanning...");
            await client.StopScanningAsync();
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
