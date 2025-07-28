using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.ViewModels;
using ReportEngine.App.Views.Windows;
using ReportEngine.Domain.Entities.BaseEntities;
using ReportEngine.Domain.Repositories.Interfaces;
using System.Windows;

namespace ReportEngine.App.Services
{
    public class GenericEquipWindowFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public GenericEquipWindowFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Window CreateWindow<TEquip>() where TEquip : BaseEquip
        {
            var repository = _serviceProvider.GetRequiredService<IGenericBaseRepository<TEquip>>();
            var viewModel = new GenericEquipViewModel<TEquip>(repository);

            return new GenericEquipView(viewModel as GenericEquipViewModel<BaseEquip>);
        }
    }
}
