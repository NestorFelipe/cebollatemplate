
using Application.Contracts.Commons;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Npgsql;
using System.Text;

namespace Infraestructure.Persistence.Logs;

public class AppLogger<T> : IAppLogger<T> where T : class
{
    public string Info(string message)
    {
        throw new NotImplementedException();
    }
    public string Log(string message)
    {
        throw new NotImplementedException();
    }
    public string Warn(string message)
    {
        throw new NotImplementedException();
    }
    public string Register(Exception e)
    {
        var vMessage = new StringBuilder();
        Guid myGuid = Guid.NewGuid();

        switch (e)
        {
            case NpgsqlException npgsqlException:
                vMessage.AppendFormat(MessageException.NpgsqlException, myGuid, npgsqlException.SqlState);
                break;

            case Exception exception:
                vMessage.AppendFormat(MessageException.Exception, myGuid);
                break;
            default:
                break;
        }

        var Logger = new NLogLoggerFactory().CreateLogger(typeof(T));
        Logger.Log(LogLevel.Error, e, $"Ticket : {myGuid}");


        return vMessage.ToString();
    }
}
