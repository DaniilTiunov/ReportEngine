using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities.BaseEntities;
using ReportEngine.Shared.Config.DebugConsol;
using System.Diagnostics;
using System.Windows;

namespace ReportEngine.App.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для GenericEquipView.xaml
    /// </summary>
    public partial class GenericEquipView : Window 
    { 
        public GenericEquipView(GenericEquipViewModel<BaseEquip> viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            Loaded += (s, e) => {
                DebugConsole.WriteLine($"DataContext type: {DataContext?.GetType().Name}");
            };
        }
    }
}
