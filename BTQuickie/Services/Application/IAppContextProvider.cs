namespace BTQuickie.Services.Application;

public interface IAppContextProvider
{
    void Minimize();
    void Close();
    void Show();
}