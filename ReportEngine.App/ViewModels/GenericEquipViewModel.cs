using ReportEngine.App.Commands;
using ReportEngine.App.Model;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Config.DebugConsol;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ReportEngine.App.ViewModels
{
    public class GenericEquipViewModel<T, TEquip> : BaseViewModel
        where T : IBaseEquip
        where TEquip : class, new()
    {
        private readonly IGenericBaseRepository<T, TEquip> _genericEquipRepository;
        public GenericEquipModel<T, TEquip> GenericEquipModel { get; set; } = new GenericEquipModel<T, TEquip>();

        public GenericEquipViewModel(IGenericBaseRepository<T, TEquip> genericEquipRepository)
        {
            InitializeCommands();

            _genericEquipRepository = genericEquipRepository;

        }

        public void InitializeCommands()
        {
            ShowAllEquipCommand = new RelayCommand(OnShowAllEquipCommandExecuted, CanShowAllEquipCommandExecute);
        }
        public ICommand ShowAllEquipCommand { get; set; }
        public bool CanShowAllEquipCommandExecute(object e) => true;
        public async void OnShowAllEquipCommandExecuted(object e)
        {
            try
            {
                var items = await _genericEquipRepository.GetAllAsync();

                var baseEquips = items.OfType<T>().ToList();
                GenericEquipModel.BaseEquips = new ObservableCollection<T>(baseEquips);

                DebugConsole.WriteLine(GenericEquipModel.BaseEquips.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public ICommand AddNewEquipCommand { get; set; }
        public ICommand RemoveEquipCommand { get; set; }
    }
}
