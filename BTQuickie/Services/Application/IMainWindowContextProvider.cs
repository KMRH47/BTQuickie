namespace BTQuickie.Services.Application;

public interface IMainWindowContextProvider
{
    void Minimize();
    void Close();
    void Show();
}