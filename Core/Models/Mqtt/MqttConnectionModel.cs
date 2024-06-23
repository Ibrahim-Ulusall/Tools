using Core.Interfaces;

namespace Core.Models.Mqtt;
public class MqttConnectionModel : IModel
{
    public string? Host { get; set; } = null!;
    public int Port { get; set; } = 1883; // Default PORT
    public string Username { get; set; } = null!;
    public string? Password { get; set; }
    public int ReTryCount { get; set; } = 5;
    public bool WithCleanSession { get; set; } = true;
    public int KeepAlivePeriod { get; set; } = 60; // Seconds

}
