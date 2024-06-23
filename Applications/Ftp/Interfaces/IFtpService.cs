using Core.Utilities.Results;
using FluentFTP;

namespace Applications.Ftp.Interfaces;
public interface IFtpService
{
    void InitalizeFtp();
    IDataResult<ICollection<FtpListItem>> GetDirectories(string path = "/");
    IDataResult<ICollection<FtpListItem>> GetFiles(string path = "/");
    IDataResult<byte[]> DownloadFile(string filePath, string localPath, string remotePath);
}
