namespace DispatcherDesktop.Domain.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Logger;
    using Model;
    using Models;
    using Newtonsoft.Json.Linq;
    using Utils;
    using Websocket.Client;

    public class DataTransferService : IDataTransferService
    {
        private const int MaxAttemptCount = 20;

        private const int MaxDevicesInConfig = 4;
        
        private readonly ILogger logger;

        private WebsocketClient websocketClient;

        private readonly HashSet<Device> cache = new HashSet<Device>();
        
        public DataTransferService(ILogger logger)
        {
            this.logger = logger;
        }

        public bool Connected => this.websocketClient?.IsRunning ?? false;

        public async Task Connect(ConnectInfo connectInfo)
        {
            await this.ConnectInternal(connectInfo);
        }

        public async Task Disconnect()
        {
            if (!this.Connected)
            {
                this.websocketClient?.Dispose();
                this.websocketClient = null;
                return;
            }
            
            await this.websocketClient.Stop(WebSocketCloseStatus.NormalClosure, string.Empty);
            this.websocketClient.Dispose();
            this.websocketClient = null;
        }
        
        public void PushData(Device device)
        {
            if (!this.Connected)
            {
                return;
            }

            if (this.cache.Contains(device))
            {
                return;
            }

            this.cache.RemoveWhere(d => d.Address == device.Address);
            
            this.cache.Add(device);
            
            this.logger.LogInfo("data message sending");

            var registersJson = new JArray(device.Registers.Select(r => new JObject(
                new JProperty("address", r.Address),
                new JProperty("value", r.Value)
            )));
            
            var json = new JObject(
                new JProperty("type", "data"),
                new JProperty("address", device.Address),
                new JProperty("registers", registersJson)
            );
            
            var _ = Task.Run(() => this.websocketClient.Send(json.ToString()));
        }
        
        public void SendConfig(ICollection<DeviceDescription> devices)
        {
            if (!devices.Any())
            {
                return;
            }
            
            this.logger.LogInfo("Config requested");
            this.cache.Clear();

            var devicesChunks = devices
                .Where(d => d.Registers.Any())
                .ToList()
                .SplitByChunk(MaxDevicesInConfig)
                .ToList();

            var lastChunk = devicesChunks.Last();

            foreach (var chunk in devicesChunks)
            {
                var endOfMessage = chunk == lastChunk;
                
                var devicesJson = new JArray(
                    chunk.Select(d => new JObject(
                        new JProperty("address", d.Id.ToString()),
                        new JProperty("name", d.Name.ToString()),
                        new JProperty("registers", new JArray(
                            d.Registers.Select(r => new JObject(
                                new JProperty("name", r.Name),
                                new JProperty("address", r.IntegerAddress.ToString()),
                                new JProperty("writable", r.WriteAddress != null)
                            ))
                        ))
                    ))
                );
                
                var config = new JObject(
                    new JProperty("type", "config"),
                    new JProperty("endOfConfig", endOfMessage),
                    new JProperty("devices", devicesJson)
                );
            
                this.websocketClient.Send(config.ToString());
            
                this.logger.LogInfo("Config sent");
            }
        }
        
        public event Action<WriteRegister> WriteRequested;
        
        public event Action ConfigRequested;

        public void Dispose()
        {
            this.websocketClient?.Dispose();
        }
        
        private async Task ConnectInternal(ConnectInfo connectInfo, int attempt = 0)
        {
            if (this.Connected)
            {
                return;
            }

            this.websocketClient?.Dispose();

            this.websocketClient =
                new WebsocketClient(connectInfo.ServerUrl, this.CreateClient(connectInfo.Login, connectInfo.Password))
                {
                    ReconnectTimeout = TimeSpan.FromSeconds(65)
                };
            
            this.websocketClient.ReconnectionHappened.Subscribe(info =>
            {
                this.logger.LogInfo($"Reconnection happened, type: {info.Type}");
            });
            
            this.websocketClient.MessageReceived.Subscribe(this.HandleMessage);
            this.websocketClient.DisconnectionHappened.Subscribe(async info =>
            {
                if (info.Exception != null)
                {
                    if (this.websocketClient != null && attempt < MaxAttemptCount)
                    {
                        this.logger.LogInfo("attempting to connect...");
                        await this.ConnectInternal(connectInfo, attempt+1);
                    }
                    else
                    {
                        this.logger.LogError($"Disonnect happened, type: {info.Exception}");
                    }
                }
            });
            await this.websocketClient.Stop(WebSocketCloseStatus.NormalClosure, string.Empty);

            try
            {
                await this.websocketClient.Start();
            }
            catch
            {
                // ignore
            }
        }
        
        private void HandleMessage(ResponseMessage msg)
        {
            try
            {
                var json = JObject.Parse(msg.Text);

                var type = json.SelectToken("type").ToString();

                switch (type)
                {
                    case "config": 
                        this.ConfigRequested?.Invoke();
                        return;
                    case "write":
                        this.RequestWrite(json, msg.Text);
                        return;
                    case "ping":
                        this.SendPong();
                        return;
                }
            }
            catch
            {
                this.logger.LogError("ошибка сериализации JSON");
            }
        }

        private void SendPong()
        {
            var pong = new JObject(
                new JProperty("type", "pong")
            );
            
            var _ = Task.Run(() => this.websocketClient.Send(pong.ToString()));
        }

        private void RequestWrite(JObject json, string body)
        {
            var device = json.SelectToken("device").ToString();
            var register = json.SelectToken("register").ToString();
            var value = json.SelectToken("value").ToString();

            this.cache.RemoveWhere(d => d.Address == device);
            
            this.WriteRequested?.Invoke(new WriteRegister(device, register, value, success =>
            {
                if (success)
                {
                    var _ = Task.Run(() => this.websocketClient.Send(body));
                }
                else
                {
                    var errorMsg = new JObject(
                        new JProperty("type", "error"),
                        new JProperty("value", "write")
                    );
                    
                    var _ = Task.Run(() => this.websocketClient.Send(errorMsg.ToString()));
                }
            }));
        }
        
        private Func<Uri, CancellationToken, Task<WebSocket>> CreateClient(string login, string password)
        {
            var token = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{login}:{password}"));
            
            return async (uri, t) =>
            {
                var client = SystemClientWebSocket.CreateClientWebSocket();
                
                switch (client)
                {
                    case System.Net.WebSockets.Managed.ClientWebSocket managed:
                        managed.Options.SetRequestHeader("Authorization", $"Basic {token}");
                        await managed.ConnectAsync(uri, t);
                        
                        break;
                    case ClientWebSocket coreSocket:
                        coreSocket.Options.SetRequestHeader("Authorization", $"Basic {token}");
                        await coreSocket.ConnectAsync(uri, t);
                        
                        break;
                }
                
                return client;
            };
        }
    }
}