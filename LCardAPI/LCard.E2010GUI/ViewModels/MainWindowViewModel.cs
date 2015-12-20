using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using Caliburn.Micro;
using LCard.E2010GUI.ViewModels.Actions;

namespace LCard.E2010GUI.ViewModels
{
    [Export(typeof(IShell))]
    public class MainWindowViewModel : Screen, IShell
    {
        private readonly IObservableCollection<PropertyChangedBase> _actions =
            new BindableCollection<PropertyChangedBase>();

        private bool _viewDataChecked = true;
        private bool _deviceSettingsChecked;
        private bool _deviceInformationChecked;

        private PropertyChangedBase _frameMain;

        public void TestButton()
        {
            Console.WriteLine("test data");
        }

        public string testLabel = "test label lib";

        public string TestLabel
        {
            get { return testLabel; }
            set
            {
                testLabel = value;
                NotifyOfPropertyChange(() => TestLabel);
            }
        }

        public bool ViewDataChecked
        {
            get { return _viewDataChecked; }
            set
            {
                if (value.Equals(_viewDataChecked)) return;
                _viewDataChecked = value;
                if (value)
                {
                    FrameMain = _actions[0];
                    //(FrameMain as ViewDataViewModel).UpdateData();
                }
                NotifyOfPropertyChange(() => ViewDataChecked);
            }
        }

        public bool DeviceSettingsChecked
        {
            get { return _deviceSettingsChecked; }
            set
            {
                if (value.Equals(_deviceSettingsChecked)) return;
                _deviceSettingsChecked = value;
                if (value)
                {
                    FrameMain = _actions[1];
                    
                }
                NotifyOfPropertyChange(() => DeviceSettingsChecked);
            }
        }

        public bool DeviceInformationChecked
        {
            get { return _deviceInformationChecked; }
            set
            {
                if (value.Equals(_deviceInformationChecked)) return;
                _deviceInformationChecked = value;
                if (value)
                {
                    FrameMain = _actions[2];
                    (FrameMain as DeviceInformationViewModel).LoadDeviceInfo();
                }
                NotifyOfPropertyChange(() => DeviceInformationChecked);
            }
        }

        public PropertyChangedBase FrameMain
        {
            get { return _frameMain; }
            set
            {
                if (value.Equals(_frameMain)) return;
                _frameMain = value;
                NotifyOfPropertyChange(() => FrameMain);
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            this._actions.Add(new ViewDataViewModel());
            this._actions.Add(new DeviceSettingsViewModel());
            this._actions.Add(new DeviceInformationViewModel());
            FrameMain = _actions[0];
        }
    }
}
