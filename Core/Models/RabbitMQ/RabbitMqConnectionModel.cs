namespace Core.Models.RabbitMQ;
public class RabbitMqConnectionModel
{
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string Hostname { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public int ReTryCount { get; set; }
    public int RequestedHeartbeat { get; set; } = 60; // SECONDS
    public int NetworkRecoveryInterval { get; set; } = 5; // SECONDS
    public bool AutomaticRecoveryEnabled { get; set; } = true;
    public bool TopologyRecoveryEnabled { get; set; } = true;
    public bool Ssl { get; set; } = false;
    public string VirtualHost { get; set; } = "/";
}
