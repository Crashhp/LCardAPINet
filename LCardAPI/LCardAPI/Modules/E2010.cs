using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCard.API.Data;
using LCard.API.Data.E2010;
using LCard.API.Interfaces;
using LCard.Core.Logger;
using LCard.Core.Poco;
using LusbapiBridgeE2010;

namespace LCard.API.Modules
{
    public class E2010 : IE2010
    {
        LusbapiE2010 _pModulE2010;
        private ILogger _logger;
        private M_ADC_PARS_E2010 _adcParsE2010;
        public Action<DataPacketPoco> OnData { get; set; }
        private int _dataStep;
        private volatile bool _needReadData = false;
        private Task _taskReadData;
        public bool Inited { get; set; }
        private volatile int _numberBlock;
        private volatile bool _parametersSetted;
        private bool _deviceOpened = false;
        public int DataStep { get; set; }

        public double InterKadrDelay
        {
            get { return (DataStep/_adcParsE2010.AdcRate + 1000); }
        }

        public void SetDigitalIn(bool[] values)
        {
            ushort ttl = 0x0;
            for (int bitIndex = 0; bitIndex < values.Length; bitIndex++)
            {
                int value=0;
                if (values[bitIndex])
                {
                    int mask = 1 << bitIndex;
                    value &= ~mask;
                    
                    value |= mask;
                    ttl |= (ushort)value;
                }
            }
            _pModulE2010.TTL_OUT(ttl);
        }

        public bool[] GetDigitalOut()
        {
            var res = new bool[16];
            ushort ttl = 0x0;
            _pModulE2010.TTL_IN(ref ttl);
            for (int bitIndex = 0; bitIndex < 16; bitIndex++)
            {
                var bit = (ttl & (1 << bitIndex)) != 0;
                res[bitIndex] = bit;
            }
            return res;
        }

        public void ENABLE_TTL_OUT(bool value)
        {
            _pModulE2010.ENABLE_TTL_OUT(Convert.ToInt32(value));
        }

        public E2010()
        {
            _pModulE2010 = new LusbapiE2010();
            _logger = Logger.Current;
            AdcRateInKhz = 100;
            DataStep = 1024*1024;
            InputRange = ADC_INPUTV.ADC_INPUT_RANGE_3000mV_E2010;
        }

        public double AdcRateInKhz { get; set; }
        public ADC_INPUTV InputRange { get; set; }

        public bool OpenLDevice()
        {
            if (_deviceOpened == false)
            {
                ushort i;
                // попробуем обнаружить модуль E20-10 в первых MAX_VIRTUAL_SLOtS_QUANTITY_LUSBAPI виртуальных слотах
                for (i = 0; i < Lusbapi.MAX_VIRTUAL_SLOTS_QUANTITY_LUSBAPI; i++)
                    if (_pModulE2010.OpenLDevice(i) > 0) break;
                // что-нибудь обнаружили?
                if (i == Lusbapi.MAX_VIRTUAL_SLOTS_QUANTITY_LUSBAPI)
                {
                    _logger.Error(" Can't find any module E20-10 in first 127 virtual slots!");
                    return _deviceOpened;
                }
                _deviceOpened = true;
                _logger.Info(" OpenLDevice({0}) --> OK");
            }
            return _deviceOpened;
        }

        public IntPtr GetModuleHandleDevice()
        {
            var moduleHandle = _pModulE2010.GetModuleHandleDevice();
            if (moduleHandle.ToInt32() == Lusbapi.INVALID_HANDLE_VALUE) _logger.Error(" GetModuleHandle() --> Bad");
            else _logger.Info(" GetModuleHandle() --> OK");
            return moduleHandle;
        }

        public string GetModuleName()
        {
            var moduleName = _pModulE2010.GetModuleName();
            _logger.Info(" GetModuleName() --> OK: "+ moduleName);
            return moduleName;
        }

        public LusbSpeed GetUsbSpeed()
        {
            return (LusbSpeed)_pModulE2010.GetUsbSpeed();
        }

        public bool LOAD_MODULE()
        {
            return Convert.ToBoolean(_pModulE2010.LOAD_MODULE(""));
        }

        public bool TEST_MODULE()
        {
            return Convert.ToBoolean(_pModulE2010.TEST_MODULE());
        }

        public M_MODULE_DESCRIPTION_E2010 GET_MODULE_DESCRIPTION()
        {
            return _pModulE2010.GET_MODULE_DESCRIPTION();
        }

        public M_ADC_PARS_E2010 GET_ADC_PARS()
        {
            return _pModulE2010.GET_ADC_PARS();
        }

        public void SET_ADC_PARS(M_ADC_PARS_E2010 AdcPars, int DataStep)
        {
            _parametersSetted = true;
            _adcParsE2010 = AdcPars;
            _dataStep = DataStep;
        }

        public M_MODULE_DESCRIPTION_E2010? Init()
        {
            Inited = true;
            M_MODULE_DESCRIPTION_E2010? res = null;
            if (!OpenLDevice()) return null;
            // попробуем прочитать дескриптор устройства
            IntPtr handle = GetModuleHandleDevice();

            // прочитаем название модуля в обнаруженном виртуальном слоте
            var moduleName = GetModuleName();
            // проверим, что это 'E20-10'
            if (moduleName != "E20-10")
            {
                _logger.Error("Incorrect Module Name");
                return res;
            }

            var usbSpeed = GetUsbSpeed();
            _logger.Info("UsbSpeed = " + usbSpeed);

            //// Образ для ПЛИС возьмём из соответствующего ресурса штатной DLL библиотеки
            var loadModuleRes = LOAD_MODULE();
            if (!loadModuleRes)
            {
                _logger.Info("LOAD_MODULE Bad: " + loadModuleRes);
                return res;
            }
            _logger.Info("LOAD_MODULE OK: " + loadModuleRes);

            // проверим загрузку модуля
            var testModuleRes = TEST_MODULE();
            _logger.Info("TEST_MODULE RES: " + testModuleRes);
            if (!testModuleRes)
            {
                return res;
            }

            //// получим информацию из ППЗУ модуля
            res = GET_MODULE_DESCRIPTION();
            _logger.Info("Active = " + res);

            return res;
        }


        public bool StartReadData()
        {

            if (!_needReadData)
            {
                if (!OpenLDevice()) return false;
                if (!_parametersSetted)
                    SetParameters();
                System.Threading.Thread.Sleep(500);
                _taskReadData = Task.Factory.StartNew(() =>
                {
                    // передадим требуемые параметры работы АЦП в модуль
                    int i;
                    var resSET_ADC_PARS = _pModulE2010.SET_ADC_PARS(_adcParsE2010, _dataStep);
                    _logger.Info("resSET_ADC_PARS = " + resSET_ADC_PARS);

                    _logger.Info("Start read data");

                    var resStop = _pModulE2010.STOP_ADC();
                    _logger.Info("resStop = " + resStop);

                    // делаем предварительный запрос на ввод данных
                    M_IO_REQUEST_LUSBAPI[] buffers = new M_IO_REQUEST_LUSBAPI[2];
                    for (i = 0; i < 2; i++)
                    {
                        buffers[i] = new M_IO_REQUEST_LUSBAPI();
                        buffers[i].Buffer = new short[2*_dataStep];
                    }
                    _pModulE2010.InitReading();
                    var resStart = _pModulE2010.START_ADC();
                    _logger.Info("resStart = " + resStart);
                    int index = 0;
                    int k = 0;
                    if (resStart > 0)
                    {
                        _needReadData = true;
                        _numberBlock = 0;
                        while (_needReadData)
                        {
                            var resRead = _pModulE2010.ReadDataSync(ref buffers[k]);
                            if (resRead == 1)
                            {
                                Task.Factory.StartNew(() =>
                                {
                                    if (OnData != null)
                                    {
                                        
                                        var data = Array.ConvertAll(buffers[k].Buffer, x => (float)x / short.MaxValue);
                                        int countData = data.Count() / 2;
                                        var datas = new float[4, countData / 4];
                                        int j = 0;
                                        for (var t = 0; t < countData; t += 4)
                                        {
                                            datas[0, j] = data[t];
                                            datas[1, j] = data[t + 1];
                                            datas[2, j] = data[t + 2];
                                            datas[3, j] = data[t + 3];
                                            j++;
                                        }
                                        var dataPacket = new DataPacketPoco
                                        {
                                            Datas = datas,
                                            NumberOfChannels = 4,
                                            DataSize = countData / 4,
                                            NumberBlock = _numberBlock,
                                            NumberOfWordsPassed = buffers[k].NumberOfWordsPassed,
                                            Timeout = buffers[k].TimeOut
                                        };

                                        if (_numberBlock > 0)
                                        {
                                            OnData(dataPacket);
                                        }
                                        
                                        _numberBlock++;
                                    }
                                });

                            }
                            k = (k + 1)%2;
                        }
                    }
                });
            }
            return true;
        }

        public void SetParameters()
        {
            if (!OpenLDevice()) return;
            if (!Inited) Init();

            var moduleDescription = GET_MODULE_DESCRIPTION();
            var ap = GET_ADC_PARS();

            int i, j;

            // установим желаемые параметры работы АЦП
            if ((char)moduleDescription.Module.Revision == 'A')
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
            ap.ChannelsQuantity = MODULE_CONSTANTS.ADC_CHANNELS_QUANTITY_E2010; 		// кол-во активных каналов
            // формируем управляющую таблицу 
            ap.ControlTable = new ushort[(ushort)MODULE_CONSTANTS.MAX_CONTROL_TABLE_LENGTH_E2010];
            for (i = 0x0; i < ap.ChannelsQuantity; i++)
            {
                ap.ControlTable[i] = (ushort)i;
            }
            // частоту сбора будем устанавливать в зависимости от скорости USB
            // частота работы АЦП в кГц
            double AdcRate = AdcRateInKhz;
            ap.AdcRate = AdcRate;
            // частота работы АЦП в кГц
            var usbSpeed = GetUsbSpeed();
            if (usbSpeed == LusbSpeed.USB11_LUSBAPI)
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
            ap.InputRange = new ushort[MODULE_CONSTANTS.ADC_CHANNELS_QUANTITY_E2010];
            ap.InputSwitch = new ushort[MODULE_CONSTANTS.ADC_CHANNELS_QUANTITY_E2010];
            for (i = 0x0; i < (ushort)MODULE_CONSTANTS.ADC_CHANNELS_QUANTITY_E2010; i++)
            {
                ap.InputRange[i] = (ushort)InputRange;  	// входной диапазон
                ap.InputSwitch[i] = (ushort)ADC_INPUT.ADC_INPUT_SIGNAL_E2010;			// источник входа - сигнал
            }
            // передаём в структуру параметров работы АЦП корректировочные коэффициенты АЦП
            ap.AdcOffsetCoefs = new double[MODULE_CONSTANTS.ADC_INPUT_RANGES_QUANTITY_E2010, MODULE_CONSTANTS.ADC_CHANNELS_QUANTITY_E2010];
            ap.AdcScaleCoefs = new double[MODULE_CONSTANTS.ADC_INPUT_RANGES_QUANTITY_E2010, MODULE_CONSTANTS.ADC_CHANNELS_QUANTITY_E2010];
            for (i = 0x0; i < MODULE_CONSTANTS.ADC_INPUT_RANGES_QUANTITY_E2010; i++)
                for (j = 0x0; j < MODULE_CONSTANTS.ADC_CHANNELS_QUANTITY_E2010; j++)
                {
                    // корректировка смещения
                    ap.AdcOffsetCoefs[i, j] = moduleDescription.Adc.OffsetCalibration[j + i * MODULE_CONSTANTS.ADC_CHANNELS_QUANTITY_E2010];
                    // корректировка масштаба
                    ap.AdcScaleCoefs[i, j] = moduleDescription.Adc.ScaleCalibration[j + i * MODULE_CONSTANTS.ADC_CHANNELS_QUANTITY_E2010];
                }

            // передадим требуемые параметры работы АЦП в модуль
            SET_ADC_PARS(ap, DataStep);
        }

        public bool IsDevicePluggedIn
        {
            get { return OpenLDevice(); }
        }

        public bool StopReadData()
        {
            if (!OpenLDevice()) return false;
            _needReadData = false;
            
            if (_taskReadData != null)
                _taskReadData.Wait();
            _pModulE2010.STOP_ADC();
            //_deviceOpened = false;
            //Inited = false;
            return true;
        }
    }
}
