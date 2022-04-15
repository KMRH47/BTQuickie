namespace BTQuickie.Services.MainWindow;

public interface IMainWindowContextProvider
{
    void Minimize();
    void Close();
    void Show();
    void Hide();
}