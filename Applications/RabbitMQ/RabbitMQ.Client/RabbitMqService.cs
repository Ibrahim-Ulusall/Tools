using Applications.RabbitMQ.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client;
using System.Net.Sockets;
using Core.Models.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Core.Messages;
using Core.Enums;

namespace Applications.RabbitMQ.RabbitMQ.Client;
public class RabbitMqService : IRabbitMQService, IDisposable
{

    private bool _disposed;
    private object lockObject = new object();
    private readonly RabbitMqConnectionModel connection = null!;
    private ILogger<RabbitMqService> _logger;
    private IConnection _connection = null!;
    public bool IsConnected => _connection != null && _connection.IsOpen;
    public IModel CreateModel() => _connection.CreateModel();

    public RabbitMqService(IConfiguration configuration, ILogger<RabbitMqService> logger)
    {
        connection = configuration.GetSection(nameof(Sections.RabbitMQ)).Get<RabbitMqConnectionModel>() ?? throw new ArgumentNullException(ErrorMessage.RabbitMQConnectionNotFound);
        _logger = logger;
    }

    public void InitalizeRabbitMQ()
    {

        lock (lockObject)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri($"amqp://{connection.Username}:{connection.Password}@{connection.Hostname}:{connection.Port}/"),
                RequestedHeartbeat = TimeSpan.FromSeconds(connection.RequestedHeartbeat),
                AutomaticRecoveryEnabled = connection.AutomaticRecoveryEnabled,
                TopologyRecoveryEnabled = connection.TopologyRecoveryEnabled,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(connection.NetworkRecoveryInterval),
                VirtualHost = connection.VirtualHost,
                Ssl =
                {
                    Enabled= connection.Ssl,
                }
            };

            var policy = Policy.Handle<SocketException>().Or<BrokerUnreachableException>()
                .WaitAndRetry(connection.ReTryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogError($"{ErrorMessage.FtpConnectionException}. Hata: {ex.Message}");
                });
            policy.Execute(() =>
            {
                _connection = connectionFactory.CreateConnection();
            });
            if (IsConnected)
            {
                _connection.ConnectionShutdown += ConnectionShutdown;
                _connection.ConnectionBlocked += ConnectionBlocked;
                _connection.CallbackException += CallbackException;
                _logger.LogInformation(SuccessMessage.RabbitMQConnectionSuccessfully);
            }
            else
            {
                _logger.LogError(ErrorMessage.RabbitMQConnectionFail);
            }
        }
    }

    private void CallbackException(object? sender, global::RabbitMQ.Client.Events.CallbackExceptionEventArgs e)
    {
        if (_disposed) return;
        InitalizeRabbitMQ();
    }

    private void ConnectionBlocked(object? sender, global::RabbitMQ.Client.Events.ConnectionBlockedEventArgs e)
    {
        if (_disposed) return;
        InitalizeRabbitMQ();
    }

    private void ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        if (_disposed) return;
        InitalizeRabbitMQ();
    }

    public void Dispose()
    {
        _disposed = true;
        _connection.Dispose();
    }
}
