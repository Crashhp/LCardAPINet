using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Autofac;
using LCard.API.Interfaces;
using LCard.Core.Interfaces;
using LCard.Core.Poco;
using LCard.Manager.Startup;
using ZedGraph;

namespace LCard.Manager.ViewModels
{
    class DataViewModel : ViewModelBase
    {
        private readonly WindowsFormsHost windowsFormsHostGrapData;
        private readonly ZedGraphControl zedGraphControlData;
        private readonly IDataService dataService;
        /// <summary>
        /// Количество отображаемых точек
        /// </summary>
        private const int Capacity = 1000;

        /// <summary>
        /// Здесь храним данные
        /// </summary>
        private IList<RollingPointPairList> datas = new List<RollingPointPairList>();

        public DataViewModel(WindowsFormsHost windowsFormsHostGrapData)
        {
            for(int i = 0; i < 4; i++)
            {
                datas.Add(new RollingPointPairList(Capacity));
            }
            this.dataService = null;
            //this.dataService = UnityConfig.GetConfiguredContainer().Resolve<IDataService>();
            this.windowsFormsHostGrapData = windowsFormsHostGrapData;
            this.zedGraphControlData = new ZedGraphControl();
            this.windowsFormsHostGrapData.Child = zedGraphControlData;
            zedGraphControlData.Width = (int) this.windowsFormsHostGrapData.ActualWidth;
            zedGraphControlData.Margin = new Padding();
            DrawGraph();
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

        private void StartReadData()
        {
            var e2020 = UnityConfig.GetConfiguredContainer().Resolve<IE2010>();

            if (e2020.OnData == null)
                e2020.OnData += UpdateData;
            e2020.StartReadData();
        }

        public void UpdateData(DataPacketPoco dataPacket)
        {
            dataService?.AddPacket(dataPacket);

            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("block number = " + dataPacket.NumberBlock);
                if ((DateTime.Now - LastUpdateTime).TotalSeconds < 1.0)
                    return;

                LastUpdateTime = DateTime.Now;

                var stepOfPoint = 1000;
                for (var j = 0; j < dataPacket.NumberOfChannels; j++)
                {
                    for (int i = 0; i < dataPacket.DataSize; i += stepOfPoint)
                    {
                        Convert.ToDouble(dataPacket.Datas[j, i]);
                    }
                }



            });
        }

        #endregion

        private double f(double x)
        {
            if (x == 0)
            {
                return 1;
            }

            return Math.Sin(x) / x;
        }

        private void DrawGraph()
        {
            // Получим панель для рисования
            GraphPane pane = zedGraphControlData.GraphPane;

            // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
            pane.CurveList.Clear();

            // Создадим список точек
            PointPairList list = new PointPairList();

            double xmin = -50;
            double xmax = 50;

            // Заполняем список точек
            for (double x = xmin; x <= xmax; x += 0.01)
            {
                // добавим в список точку
                list.Add(x, f(x));
            }

            // Создадим кривую с названием "Sinc", 
            // которая будет рисоваться голубым цветом (Color.Blue),
            // Опорные точки выделяться не будут (SymbolType.None)
            LineItem myCurve = pane.AddCurve("Sinc", list, Color.Blue, SymbolType.None);

            // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
            // В противном случае на рисунке будет показана только часть графика, 
            // которая умещается в интервалы по осям, установленные по умолчанию
            zedGraphControlData.AxisChange();

            // Обновляем график
            zedGraphControlData.Invalidate();
        }
    }
}
