using Applications.SignalR.Interfaces;
using Core.Enums;
using Core.Messages;
using Core.Models.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace Applications.SignalR.Microsoft;
public class SignalRService : ISignalRService, IDisposable
{
    private readonly SignalRConnectionModel connection;
    private readonly ILogger<SignalRService> logger;
    private HubConnection hub = null!;
    public SignalRService(IConfiguration configuration, ILogger<SignalRService> logger)
    {
        connection = configuration.GetSection(nameof(Sections.SignalR)).Get<SignalRConnectionModel>()
            ?? throw new ArgumentNullException(ErrorMessage.SignalRConnectionNotFound);
        this.logger = logger;
    }
    public void Dispose()
    {
        hub.DisposeAsync().GetAwaiter().GetResult();
    }

    public async Task InitalizeSignalR()
    {
        hub = new HubConnectionBuilder()
         .WithUrl(connection.Hostname)
         .WithAutomaticReconnect(new[] { TimeSpan.FromSeconds(connection.ReConnectDelays) })
         .WithServerTimeout(TimeSpan.FromSeconds(connection.ServerTimeoutSeconds))
         .WithStatefulReconnect()
         .WithKeepAliveInterval(TimeSpan.FromSeconds(connection.KeepAliveIntervalSeconds))
         .Build();

        if (hub.State != HubConnectionState.Connected)
        {
            await hub.StartAsync();
        }
        hub.Closed += Closed;

    }

    private Task Closed(Exception? arg)
    {
        logger.LogWarning(WarningMessage.SingalRConnectionClosed);
        return Task.CompletedTask;
    }
}
