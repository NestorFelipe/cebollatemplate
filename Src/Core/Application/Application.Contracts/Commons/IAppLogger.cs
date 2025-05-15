namespace Application.Contracts.Commons;

public interface IAppLogger<T> where T : class
{
    string Register(Exception e);

    string Info(string message);

    string Warn(string message);

    string Log(string message);

}
