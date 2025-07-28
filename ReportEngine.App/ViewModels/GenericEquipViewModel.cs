using ReportEngine.App.Commands;
using ReportEngine.App.Model;
using ReportEngine.Domain.Entities.BaseEntities;
using ReportEngine.Domain.Repositories.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ReportEngine.App.ViewModels
{
    public class GenericEquipViewModel<T> : BaseViewModel where T : BaseEquip
    {
        private readonly IGenericBaseRepository<T> _genericEquipRepository;
        public GenericEquipModel<T> GenericEquipModel { get; set; } = new GenericEquipModel<T>();

        public GenericEquipViewModel(IGenericBaseRepository<T> genericEquipRepository)
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
                GenericEquipModel.BaseEquips = new ObservableCollection<T>(items);
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
