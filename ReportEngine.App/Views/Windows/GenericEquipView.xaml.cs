using ReportEngine.App.AppHelpers;
using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities.BaseEntities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using System.Windows;
using System.Windows.Input;

namespace ReportEngine.App.Views.Windows;

/// <summary>
///     Логика взаимодействия для GenericEquipView.xaml
/// </summary>
public partial class GenericEquipView : Window
{
    public GenericEquipView()
    {
        InitializeComponent();
    }

    private void SelectEquip_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        ExceptionHelper.SafeExecute( () =>
        {
            var type = DataContext.GetType();

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(GenericEquipViewModel<>))
            {
                dynamic vm = DataContext;
                vm.SelectCommand.Execute(null);
            }

        });
    }
}