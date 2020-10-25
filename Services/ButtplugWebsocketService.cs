using Buttplug.Client;
using Buttplug.Client.Connectors.WebsocketConnector;
using Buttplug.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ButtplugWebBridge.Services
{
    public class ButtplugWebsocketService : IHostedService, IDisposable
    {
        private readonly ILogger<ButtplugWebsocketService> _logger;
        private Timer _timer;

        private DeviceRegister Register { get; }
        private ButtplugWebsocketConnector connector;
        private ButtplugClient client;
        public ButtplugWebsocketService(ILogger<ButtplugWebsocketService> logger, DeviceRegister register)
        {
            _logger = logger;
            Register = register;
        }

        void OnScanFinished()
        {
            _logger.LogInformation("Scan finished.");
        }

        void OnDisconnected()
        {
            _logger.LogInformation("Disconnected from Intiface.");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ButtplugWebsocket Service running.");

            _timer = new Timer(MonitorWebsocket, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }
        private async void MonitorWebsocket(object state)
        {
            if (client != null)
            {
                if (client.Connected)
                    return;
                await CleanupAsync();
            }

            await ConnectAsync();
        }
        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ButtplugWebsocket Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        public async Task ConnectAsync()
        {
            _logger.LogInformation("ButtplugWebsocket Service is connecting...");

            connector = new ButtplugWebsocketConnector(new Uri("ws://localhost:12345/buttplug"));
            client = new ButtplugClient("Simple HTTP Bridge", connector);

            client.DeviceAdded += (aObj, args) => Register.AddDevice(args.Device);
            client.DeviceRemoved += (aObj, args) => Register.RemoveDevice(args.Device);
            client.ServerDisconnect += (aObj, args) => OnDisconnected();

            try
            {
                await client.ConnectAsync();

                client.ScanningFinished += (aObj, args) => OnScanFinished();
                client.Log += (aObj, args) => _logger.LogInformation($"Log: {args.Message}");
                client.ErrorReceived += (aObj, args) => _logger.LogError($"Stuff fucked up {args.Exception.Message}", args.Exception);
            }
            catch (ButtplugClientConnectorException ex)
            {
                // If our connection failed, because the server wasn't turned on,
                // SSL/TLS wasn't turned off, etc, we'll just print and exit
                // here. This will most likely be a wrapped exception.
                _logger.LogError(
                    $"Can't connect, exiting! Message: {ex.InnerException.Message}", ex);
                return;
            }
            catch (ButtplugHandshakeException ex)
            {
                // This means our client is newer than our server, and we need to
                // upgrade the server we're connecting to.
                _logger.LogError(
                     $"Handshake issue, exiting! Message: {ex.InnerException.Message}", ex);
                return;
            }

            _logger.LogInformation("Connected! Check Server for Client Name.");
            _logger.LogInformation("Starting to scan.");
            await client.StartScanningAsync();
        }
        public async Task CleanupAsync()
        {
            if (client != null && client.Connected)
            {
                await client.StopScanningAsync();
                await client.DisconnectAsync();
            }

            if (connector != null && connector.Connected)
            {
                await connector.DisconnectAsync();
            }
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
