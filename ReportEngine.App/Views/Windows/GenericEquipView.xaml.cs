using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities.BaseEntities;
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
        }
    }
}
