using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.ViewModels.Utils
{
    public class UIValidatorService
    {
        private readonly INotificationService _notificationService;
        private readonly IProjectInfoRepository _projectRepository;

        public UIValidatorService(
            IProjectInfoRepository projectRepository,
            INotificationService notificationService)
        {
            _notificationService = notificationService;
            _projectRepository = projectRepository;
        }


        //валидация номера обвязки
        public bool ValidateCorrectObvNN(int newObvNN)
        {
            var invalidNN = newObvNN < 1;

            if (invalidNN)
            {
                _notificationService.ShowError("Указанный № обвязки некорректен!");
                return false;
            }

            return true;
        }

        public bool ValidateFreeObvNN(ProjectViewModel projectViewModel, int newObvNN, bool excludeSelected)
        {
            var selectedStand = projectViewModel.CurrentProjectModel.SelectedStand;

            if (selectedStand == null)
                return false;

            var obvCollection = selectedStand.ObvyazkiInStand;
            var selectedObv = selectedStand.SelectedObvyazkaInStand;

            if (obvCollection == null)
                return true;

            var isAlreadyExist = true;

            if (!excludeSelected)
            {
                isAlreadyExist = obvCollection
                    .Any(obv => obv.NN == newObvNN);
            }
            else if (selectedObv != null)
            {
                isAlreadyExist = obvCollection
                    .Where(obv => obv != selectedObv)
                    .Any(obv => obv.NN == newObvNN);
            }

            if (isAlreadyExist)
            {
                _notificationService.ShowError("Указанный № обвязки уже существует!");
                return false;
            }

            return true;
        }

        public bool ValidateCorrectStandNN(int newStandNumber)
        {
            var invalidNN = newStandNumber < 1;

            if (invalidNN)
            {
                _notificationService.ShowError("Указанный № стенда некорректен!");
                return false;
            }

            return true;
        }

        public bool ValidateFreeStandNN(ProjectViewModel projectViewModel,int newStandNumber, bool excludeSelected)
        {
            var standsCollection = projectViewModel.CurrentProjectModel.Stands;
            var selectedStand = projectViewModel.CurrentProjectModel.SelectedStand;

            if (standsCollection == null)
                return true;

            var isAlreadyExist = true;

            if (!excludeSelected)
            {
                isAlreadyExist = standsCollection
                    .Any(stand => stand.Number == newStandNumber);
            }
            else if (selectedStand != null)
            {
                isAlreadyExist = standsCollection
                    .Where(stand => stand.Id != selectedStand.Id)
                    .Any(stand => stand.Number == newStandNumber);
            }

            if (isAlreadyExist)
            {
                _notificationService.ShowError("Указанный № стенда уже существует!");
                return false;
            }

            return true;
        }

        public bool ValidateCorrectProjNN(int newProjNumber)
        {
            var invalidNN = newProjNumber < 1;

            if (invalidNN)
            {
                _notificationService.ShowError("Указанный № проекта некорректен!");
                return false;
            }

            return true;
        }

        public async Task<bool> ValidateFreeProjNN(ProjectViewModel projectViewModel, int newProjNumber, bool excludeSelected)
        {
            var allProjects = await _projectRepository.GetAllAsync();

            if (allProjects == null)
                return true;

            var isAlreadyExist = true;

            if (!excludeSelected)
            {
                isAlreadyExist = allProjects
                    .Any(proj => proj.Number == newProjNumber);
            }
            else if (projectViewModel.CurrentProjectModel != null)
            {
                isAlreadyExist = allProjects
                    .Where(proj => proj.Id != projectViewModel.CurrentProjectModel.CurrentProjectId)
                    .Any(proj => proj.Number == newProjNumber);
            }

            if (isAlreadyExist)
            {
                _notificationService.ShowError("Указанный № проекта уже существует!");
                return false;
            }

            return true;
        }

        public bool ValidateProjectStatus(ProjectViewModel projectViewModel)
        {
            if (string.IsNullOrEmpty(projectViewModel.CurrentProjectModel.Status))
            {
                _notificationService.ShowError("Не указан статус проекта!");
                return false;
            }

            return true;
        }

        //проверяем кол-во указанных датчиков и датчиков в обвязке
        //попробовать не с SelectedObvyazka а с SelectedObvyazkaInStand
        public bool ValidateSensorsQuantityInNewObv(ProjectViewModel projectViewModel)
        {
            var selectedStand = projectViewModel.CurrentProjectModel.SelectedStand;

            //считаем кол-во датчиков, сравниваем с тем что в выбранной обвязке
            int newObvSensorsQuantity = 0;

            if (!string.IsNullOrEmpty(selectedStand.FirstSensorType))
                newObvSensorsQuantity++;

            if (!string.IsNullOrEmpty(selectedStand.SecondSensorType))
                newObvSensorsQuantity++;

            if (!string.IsNullOrEmpty(selectedStand.ThirdSensorType))
                newObvSensorsQuantity++;

            //смотрим кол-во датчиков в выбранной обвязке
            var sensorsQuantityInObv = projectViewModel.SelectedObvyazka.Sensor;

            var temp = projectViewModel.CurrentProjectModel.SelectedStand.SelectedObvyazkaInStand.Sensor;


            if (newObvSensorsQuantity > sensorsQuantityInObv)
            {
                _notificationService.ShowError("Количество выбранных датчиков превышает их количество в обвязке!");
                return false;
            }

            return true;
        }

        public bool ValidateSelectedStand(ProjectViewModel projectViewModel)
        {
            if (projectViewModel.CurrentProjectModel.SelectedStand == null)
            {
                _notificationService.ShowError("Не выбран стенд!");
                return false;
            }

            return true;
        }
    }
}

