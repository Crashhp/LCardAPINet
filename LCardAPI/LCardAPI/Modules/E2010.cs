using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCard.API.Data;
using LCard.API.Data.E2010;
using LCard.API.Interfaces;
using LCard.Core.Logger;
using LusbapiBridgeE2010;

namespace LCard.API.Modules
{
    public class E2010 : IE2010
    {
        LusbapiE2010 _pModulE2010;
        private ILogger _logger;
        private M_ADC_PARS_E2010 _adcParsE2010;
        public Action<float[,],int,int,int> OnData { get; set; }
        private int _dataStep;
        private volatile bool _needReadData = false;
        private Task _taskReadData;
        public bool Inited { get; set; }
        private volatile int _numberBlock = 0;

        public E2010()
        {
            _pModulE2010 = new LusbapiE2010();
            _logger = Logger.Current;
        }

        public bool OpenLDevice()
        {
            ushort i;
            // попробуем обнаружить модуль E20-10 в первых MAX_VIRTUAL_SLOtS_QUANTITY_LUSBAPI виртуальных слотах
            for (i = 0; i < Lusbapi.MAX_VIRTUAL_SLOTS_QUANTITY_LUSBAPI; i++)
                if (_pModulE2010.OpenLDevice(i) > 0) break;
            // что-нибудь обнаружили?
            if (i == Lusbapi.MAX_VIRTUAL_SLOTS_QUANTITY_LUSBAPI)
            {
                _logger.Error(" Can't find any module E20-10 in first 127 virtual slots!");
                return false;
            }
            else
            {
                _logger.Info(String.Format(" OpenLDevice({0}) --> OK", i));
            }
            return true;
        }

        public IntPtr GetModuleHandleDevice()
        {
            IntPtr ModuleHandle = _pModulE2010.GetModuleHandleDevice();
            if (ModuleHandle.ToInt32() == Lusbapi.INVALID_HANDLE_VALUE) _logger.Error(" GetModuleHandle() --> Bad");
            else _logger.Info(" GetModuleHandle() --> OK");
            return ModuleHandle;
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
            _adcParsE2010 = AdcPars;
            _dataStep = DataStep;
        }

        public M_MODULE_DESCRIPTION_E2010? Init()
        {
            Inited = true;
            M_MODULE_DESCRIPTION_E2010? res = null;
            OpenLDevice();
            // попробуем прочитать дескриптор устройства
            IntPtr handle = GetModuleHandleDevice();

            // прочитаем название модуля в обнаруженном виртуальном слоте
            var ModuleName = GetModuleName();
            // проверим, что это 'E20-10'
            if (ModuleName != "E20-10")
            {
                _logger.Error("Incorrect Module Name");
                return res;
            }

            var UsbSpeed = GetUsbSpeed();
            _logger.Info("UsbSpeed = " + UsbSpeed);

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
                                        var data = Array.ConvertAll(buffers[k].Buffer, x => (float) x/short.MaxValue);
                                        int countData = data.Count()/2;
                                        var datas = new float[4, countData / 4];
                                        int j = 0;
                                        for (var t = 0; t < countData; t+=4)
                                        {
                                            datas[0, j] = data[t];
                                            datas[1, j] = data[t + 1];
                                            datas[2, j] = data[t + 2];
                                            datas[3, j] = data[t + 3];
                                            j++;
                                        }
                                        OnData(datas,4, countData/4, _numberBlock);
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

        
        public bool StopReadData()
        {
            _needReadData = false;
            if (_taskReadData != null)
                _taskReadData.Wait();
            _pModulE2010.STOP_ADC();
            return true;
        }
    }
}
