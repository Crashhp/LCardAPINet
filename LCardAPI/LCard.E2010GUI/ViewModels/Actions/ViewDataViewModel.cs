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
using System.Windows.Threading;
using Autofac;
using Caliburn.Micro;
using DynamicDataDisplay;
using LCard.API.Interfaces;
using LCard.E2010GUI.Startup;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace LCard.E2010GUI.ViewModels.Actions
{
    public class ViewDataViewModel : PropertyChangedBase
    {
        #region Graph Data
        private Dispatcher _dispatcherUIThread = Dispatcher.FromThread(Thread.CurrentThread);

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


        private ObservableDataSource<Point>[] grapDataLine = new ObservableDataSource<Point>[]
        {
            new ObservableDataSource<Point>(),
            new ObservableDataSource<Point>(),
            new ObservableDataSource<Point>(),
            new ObservableDataSource<Point>()
        };
        public ObservableDataSource<Point> GrapDataLine1
        {
            get { return grapDataLine[0]; }
            set
            {
                grapDataLine[0] = value;
                NotifyOfPropertyChange(() => GrapDataLine1);
            }
        }

        public ObservableDataSource<Point> GrapDataLine2
        {
            get { return grapDataLine[1]; }
            set
            {
                grapDataLine[1] = value;
                NotifyOfPropertyChange(() => GrapDataLine2);
            }
        }

        
        public ObservableDataSource<Point> GrapDataLine3
        {
            get { return grapDataLine[2]; }
            set
            {
                grapDataLine[2] = value;
                NotifyOfPropertyChange(() => GrapDataLine3);
            }
        }

        public ObservableDataSource<Point> GrapDataLine4
        {
            get { return grapDataLine[3]; }
            set
            {
                grapDataLine[3] = value;
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

                    var e2020 = UnityConfig.GetConfiguredContainer().Resolve<IE2010>();
                    if (e2020.Inited)
                    {
                        if(e2020.OnData == null)
                            e2020.OnData += OnData;
                        e2020.StartReadData();
                    } 
                    
                    NotifyAllToggle();
                }
            }
        }

        private void OnData(float[,] datas, int numberOfChannels, int dataSize, int numberBlock)
        {
            UpdateData(datas, numberOfChannels, dataSize, numberBlock);
            Console.WriteLine(datas.Length.ToString());
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

        

        public void UpdateData(float[,] datas, int numberOfChannels, int dataSize, int numberBlock)
        {
            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("block number = " + numberBlock);
                if((DateTime.Now - LastUpdateTime).TotalSeconds < 1.0)
                    return;
                LastUpdateTime = DateTime.Now;
                var pointLists = new List<Point>[4];
                for (var j = 0; j < 4; j++)
                {
                    grapDataLine[j] = new ObservableDataSource<Point>();
                    pointLists[j] = new List<Point>();
                }
                var stepOfPoint = 1000;
                for (var j = 0; j < numberOfChannels; j++)
                {
                    for (int i = 0; i < dataSize; i+= stepOfPoint)
                    {
                        pointLists[j].Add(new Point(i, Convert.ToDouble(datas[j,i])));
                    }
                }


                for (var j = 0; j < 4; j++)
                {
                    grapDataLine[j].AppendMany(pointLists[j]);
                }

                NotifyOfPropertyChange(() => GrapDataLine1);
                NotifyOfPropertyChange(() => GrapDataLine2);
                NotifyOfPropertyChange(() => GrapDataLine3);
                NotifyOfPropertyChange(() => GrapDataLine4);

            });
        }
    }
}
