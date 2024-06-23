using Core.Enums;

namespace Core.Messages;
public static class ErrorMessage
{
    public const string FtpConfigurationNotFound = $"appsettings.json dosyası içerisinde {nameof(Sections.FTP)} adında bir alan bulunamadı.";
    public const string FtpConnectionFail = "Ftp Bağlantısı kurulamadı.";
    public const string FtpConnectionException = "Ftp Bağlantısı kurulurken hata meydana geldi.";

    public const string MqttConnectionNotFound = $"appsettings.json dosyası içerisinde {nameof(Sections.Mqtt)} adında bir alan bulunamadı";
    public const string MqttConnectionFail = "Mqtt Bağlantısı kurulamadı.";
    public const string MqttConnectionException = "Mqtt Bağlantısı kurulurken hata meydana geldi.";

    public const string RabbitMQConnectionNotFound = $"appsettings.json dosyası içerisinde {nameof(Sections.RabbitMQ)} adında bir alan bulunamadı";
    public const string RabbitMQConnectionFail = "RabbitMQ Bağlantısı kurulamadı.";
    public const string RabbitMQConnectionException = "RabbitMQ Bağlantısı kurulurken hata meydana geldi.";
    
    public const string SignalRConnectionNotFound = $"appsettings.json dosyası içerisinde {nameof(Sections.SignalR)} adında bir alan bulunamadı";
    public const string SignalRConnectionFail = "SignalR Bağlantısı kurulamadı.";
    public const string SignalRConnectionException = "SignalR Bağlantısı kurulurken hata meydana geldi.";

    public const string PathNotFound = "Dizin Bulunamadı.";
    public const string FileNotFound = "Dosya Bulunamadı.";
}
