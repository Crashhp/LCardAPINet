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
        private readonly IObservableCollection<PropertyChangedBase> actions =
            new BindableCollection<PropertyChangedBase>();

        private bool _viewDataChecked = true;
        private bool _recordDataChecked = false;
        private bool _deviceSettingsChecked = false;
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
                    FrameMain = actions[0];
                }
                NotifyOfPropertyChange(() => ViewDataChecked);
            }
        }

        public bool RecordDataChecked
        {
            get { return _recordDataChecked; }
            set
            {
                if (value.Equals(_recordDataChecked)) return;
                _recordDataChecked = value;
                if (value)
                {
                    FrameMain = actions[1];
                }
                NotifyOfPropertyChange(() => RecordDataChecked);
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
                    FrameMain = actions[2];
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
                    FrameMain = actions[3];
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
            this.actions.Add(new ViewDataViewModel());
            this.actions.Add(new RecordDataViewModel());
            this.actions.Add(new DeviceSettingsViewModel());
            this.actions.Add(new DeviceInformationViewModel());
            FrameMain = actions[0];
        }
    }
}
