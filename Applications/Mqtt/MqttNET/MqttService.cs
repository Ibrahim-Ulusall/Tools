using Applications.Mqtt.Interfaces;
using Core.Enums;
using Core.Messages;
using Core.Models.Mqtt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Adapter;
using MQTTnet.Client;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using Polly;
using System.Net.Sockets;
using System.Text;

namespace Applications.Mqtt.MqttNET;
public abstract class MqttService : IMqttService, IDisposable
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

    public async Task<bool> PublishAsync(MqttRequestModel request)
    {
        MqttApplicationMessage message = new()
        {
            Topic = request.Topic,
            PayloadSegment = Encoding.UTF8.GetBytes(request.Payload),
            QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce,
        };
        
        await mqttClient.PublishAsync(message);
        return true;
    }

    public async Task<bool> PublishAsync(ICollection<MqttRequestModel> requests)
    {
        foreach (var request in requests)
            await PublishAsync(request);
        return true;
    }

    public async Task<bool> SubscribeAsync(string topic)
    {
        await mqttClient.SubscribeAsync(topic);
        return true;
    }
    public async Task<bool> SubscribeAsync(ICollection<string> topics)
    {
        List<MqttTopicFilter> topicFilters = new List<MqttTopicFilter>();
        foreach (var topic in topics)
        {
            topicFilters.Add(new MqttTopicFilter()
            {
                Topic = topic,
                QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce
            });
        }

        MqttClientSubscribeOptions subscribeOption = new MqttClientSubscribeOptions()
        {
            TopicFilters = topicFilters,
        };
        await mqttClient.SubscribeAsync(subscribeOption);
        return true;
    }

    public void ListenMqttMessage(Func<MqttApplicationMessageReceivedEventArgs, Task> handler)
    {
        mqttClient.ApplicationMessageReceivedAsync += async (e) =>
        {
            try
            {
                await handler(e);
            }
            catch (Exception ex)
            {
                logger.LogError($"Hata oluştu: {ex.Message}");
            }
        };
    }

    protected virtual void HandleApplicationMessage(MqttApplicationMessageReceivedEventArgs e)
    {
        
    }
    private Task ConnectedAsync(MqttClientConnectedEventArgs arg)
    {
        logger.LogInformation(SuccessMessage.MqttConnectionSuccessfully);
        return Task.CompletedTask;
    }

    private Task DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
    {
        logger.LogWarning(WarningMessage.MqttConnectionDisconnected);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        mqttClient.Dispose();
    }
}
