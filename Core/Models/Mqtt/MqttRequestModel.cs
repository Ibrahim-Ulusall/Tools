namespace Core.Models.Mqtt;
public class MqttRequestModel
{
    public string Topic { get; set; } = null!;
    public string Payload { get; set; } = null!;
}
