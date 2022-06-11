using BTQuickie.ViewModels;

namespace BTQuickie.Views;

public partial class TaskbarIconView
{
    public TaskbarIconView(TaskbarIconViewModel taskbarIconViewModel)
    {
        InitializeComponent();
        DataContext = taskbarIconViewModel;
        TaskbarIcon.ForceCreate();
    }
}