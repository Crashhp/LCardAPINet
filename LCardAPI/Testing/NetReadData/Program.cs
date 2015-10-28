using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LusbapiBridgeE2010;
using NetReadData.Lusb;
using NetReadData.Lusb.Data;

namespace NetReadData
{
    class Program
    {
        static void Main(string[] args)
        {
            var dllVersion = Lusbapi.GetDllVersion();
            Console.WriteLine("dllVersion = " + dllVersion);

            IntPtr instance = Lusbapi.CreateLInstance("e2010");
            Console.WriteLine("instance = " + instance);

            //Lusbapi
            var instanceE2010 = new LusbapiE2010();

            ushort i;
            // попробуем обнаружить модуль E20-10 в первых MAX_VIRTUAL_SLOtS_QUANTITY_LUSBAPI виртуальных слотах
            for (i = 0; i < Lusbapi.MAX_VIRTUAL_SLOTS_QUANTITY_LUSBAPI; i++) 
                if (instanceE2010.OpenLDevice(i) > 0) break;
            // что-нибудь обнаружили?
            if (i == Lusbapi.MAX_VIRTUAL_SLOTS_QUANTITY_LUSBAPI) Console.WriteLine(" Can't find any module E20-10 in first 127 virtual slots!");
            else Console.WriteLine(" OpenLDevice({0}) --> OK", i);

            // попробуем прочитать дескриптор устройства
            var ModuleHandle = instanceE2010.GetModuleHandleDevice();
            if (ModuleHandle.ToInt32() == Lusbapi.INVALID_HANDLE_VALUE) Console.WriteLine(" GetModuleHandle() --> Bad");
            else Console.WriteLine(" GetModuleHandle() --> OK");

            // прочитаем название модуля в обнаруженном виртуальном слоте
            var ModuleName = instanceE2010.GetModuleName();
            Console.WriteLine("ModuleName = " + ModuleName);
            // проверим, что это 'E20-10'

            var UsbSpeed = (LusbSpeed)instanceE2010.GetUsbSpeed();
            Console.WriteLine("UsbSpeed = " + UsbSpeed);

            //// Образ для ПЛИС возьмём из соответствующего ресурса штатной DLL библиотеки
            var loadModuleRes = Convert.ToBoolean(instanceE2010.LOAD_MODULE(""));
            Console.WriteLine("load mod res = " + loadModuleRes);

            // проверим загрузку модуля
            var testModuleRes = Convert.ToBoolean(instanceE2010.TEST_MODULE());
            Console.WriteLine("testModuleRes = " + testModuleRes);

            //// получим информацию из ППЗУ модуля
            var ModuleDescription = instanceE2010.GET_MODULE_DESCRIPTION();
            Console.WriteLine("Active = " + ModuleDescription);

            // получим текущие параметры работы АЦП
            M_ADC_PARS_E2010 ap = instanceE2010.GET_ADC_PARS();
            Console.WriteLine("adc_pars = " + ap);


            // установим желаемые параметры работы АЦП
            if ((char)ModuleDescription.Module.Revision == 'A')
                ap.IsAdcCorrectionEnabled = 0;		// запретим автоматическую корректировку данных на уровне модуля (для Rev.A)
            else
                ap.IsAdcCorrectionEnabled = 1;		// разрешим автоматическую корректировку данных на уровне модуля (для Rev.B и выше)
            ap.SynchroPars.StartSource = (ushort)INT_ADC_START.INT_ADC_START_E2010;						// внутренний старт сбора с АЦП
            //	ap.SynchroPars.StartSource = INT_ADC_START_WITH_TRANS_E2010;		// внутренний старт сбора с АЦП с трансляцией
            //	ap.SynchroPars.StartSource = EXT_ADC_START_ON_RISING_EDGE_E2010;	// внешний старт сбора с АЦП по фронту 
            ap.SynchroPars.SynhroSource = (ushort)INT_ADC_CLOCK.INT_ADC_CLOCK_E2010;						// внутренние тактовые импульсы АЦП
            //	ap.SynchroPars.SynhroSource = INT_ADC_CLOCK_WITH_TRANS_E2010;		// внутренние тактовые импульсы АЦП с трансляцией
            ap.SynchroPars.StartDelay = 0x0;									// задержка начала сбора данных в кадрах отсчётов (для Rev.B и выше)
            ap.SynchroPars.StopAfterNKadrs = 0x0;							// останов сбора данных через заданное кол-во кадров отсчётов (для Rev.B и выше)
            ap.SynchroPars.SynchroAdMode = (ushort)E2010_SYNC.NO_ANALOG_SYNCHRO_E2010;	// тип аналоговой синхронизации (для Rev.B и выше)
            //	ap.SynchroPars.SynchroAdMode = ANALOG_SYNCHRO_ON_HIGH_LEVEL_E2010;
            ap.SynchroPars.SynchroAdChannel = 0x0;							// канал аналоговой синхронизации (для Rev.B и выше)
            ap.SynchroPars.SynchroAdPorog = 0;								// порог аналоговой синхронизации в кодах АЦП (для Rev.B и выше)
            ap.SynchroPars.IsBlockDataMarkerEnabled = 0x0;				// маркирование начала блока данных (удобно, например, при аналоговой синхронизации ввода по уровню) (для Rev.B и выше)
            ap.ChannelsQuantity = (ushort)MODULE_CONTSANTS.ADC_CHANNELS_QUANTITY_E2010; 		// кол-во активных каналов
            // формируем управляющую таблицу 
            ap.ControlTable = new ushort[(ushort)MODULE_CONTSANTS.MAX_CONTROL_TABLE_LENGTH_E2010];
            for (i = 0x0; i < ap.ChannelsQuantity; i++)
            {
                ap.ControlTable[i] = i;
            }
            // частоту сбора будем устанавливать в зависимости от скорости USB
            // частота работы АЦП в кГц
            const double AdcRate = 5000.0;
            ap.AdcRate = AdcRate;
            int DataStep;
            // частота работы АЦП в кГц
            if (UsbSpeed == LusbSpeed.USB11_LUSBAPI)
            {
                ap.InterKadrDelay = 0.01;		// межкадровая задержка в мс
                DataStep = 256 * 1024;				// размер запроса
            }
            else
            {
                ap.InterKadrDelay = 0.0;		// межкадровая задержка в мс
                DataStep = 1024 * 1024;			// размер запроса
            }
            // сконфигурим входные каналы
            ap.InputRange = new ushort[(ushort)MODULE_CONTSANTS.ADC_CHANNELS_QUANTITY_E2010];
            ap.InputSwitch = new ushort[(ushort)MODULE_CONTSANTS.ADC_CHANNELS_QUANTITY_E2010];
            for (i = 0x0; i < (ushort)MODULE_CONTSANTS.ADC_CHANNELS_QUANTITY_E2010; i++)
            {
                ap.InputRange[i] = (ushort)ADC_INPUTV.ADC_INPUT_RANGE_3000mV_E2010;  	// входной диапазоне 3В
                ap.InputSwitch[i] = (ushort)ADC_INPUT.ADC_INPUT_SIGNAL_E2010;			// источник входа - сигнал
            }
            // передаём в структуру параметров работы АЦП корректировочные коэффициенты АЦП
            ap.AdcOffsetCoefs = new double[(ushort)MODULE_CONTSANTS.ADC_INPUT_RANGES_QUANTITY_E2010, (ushort)MODULE_CONTSANTS.ADC_CHANNELS_QUANTITY_E2010];
            ap.AdcScaleCoefs = new double[(ushort)MODULE_CONTSANTS.ADC_INPUT_RANGES_QUANTITY_E2010, (ushort)MODULE_CONTSANTS.ADC_CHANNELS_QUANTITY_E2010];
            for (i = 0x0; i < (ushort)MODULE_CONTSANTS.ADC_INPUT_RANGES_QUANTITY_E2010; i++)
                for (ushort j = 0x0; j < (ushort)MODULE_CONTSANTS.ADC_CHANNELS_QUANTITY_E2010; j++)
                {
                    // корректировка смещения
                    ap.AdcOffsetCoefs[i,j] = ModuleDescription.Adc.OffsetCalibration[j + i * (ushort)MODULE_CONTSANTS.ADC_CHANNELS_QUANTITY_E2010];
                    // корректировка масштаба
                    ap.AdcScaleCoefs[i,j] = ModuleDescription.Adc.ScaleCalibration[j + i * (ushort)MODULE_CONTSANTS.ADC_CHANNELS_QUANTITY_E2010];
                }

            // передадим требуемые параметры работы АЦП в модуль
            var resSET_ADC_PARS = instanceE2010.SET_ADC_PARS(ap, DataStep);
            Console.WriteLine("resSET_ADC_PARS = " + resSET_ADC_PARS);

            Console.WriteLine("Start read data");

            var resStop = instanceE2010.STOP_ADC();
            Console.WriteLine("resStop = "+ resStop);

            // делаем предварительный запрос на ввод данных
            M_IO_REQUEST_LUSBAPI[] buffers = new M_IO_REQUEST_LUSBAPI[2];
            for (i = 0; i < 2; i++)
            {
                buffers[i] = new M_IO_REQUEST_LUSBAPI();
                buffers[i].Buffer = new short[2 * DataStep];
            }
            instanceE2010.InitReading();
            var resStart = instanceE2010.START_ADC();
            Console.WriteLine("resStart = " + resStart);
            int index = 0;
            List<short> arrayData = new List<short>();
            int k = 0;
            if (resStart > 0)
            {
                for (i = 0; i < 10; i++)
                {
                    var resRead = instanceE2010.ReadDataSync(ref buffers[k]);
                    if (resRead == 1)
                    {
                        Console.WriteLine("Good Data "+ buffers[k].NumberOfWordsPassed + " " + buffers[k].Buffer[0]);
                        for (index = 0; index < DataStep; index+=4)
                        {
                            arrayData.Add(buffers[k].Buffer[index]);
                        }
                    }
                }

                k = (k + 1)%2;
            }

            instanceE2010.Dispose();


        }
    }
}
