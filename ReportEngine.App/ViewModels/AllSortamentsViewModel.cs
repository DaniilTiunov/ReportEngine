using System.Windows.Controls;
using System.Windows.Data;
using ReportEngine.App.Display;
using ReportEngine.App.Model;
using ReportEngine.Domain.Entities.BaseEntities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.Pipes;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.ViewModels;

public class AllSortamentsViewModel<T> : BaseViewModel
    where T : class, IBaseEquip
{
    public AllSortamentsModel CurrentSortamentsModel { get; set; } = new();
    
    private readonly IGenericBaseRepository<T, T> _genericBaseRepository;

    public AllSortamentsViewModel(IGenericBaseRepository<T, T> genericBaseRepository)
    {
        _genericBaseRepository = genericBaseRepository;
    }

    private void GenerateDataGrid<T>(DataGrid dataGrid)
        where T : class, IBaseEquip
    {
        dataGrid.Columns.Clear();
        var properties = typeof(T).GetProperties();
        foreach (var property in properties)
        {
            if (property.Name == "Id")
                continue;

            DataGridColumn column = new DataGridTextColumn
            {
                Header = GenericEquipMapper.GetColumnName(property.Name),
                Binding = new Binding(property.Name)
            };
            
            if (property ==
                properties[properties.Length - 1])
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
        }
    }
}