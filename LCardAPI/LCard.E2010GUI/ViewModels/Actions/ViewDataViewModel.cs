using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Autofac;
using Caliburn.Micro;
using DynamicDataDisplay;
using LCard.API.Interfaces;
using LCard.Core.Interfaces;
using LCard.Core.Poco;
using LCard.E2010GUI.Startup;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace LCard.E2010GUI.ViewModels.Actions
{
    public class ViewDataViewModel : PropertyChangedBase
    {
        public ViewDataViewModel()
        {
            _dataService = UnityConfig.GetConfiguredContainer().Resolve<IDataService>();
        }

        private volatile IDataService _dataService;

        #region Graph Data

        private long _lastUpdateTime;
        virtual public DateTime LastUpdateTime
        {
            get
            {
                var ticks = Interlocked.CompareExchange(ref _lastUpdateTime, 0, 0);
                return DateTime.FromBinary(ticks);
            }
            protected set
            {
                Interlocked.Exchange(ref _lastUpdateTime, value.Ticks);
            }
        }


        private ObservableDataSource<Point>[] _grapDataLine = {
            new ObservableDataSource<Point>(),
            new ObservableDataSource<Point>(),
            new ObservableDataSource<Point>(),
            new ObservableDataSource<Point>()
        };
        public ObservableDataSource<Point> GrapDataLine1
        {
            get { return _grapDataLine[0]; }
            set
            {
                _grapDataLine[0] = value;
                NotifyOfPropertyChange(() => GrapDataLine1);
            }
        }

        public ObservableDataSource<Point> GrapDataLine2
        {
            get { return _grapDataLine[1]; }
            set
            {
                _grapDataLine[1] = value;
                NotifyOfPropertyChange(() => GrapDataLine2);
            }
        }

        
        public ObservableDataSource<Point> GrapDataLine3
        {
            get { return _grapDataLine[2]; }
            set
            {
                _grapDataLine[2] = value;
                NotifyOfPropertyChange(() => GrapDataLine3);
            }
        }

        public ObservableDataSource<Point> GrapDataLine4
        {
            get { return _grapDataLine[3]; }
            set
            {
                _grapDataLine[3] = value;
                NotifyOfPropertyChange(() => GrapDataLine4);
            }
        }

        #endregion

        private bool _toggleButtonViewData = false;
        public bool ToggleButtonViewData
        {
            get { return _toggleButtonViewData; }
            set
            {
                if (!_toggleButtonViewData)
                {
                    _toggleButtonViewData = value;
                    _toggleButtonRecord = false;
                    _toggleButtonStop = false;
                    
                    NotifyAllToggle();

                    _dataService = null;
                    StartReadData();
                }
            }
        }

        private bool _toggleButtonRecord = false;
        public bool ToggleButtonRecord
        {
            get { return _toggleButtonRecord; }
            set
            {
                if (!_toggleButtonRecord)
                {
                    _toggleButtonViewData = false;
                    _toggleButtonRecord = value;
                    _toggleButtonStop = false;
                    NotifyAllToggle();

                    _dataService = UnityConfig.GetConfiguredContainer().Resolve<IDataService>();
                    StartReadData();
                }
            }
        }

        private bool _toggleButtonStop = true;
        public bool ToggleButtonStop
        {
            get { return _toggleButtonStop; }
            set
            {
                if (!_toggleButtonStop)
                {
                    _toggleButtonStop = value;
                    _toggleButtonViewData = false;
                    _toggleButtonRecord = false;
                    UnityConfig.GetConfiguredContainer().Resolve<IE2010>().StopReadData();

                    if (_dataService != null)
                    {
                        _dataService.InputRateInkHz = ModuleE2010.Default.InputRateInkHz;
                        _dataService?.WriteData();
                    }
                    
                    
                    
                    _dataService = null;
                }
                NotifyAllToggle();
            }
        }

        private void NotifyAllToggle()
        {
            NotifyOfPropertyChange(() => ToggleButtonStop);
            NotifyOfPropertyChange(() => ToggleButtonRecord);
            NotifyOfPropertyChange(() => ToggleButtonViewData);
        }

        private void StartReadData()
        {
            var e2020 = UnityConfig.GetConfiguredContainer().Resolve<IE2010>();

            if (e2020.OnData == null)
                e2020.OnData += UpdateData;
            e2020.StartReadData();
        }

        public void UpdateData(DataPacketPoco dataPacket)
        {
            _dataService?.AddPacket(dataPacket);

            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("block number = " + dataPacket.NumberBlock);
                if((DateTime.Now - LastUpdateTime).TotalSeconds < 1.0)
                    return;
                LastUpdateTime = DateTime.Now;
                var pointLists = new List<Point>[4];
                for (var j = 0; j < 4; j++)
                {
                    _grapDataLine[j] = new ObservableDataSource<Point>();
                    pointLists[j] = new List<Point>();
                }
                var stepOfPoint = 1000;
                for (var j = 0; j < dataPacket.NumberOfChannels; j++)
                {
                    for (int i = 0; i < dataPacket.DataSize; i+= stepOfPoint)
                    {
                        pointLists[j].Add(new Point(i, Convert.ToDouble(dataPacket.Datas[j,i])));
                    }
                }


                for (var j = 0; j < 4; j++)
                {
                    _grapDataLine[j].AppendMany(pointLists[j]);
                }

                NotifyOfPropertyChange(() => GrapDataLine1);
                NotifyOfPropertyChange(() => GrapDataLine2);
                NotifyOfPropertyChange(() => GrapDataLine3);
                NotifyOfPropertyChange(() => GrapDataLine4);

            });
        }
    }
}
