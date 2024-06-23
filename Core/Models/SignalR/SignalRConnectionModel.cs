namespace Core.Models.SignalR;
public class SignalRConnectionModel
{
    public string Hostname { get; set; } = null!;
    public int ReConnectDelays { get; set; } = 10; // From Seconds
    public int KeepAliveIntervalSeconds { get; set; } = 60; // From Seconds
    public int ServerTimeoutSeconds { get; set; } = 60; // From Seconds

}
