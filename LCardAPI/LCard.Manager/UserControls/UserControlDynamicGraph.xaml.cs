using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Autofac;
using LCard.API.Interfaces;
using LCard.Core.Poco;
using LCard.Manager.Startup;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;

namespace LCard.Manager.UserControls
{
    public class MeasureModel
    {
        public DateTime DateTime { get; set; }
        public double Value { get; set; }
    }

    /// <summary>
    /// Interaction logic for UserControlDynamicGraph.xaml
    /// </summary>
    public partial class UserControlDynamicGraph : UserControl, INotifyPropertyChanged
    {
        private double _axisMax;
        private double _axisMin;
        private string nameChannel1;
        private string nameChannel2;
        private string nameChannel3;
        private string nameChannel4;
        private IDeviceManager deviceManager = UnityConfig.GetConfiguredContainer().Resolve<IDeviceManager>();
        private IE2010 e2010 = UnityConfig.GetConfiguredContainer().Resolve<IE2010>();
        private int intervalInMs = 150;
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
        private List<ChartValues<MeasureModel>> _channelValues = new List<ChartValues<MeasureModel>>(); 
        private List<LineSeries> _channelLines = new List<LineSeries>(); 
        private volatile DataPacketPoco _lastDataPacketPoco = null;

        public UserControlDynamicGraph()
        {
            InitializeComponent();

            var mapper = Mappers.Xy<MeasureModel>()
                .X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
                .Y(model => model.Value);           //use the value property as Y

            //lets save the mapper globally.
            Charting.For<MeasureModel>(mapper);


            //the values property will store our values array
            ValuesChannel1 = new ChartValues<MeasureModel>();
            ValuesChannel2 = new ChartValues<MeasureModel>();
            ValuesChannel3 = new ChartValues<MeasureModel>();
            ValuesChannel4 = new ChartValues<MeasureModel>();

            _channelValues.Add(ValuesChannel1);
            _channelValues.Add(ValuesChannel2);
            _channelValues.Add(ValuesChannel3);
            _channelValues.Add(ValuesChannel4);

            _channelLines.Add(LineSeriesChannel1);
            _channelLines.Add(LineSeriesChannel2);
            _channelLines.Add(LineSeriesChannel3);
            _channelLines.Add(LineSeriesChannel4);

            UpdateChannelNames();


            //lets set how to display the X Labels
            DateTimeFormatter = value => new DateTime((long)value).ToString("mm:ss");

            AxisStep = TimeSpan.FromSeconds(1).Ticks;
            SetAxisLimits(DateTime.Now);

            //The next code simulates data changes every 300 ms
            Timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(intervalInMs)
            };
            Timer.Tick += TimerOnTick;
            IsDataInjectionRunning = false;
            R = new Random();

            DataContext = this;

            e2010.OnData += OnDataE2010;
        }

        private void UpdateChannelNames()
        {
            NameChannel1 = deviceManager.Sensors[0].Name;
            NameChannel2 = deviceManager.Sensors[1].Name;
            NameChannel3 = deviceManager.Sensors[2].Name;
            NameChannel4 = deviceManager.Sensors[3].Name;
        }

        private void OnDataE2010(DataPacketPoco dataPacket)
        {
            LastUpdateTime = DateTime.UtcNow;
            _lastDataPacketPoco = dataPacket;
            UpdateChannelNames();
        }

        private List<double> GetCurrentChannelValues()
        {
            if (_lastDataPacketPoco != null)
            {
                var currentTime = DateTime.UtcNow;
                var interCadrDelayInMs = (double)1024 * 1024 / (deviceManager.mE2010.InputRateInKhz);
                int offset = (int)((currentTime - LastUpdateTime).TotalMilliseconds*
                             (_lastDataPacketPoco.DataSize / interCadrDelayInMs));
                if (offset < _lastDataPacketPoco.DataSize)
                {
                    var res = new List<double>();
                    for (var j = 0; j < _lastDataPacketPoco.NumberOfChannels; j++)
                    {
                        var value = _lastDataPacketPoco.Datas[j, offset];
                        res.Add(value);
                    }
                    return res;
                }
            }
            
            return null;
        }

        public ChartValues<MeasureModel> ValuesChannel1 { get; set; }
        public ChartValues<MeasureModel> ValuesChannel2 { get; set; }
        public ChartValues<MeasureModel> ValuesChannel3 { get; set; }
        public ChartValues<MeasureModel> ValuesChannel4 { get; set; }
        public Func<double, string> DateTimeFormatter { get; set; }

        public double AxisStep { get; set; }

        public double AxisMax
        {
            get { return _axisMax; }
            set
            {
                _axisMax = value;
                OnPropertyChanged("AxisMax");
            }
        }
        public double AxisMin
        {
            get { return _axisMin; }
            set
            {
                _axisMin = value;
                OnPropertyChanged("AxisMin");
            }
        }

        public string NameChannel1
        {
            get { return nameChannel1; }
            set
            {
                nameChannel1 = value;
                OnPropertyChanged("NameChannel1");
            }
        }

        public string NameChannel2
        {
            get { return nameChannel2; }
            set
            {
                nameChannel2 = value;
                OnPropertyChanged("NameChannel2");
            }
        }

        public string NameChannel3
        {
            get { return nameChannel3; }
            set
            {
                nameChannel3 = value;
                OnPropertyChanged("NameChannel3");
            }
        }

        public string NameChannel4
        {
            get { return nameChannel4; }
            set
            {
                nameChannel4 = value;
                OnPropertyChanged("NameChannel4");
            }
        }

        public DispatcherTimer Timer { get; set; }
        public bool IsDataInjectionRunning { get; set; }
        public Random R { get; set; }

        private void RunDataOnClick(object sender, RoutedEventArgs e)
        {
            if (IsDataInjectionRunning)
            {
                Timer.Stop();
                IsDataInjectionRunning = false;
            }
            else
            {
                Timer.Start();
                IsDataInjectionRunning = true;
            }
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            var now = DateTime.Now;
            var channleValues = GetCurrentChannelValues();

            if (channleValues != null)
            {
                ValuesChannel1.Add(new MeasureModel
                {
                    DateTime = now,
                    Value = channleValues[0]
                });

                ValuesChannel2.Add(new MeasureModel
                {
                    DateTime = now,
                    Value = channleValues[1]
                });

                ValuesChannel3.Add(new MeasureModel
                {
                    DateTime = now,
                    Value = channleValues[2]
                });

                ValuesChannel4.Add(new MeasureModel
                {
                    DateTime = now,
                    Value = channleValues[3]
                });

                SetAxisLimits(now);

                //lets only use the last 30 values
                if (ValuesChannel1.Count > 30) ValuesChannel1.RemoveAt(0);
                if (ValuesChannel2.Count > 30) ValuesChannel2.RemoveAt(0);
                if (ValuesChannel3.Count > 30) ValuesChannel3.RemoveAt(0);
                if (ValuesChannel4.Count > 30) ValuesChannel4.RemoveAt(0);
            }
        }

        private void SetAxisLimits(DateTime now)
        {
            AxisMax = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 100ms ahead
            AxisMin = now.Ticks - TimeSpan.FromSeconds(8).Ticks; //we only care about the last 8 seconds
        }

        public void EnabelChannel(int nChannel)
        {
            _channelLines[nChannel].Visibility = Visibility.Visible;
        }

        public void DisableChannel(int nChannel)
        {
            _channelLines[nChannel].Visibility = Visibility.Hidden;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
