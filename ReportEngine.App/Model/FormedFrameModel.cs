using ReportEngine.App.Convert;
using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.BaseEntities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.Frame;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ReportEngine.App.Model
{
    public class FormedFrameModel : BaseViewModel
    {
        private ObservableCollection<FrameDetail> _frameDetails = new(); //Коллекции комплектующих
        private ObservableCollection<FrameRoll> _frameRolls = new();
        private ObservableCollection<PillarEqiup> _pillarEqiups = new();
        private ObservableCollection<FormedFrame> _allFrames = new();
        private ObservableCollection<DisplayedComponent> _displayedComponents = new(); //Коллекция для отображения комплектующих в UI
        private IBaseEquip _selectedComponentForAdd;
        private DisplayedComponent _selectedComponentInFrame;
        private float? _componentLength;

        private FormedFrame _selectedFrame = new(); //Свойства для выбранного элемента на UI
        private FormedFrame _newFrame = new(); // для создания новой рамы

        public ObservableCollection<DisplayedComponent> DisplayedComponents 
        { 
            get => _displayedComponents;
            set => Set(ref _displayedComponents, value);
        }
        public ObservableCollection<FormedFrame> AllFrames
        {
            get => _allFrames;
            set => Set(ref _allFrames, value);
        }
        public ObservableCollection<FrameDetail> FrameDetails
        {
            get => _frameDetails;
            set => Set(ref _frameDetails, value);
        }
        public ObservableCollection<FrameRoll> FrameRolls
        {
            get => _frameRolls;
            set => Set(ref _frameRolls, value);
        }
        public ObservableCollection<PillarEqiup> PillarEqiups
        {
            get => _pillarEqiups;
            set => Set(ref _pillarEqiups, value);
        }
        public IBaseEquip SelectedComponentForAdd // Выбранное комплектующее
        {
            get => _selectedComponentForAdd;
            set => Set(ref _selectedComponentForAdd, value);
        }
        public FormedFrame NewFrame // Новая рама
        {
            get => _newFrame;
            set => Set(ref _newFrame, value);
        }
        public DisplayedComponent SelectedComponentInFrame
        {
            get => _selectedComponentInFrame;
            set => Set(ref _selectedComponentInFrame, value);
        }
        public FormedFrame SelectedFrame // Выбранная рама
        {
            get => _selectedFrame;
            set
            {
                Set(ref _selectedFrame, value);
                UpdateDisplayedComponents();
            }
        }
        public float? ComponentLength
        {
            get => _componentLength;
            set => Set(ref _componentLength, value);
        }
        public FormedFrameModel()
        {
            SelectedFrame = new FormedFrame();
        }

        public FormedFrame CreateNewFrame()
        {

            return new FormedFrame
            {
                Name = NewFrame.Name,
                Weight = NewFrame.Weight,
                FrameType = NewFrame.FrameType,
                Depth = NewFrame.Depth,
                Width = NewFrame.Width,
                Height = NewFrame.Height,
                Designe = NewFrame.Designe,
            };
        }
        public void UpdateDisplayedComponents()
        {
            DisplayedComponents.Clear();
            if (SelectedFrame?.Components == null) return;

            foreach (var frameComponent in SelectedFrame.Components)
            {
                IBaseEquip component = frameComponent.ComponentType switch
                {
                    nameof(FrameDetail) => FrameDetails.FirstOrDefault(d => d.Id == frameComponent.ComponentId),
                    nameof(FrameRoll) => FrameRolls.FirstOrDefault(r => r.Id == frameComponent.ComponentId),
                    nameof(PillarEqiup) => PillarEqiups.FirstOrDefault(p => p.Id == frameComponent.ComponentId),
                    _ => null
                };
                if (component != null)
                {
                    var displayed = new DisplayedComponent { Component = component, Count = frameComponent.Count };
                    if (component is BaseFrame baseFrame && baseFrame.Measure == "м")
                        displayed.Length = frameComponent.Length ?? 0;
                    DisplayedComponents.Add(displayed);
                }
            }
        }
    }
}
