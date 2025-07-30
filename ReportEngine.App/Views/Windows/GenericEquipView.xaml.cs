using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities.BaseEntities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Shared.Config.DebugConsol;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static MaterialDesignThemes.Wpf.Theme;

namespace ReportEngine.App.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для GenericEquipView.xaml
    /// </summary>
    public partial class GenericEquipView : Window 
    { 
        public GenericEquipView()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is GenericEquipViewModel<IBaseEquip, BaseEquip> viewModel)
            {
                GenerateDataGridColumns(viewModel);
            }
        }
        private void GenerateDataGridColumns(GenericEquipViewModel<IBaseEquip, BaseEquip> viewModel)
        {
            GenericEquipDataGrid.Columns.Clear();

            var itemType = typeof(BaseEquip);
            var properties = itemType.GetProperties();

            foreach (var property in properties)
            {
                DataGridColumn column;

                // Выбираем тип столбца в зависимости от типа свойства
                if (property.PropertyType == typeof(bool))
                {
                    column = new DataGridCheckBoxColumn
                    {
                        Header = property.Name,
                        Binding = new Binding(property.Name)
                    };
                }
                else
                {
                    column = new DataGridTextColumn
                    {
                        Header = property.Name,
                        Binding = new Binding(property.Name)
                    };
                }

                GenericEquipDataGrid.Columns.Add(column);
            }
        }

    }
}
