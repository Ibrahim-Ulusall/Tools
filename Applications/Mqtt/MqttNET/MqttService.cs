using Applications.Mqtt.Interfaces;
using Core.Enums;
using Core.Messages;
using Core.Models.Mqtt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Adapter;
using MQTTnet.Client;
using Polly;
using System.Net.Sockets;

namespace Applications.Mqtt.MqttNET;
public class MqttService : IMqttService, IDisposable
{
    private object _lock = new object();
    private readonly MqttConnectionModel connection;
    private readonly ILogger<MqttService> logger;
    private IMqttClient mqttClient = null!;
    private MqttClientOptions options = null!;
    public MqttService(IConfiguration configuration, ILogger<MqttService> logger)
    {
        this.logger = logger;
        connection = configuration.GetSection(nameof(Sections.Mqtt)).Get<MqttConnectionModel>() ?? throw new ArgumentNullException(ErrorMessage.MqttConnectionNotFound);
    }
    public void InitalizeMqtt()
    {
        MqttFactory factory = new MqttFactory();

        mqttClient = factory.CreateMqttClient();

        string clientId = Guid.NewGuid().ToString();
        options = new MqttClientOptionsBuilder()
            .WithTcpServer(connection.Host).WithCredentials(connection.Username, connection.Password)
            .WithClientId(clientId).WithKeepAlivePeriod(TimeSpan.FromSeconds(connection.KeepAlivePeriod))
            .WithCleanSession(connection.WithCleanSession).Build();

        lock (_lock)
        {
            Policy policy = Policy.Handle<SocketException>().Or<MqttConnectingFailedException>()
            .WaitAndRetry(connection.ReTryCount, reTryAttempt => TimeSpan.FromSeconds(Math.Pow(2, reTryAttempt)), (exepction, time) =>
            {
                logger.LogError($"{ErrorMessage.MqttConnectionException}. Hata : {exepction.Message}");
            });

            policy.Execute(async () =>
            {
                await mqttClient.ConnectAsync(options);
            });

            if (!mqttClient.IsConnected)
                logger.LogError(ErrorMessage.MqttConnectionFail);

            mqttClient.ConnectedAsync += ConnectedAsync;
            mqttClient.DisconnectedAsync += DisconnectedAsync;
        }

    }

    private Task DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
    {
        logger.LogWarning(WarningMessage.MqttConnectionDisconnected);
        return Task.CompletedTask;
    }

    private Task ConnectedAsync(MqttClientConnectedEventArgs arg)
    {
        logger.LogInformation(SuccessMessage.MqttConnectionSuccessfully);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        mqttClient.Dispose();
    }
}
