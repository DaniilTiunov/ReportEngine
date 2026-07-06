using System.Windows.Controls;
using ReportEngine.App.LLM.ViewModels;

namespace ReportEngine.App.LLM;

public partial class ChatWithAi : UserControl
{
    public ChatWithAi(ChatWithAiViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
