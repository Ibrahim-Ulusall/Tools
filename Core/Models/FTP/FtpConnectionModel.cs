using Core.Interfaces;

namespace Core.Models.FTP;
public class FtpConnectionModel : IModel
{
    public string Host { get; set; } = null!;
    public int Port { get; set; } = 21; // Default PORT
    public string Username { get; set; } = null!;
    public string? Password { get; set; }
    public int ReTryCount { get; set; } = 5;
}
