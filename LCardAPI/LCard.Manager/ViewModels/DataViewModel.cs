using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Autofac;
using LCard.API.Data.E2010;
using LCard.API.Interfaces;
using LCard.API.Modules;
using LCard.Core.Interfaces;
using LCard.Core.Poco;
using LCard.Manager.Properties;
using LCard.Manager.Startup;
using ZedGraph;

namespace LCard.Manager.ViewModels
{
    class DataViewModel : ViewModelBase
    {
        public RelayCommand ViewDataCommand { get; private set; }
        public RelayCommand WriteDataCommand { get; private set; }
        public RelayCommand StopCommand { get; private set; }
        public RelayCommand ChannelEnabledCommand { get; private set; }

        private readonly WindowsFormsHost windowsFormsHostGrapData;
        private readonly IDialogService dialogService;
        private readonly ZedGraphControl zedGraphControlData;
        private IDataService dataService;
        /// <summary>
        /// Количество отображаемых точек
        /// </summary>
        private int Capacity = 500;
        private const int NumberOfCahnnels = 4;

        /// <summary>
        /// Здесь храним данные
        /// </summary>
        private IList<RollingPointPairList> datas = new List<RollingPointPairList>();

        private IDeviceManager _deviceManager;


        public DataViewModel(WindowsFormsHost windowsFormsHostGrapData, IDialogService dialogService)
        {
            ViewDataCommand = new RelayCommand(_ => ViewData());
            WriteDataCommand = new RelayCommand(_ => WriteData());
            StopCommand = new RelayCommand(_ => Stop());
            ChannelEnabledCommand = new RelayCommand(_ => ChannelEnabled());
            Capacity = Settings.Default.BufferDisplayLength;
            for (int i = 0; i < NumberOfCahnnels; i++)
            {
                datas.Add(new RollingPointPairList(Capacity));
            }
            this.dataService = null;
            //
            this.windowsFormsHostGrapData = windowsFormsHostGrapData;
            this.dialogService = dialogService;
            this.zedGraphControlData = new ZedGraphControl();
            this.windowsFormsHostGrapData.Child = zedGraphControlData;
            Capacity = Convert.ToInt16(windowsFormsHostGrapData.ActualWidth)-1;
            PrepareGraph();
            CheckSettings();

            var e2020 = UnityConfig.GetConfiguredContainer().Resolve<IE2010>();
            this.dataService = null;
            if (e2020.OnData == null)
                e2020.OnData += UpdateData;
            StartDate = DateTime.UtcNow;

            _deviceManager = new DeviceManager();
            _deviceManager.IsBlockAdapter = Settings.Default.IsBlockAdapter;
            _deviceManager.mE2010 = e2020;

            if (!_deviceManager.mE2010.OpenLDevice())
            {
                this.windowsFormsHostGrapData.Visibility = Visibility.Hidden;
            }
            else
            {
                this.windowsFormsHostGrapData.Visibility = Visibility.Visible;
            }
            _deviceManager.StopDetectionLoop();
            LastUpdateTime = DateTime.MinValue;
            
        }

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

        private void ViewData()
        {
            if (CheckSettings())
            {
                _deviceManager.StopDetectionLoop();
                Thread.Sleep(3000);
                var e2020 = UnityConfig.GetConfiguredContainer().Resolve<IE2010>();
                if (e2020.Inited == false) e2020.Init();
                e2020.AdcRateInKhz = Settings.Default.InputRateInkHz;
                e2020.InputRange = (ADC_INPUTV)Settings.Default.InputRange;
                e2020.SetParameters();
                e2020.SetDigitalIn(new bool[16]);
                this.dataService = null;
                if (e2020.OnData == null)
                    e2020.OnData += UpdateData;
                StartDate = DateTime.UtcNow;
                e2020.StartReadData();
            }
        }

        private void WriteData()
        {
            if (CheckSettings())
            {
                _deviceManager.StopDetectionLoop();
                this.dataService = UnityConfig.GetConfiguredContainer().Resolve<IDataService>();
                var e2020 = UnityConfig.GetConfiguredContainer().Resolve<IE2010>();
                if (e2020.Inited == false) e2020.Init();
                e2020.AdcRateInKhz = Settings.Default.InputRateInkHz;
                e2020.InputRange = (ADC_INPUTV)Settings.Default.InputRange;
                e2020.SetParameters();
                e2020.SetDigitalIn(new bool[16]);
                if (e2020.OnData == null)
                    e2020.OnData += UpdateData;
                e2020.StartReadData();
            }
        }

        private async void Stop()
        {
            //if (CheckSettings())
            {
                {
                    
                    UnityConfig.GetConfiguredContainer().Resolve<IE2010>().StopReadData();

                    if (this.dataService != null)
                    {
                        this.windowsFormsHostGrapData.Visibility = Visibility.Hidden;
                        var controller =
                            await
                                this.dialogService.ShowProgressAsync("Запись данных",
                                    String.Format("Частота {0}, кГц; Число каналов: 4", Settings.Default.InputRateInkHz));

                        controller.SetCancelable(false);
                        this.dataService.InputRateInkHz = Settings.Default.InputRateInkHz;
                        await HotFixTask();
                        var action = new Action<int, int>(delegate(int progressValue, int maxProgress)
                        {
                            controller.SetProgress(Convert.ToDouble(progressValue)/Convert.ToDouble(maxProgress));
                        });
                        dataService?.WriteData(action,
                            new[]
                            {
                                Settings.Default.IsChannel1,
                                Settings.Default.IsChannel2,
                                Settings.Default.IsChannel3,
                                Settings.Default.IsChannel4
                            },
                            Settings.Default.SaveResultPath);
                        await controller.CloseAsync();
                        this.windowsFormsHostGrapData.Visibility = Visibility.Visible;

                    }
                    this.dataService = null;
                    _deviceManager.StartDetectionLoop();
                }
            }
        }
        private Task HotFixTask()
        {
            return Task.Delay(250);
        }

        private async void OnPublishCommand()
        {
            var controller = await this.dialogService.ShowProgressAsync("Запись данных", String.Format("Частота {0}, кГц; Число каналов: 4", Settings.Default.InputRateInkHz));
            controller.SetCancelable(true);
            int uploadCount = 0;
            var matches = new[] {"1", "2", "3", "4", "5"};
            foreach (var m in matches)
            {
                await UploadMatch(m);
                if (controller.IsCanceled) break;
                uploadCount++;
                controller.SetProgress((double)uploadCount / matches.Length);
            }
            await controller.CloseAsync();
            if (controller.IsCanceled)
                await dialogService.ShowMessageAsync("Publishing", "Publish cancelled");
            else
                await dialogService.ShowMessageAsync("Publishing", "Publish successful");
            this.windowsFormsHostGrapData.Visibility = Visibility.Visible;
            this.dataService = null;
        }

        private Task UploadMatch(string matchViewModel)
        {
            return Task.Delay(1250);
        }

        private DateTime StartDate = DateTime.UtcNow;
        public void UpdateData(DataPacketPoco dataPacket)
        {

            dataService?.AddPacket(dataPacket);

            //if ((DateTime.UtcNow - LastUpdateTime).TotalSeconds > 5)
            {
                Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("block number = " + dataPacket.NumberBlock);
                    //if ((DateTime.Now - LastUpdateTime).TotalSeconds < 0.1)
                    //    return;
                    Console.WriteLine("Timeout = " + dataPacket.Timeout);
                    Console.WriteLine("Timeout = " + (DateTime.UtcNow - StartDate).TotalMilliseconds);
                    Console.WriteLine("DataSize = " + dataPacket.DataSize);
                    StartDate = DateTime.UtcNow;
                    LastUpdateTime = DateTime.Now;
                    var interCadrDelay = (double) 1024*1024/(_deviceManager.mE2010.InputRateInKhz*1000.0);
                    var stepOfPoint = 2000;
                    var maxY = Double.MinValue;
                    var minY = Double.MaxValue;
                    var firstEnabledIndex = 3;
                    for (var j = 0; j < dataPacket.NumberOfChannels; j++)
                    {
                        if (IsChannelEnabled(j))
                        {
                            for (int i = 0; i < dataPacket.DataSize; i += stepOfPoint)
                            {
                                var value = dataPacket.Datas[j, i];
                                datas[j].Add(
                                    Convert.ToDouble((double) i/dataPacket.DataSize + dataPacket.NumberBlock)*
                                    interCadrDelay,
                                    dataPacket.Datas[j, i]);
                                maxY = Math.Max(value, maxY);
                                minY = Math.Min(value, minY);
                            }
                            firstEnabledIndex = Math.Min(firstEnabledIndex, j);
                        }
                    }
                    var graphPane = zedGraphControlData.GraphPane;
                    // Устанавливаем интересующий нас интервал по оси X

                    graphPane.XAxis.Scale.Min = datas[firstEnabledIndex][0].X;
                    graphPane.XAxis.Scale.Max = datas[firstEnabledIndex][datas[firstEnabledIndex].Count - 1].X +
                                                (graphPane.XAxis.Scale.Max - graphPane.XAxis.Scale.Min)/5.0;

                    // Устанавливаем интересующий нас интервал по оси Y
                    graphPane.YAxis.Scale.Min = minY - ((maxY - minY)*0.1 + 0.03);
                    graphPane.YAxis.Scale.Max = maxY + ((maxY - minY)*0.1 + 0.03);

                    int nChannel = 1;
                    foreach (var pane in zedGraphControlData.GraphPane.CurveList)
                    {
                        if (_deviceManager.Sensors[nChannel - 1] != null && _deviceManager.IsBlockAdapter)
                        {
                            pane.Label.Text = nChannel + " " + _deviceManager.Sensors[nChannel - 1].Name;
                        }
                        else
                        {
                            pane.Label.Text = "Канал - " + (nChannel);
                        }
                        nChannel++;
                    }

                    // Обновим оси
                    zedGraphControlData.AxisChange();

                    // Обновим сам график
                    zedGraphControlData.Invalidate();
                });

                LastUpdateTime = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Подготовка к отображению данных
        /// </summary>
        public void PrepareGraph()
        {
            windowsFormsHostGrapData.Visibility = SettingsViewModel.NumberSelectedChannels > 0 ? Visibility.Visible : Visibility.Hidden;
            // Получим панель для рисования
            var pane = zedGraphControlData.GraphPane;
                pane.Title.Text = "";
                pane.YAxis.Title.Text = "Напряжение, В";
                pane.XAxis.Title.Text = "Время, с";
                // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
                pane.CurveList.Clear();
                if (IsChannelEnabled(0))
                    pane.AddCurve("Канал " + 1, datas[0], Color.Red, SymbolType.None);
                if (IsChannelEnabled(1))
                    pane.AddCurve("Канал " + 2, datas[1], Color.Green, SymbolType.None);
                if (IsChannelEnabled(2))
                    pane.AddCurve("Канал " + 3, datas[2], Color.Blue, SymbolType.None);
                if (IsChannelEnabled(3))
                    pane.AddCurve("Канал " + 4, datas[3], Color.Indigo, SymbolType.None);

                // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
                zedGraphControlData.AxisChange();

                // Обновляем график
                zedGraphControlData.Invalidate();


        }

        private bool IsChannelEnabled(int nChannel)
        {
            CheckSettings();
            bool res = false;
            switch (nChannel)
            {
                case 0:
                    res = Settings.Default.IsChannel1;
                    break;
                case 1:
                    res = Settings.Default.IsChannel2;
                    break;
                case 2:
                    res = Settings.Default.IsChannel3;
                    break;
                case 3:
                    res = Settings.Default.IsChannel4;
                    break;
            }
            return res;
        }

        private bool CheckSettings()
        {
            if (SettingsViewModel.NumberSelectedChannels == 0)
            {
                Settings.Default.IsChannel1 = true;
                Settings.Default.Save();
            }
            return true;
        }


        private void ChannelEnabled_OnClick(object sender, RoutedEventArgs e)
        {
            if (
                Settings.Default.IsChannel1 == false &&
                Settings.Default.IsChannel2 == false &&
                Settings.Default.IsChannel3 == false &&
                Settings.Default.IsChannel4 == false)
            {

            }
            Settings.Default.Save();
        }

        private void ChannelEnabled()
        {
            Settings.Default.Save();
        }

        #endregion
    }
}
