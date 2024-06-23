using Applications.Ftp.Interfaces;
using Core.Enums;
using Core.Messages;
using Core.Models.FTP;
using FluentFTP;
using FluentFTP.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using System.Net;
using System.Net.Sockets;

namespace Applications.Ftp.FluentFTP;
public class FtpService : IFtpService, IDisposable
{
    private object _lock = new object();
    private readonly FtpConnectionModel connection;
    private readonly ILogger<FtpService> logger;
    private FtpClient ftpClient = null!;
    public FtpService(IConfiguration configuration, ILogger<FtpService> logger)
    {
        connection = configuration.GetSection(nameof(Sections.FTP)).Get<FtpConnectionModel>() ?? throw new ArgumentNullException(ErrorMessage.FtpConfigurationNotFound);
        this.logger = logger;
        InitalizeFtp();
    }

    public void InitalizeFtp()
    {
        NetworkCredential networkCredential = new()
        {
            UserName = connection.Username,
            Password = connection.Password
        };

        ftpClient = new FtpClient()
        {
            Host = connection.Host,
            Port = connection.Port,
            Credentials = networkCredential,
        };

        lock (_lock)
        {
            Policy policy = Policy.Handle<SocketException>().Or<FtpException>()
           .WaitAndRetry(connection.ReTryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (execption, time) =>
           {
               logger.LogError($"{ErrorMessage.FtpConnectionException}. Hata : {execption.Message}");
           });
            policy.Execute(() =>
            {
                ftpClient.Connect();

            });
            if (!ftpClient.IsConnected)
                logger.LogError(ErrorMessage.FtpConnectionFail);
            else
                logger.LogInformation(SuccessMessage.FtpConnectionSuccessfully);

        }
    }

    public void Dispose()
    {
        ftpClient.Dispose();
    }
}
