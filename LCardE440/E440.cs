using LCardE440Bridge;
using System;
using System.Threading;

namespace LCardE440
{
    public class AdcChannel
    {
        public int ChannelNumber { get; set; }
        public AdcInputRange ChannelInputRange { get; set; }
    }

    public class E440
    {
        private E440Bridge e440Bridge;

        public double AdcRate { get; private set; }
        public int Duration { get; private set; }
        public AdcChannel[] AdcChannels { get; private set; }

        public E440(double adcRate, int duration, AdcChannel[] adcChannels)
        {
            e440Bridge = new E440Bridge();

            AdcRate = adcRate;
            Duration = duration;
            AdcChannels = adcChannels;
        }

        public int InitDevice()
        {
            int[] channelsNumbers = new int[AdcChannels.Length];
            int[] channelsInputRanges = new int[AdcChannels.Length];
            for (int i = 0; i < AdcChannels.Length; i++)
            {
                channelsNumbers[i] = AdcChannels[i].ChannelNumber;
                channelsInputRanges[i] = AdcChannels[i].ChannelInputRange.ToInt();
            }

            e440Bridge.InitDevice(AdcRate, Duration, channelsNumbers, channelsInputRanges, AdcChannels.Length);

            return 0;
        }

        //private static readonly ushort MAX_CONTROL_TABLE_LENGTH_E440 = 128;
        //private static readonly ushort ADC_INPUT_RANGES_QUANTITY_E440 = AdcInputRange.INVALID_ADC_INPUT_RANGE_E440.ToUShort();
        //private static readonly ushort ADC_CALIBR_COEFS_QUANTITY_E440 = ADC_INPUT_RANGES_QUANTITY_E440;
        //private static readonly ushort MAX_ADC_FIFO_SIZE_E440 = 0x3000; // 12288
        //private static readonly ushort DAC_CHANNELS_QUANTITY_E440 = 0x2;
        //private static readonly ushort DAC_CALIBR_COEFS_QUANTITY_E440 = DAC_CHANNELS_QUANTITY_E440;
        //private static readonly ushort MAX_DAC_FIFO_SIZE_E440 = 0x0FC0; // 4032
        //private static readonly ushort TTL_LINES_QUANTITY_E440 = 0x10; // кол-во цифровых линий
        //private static readonly ushort REVISIONS_QUANTITY_E440 = Revision.INVALID_REVISION_E440.ToUShort(); // кол-во ревизий (модификаций) модуля

        //private E440Bridge _E440Bridge;
        //private double _AdcRate;
        //private TimeSpan _Duration;

        //private IntPtr _Handle;
        //private string _Name;
        //private byte _UsbSpeed;
        //private ModuleDescriptionE440 _ModuleDescription;
        //AdcParamsE440 _AdcParams;

        //public E440(double adcRate, int duration)
        //{
        //    _E440Bridge = new E440Bridge();
        //    var dll = _E440Bridge.DllVersion();
        //    var ins = _E440Bridge.CreateInstance();

        //    _AdcRate = adcRate;
        //    _Duration = duration;
        //}

        //public void ReadData()
        //{
        //    var open = _E440Bridge.OpenLDevice(0);

        //    _Handle = _E440Bridge.GetModuleHandleDevice();
        //    _Name = _E440Bridge.GetModuleName();
        //    _UsbSpeed = _E440Bridge.GetUsbSpeed();

        //    var load = _E440Bridge.LoadModule();
        //    var test = _E440Bridge.TestModule();

        //    _ModuleDescription = _E440Bridge.GetModuleDescription();
        //    _AdcParams = _E440Bridge.GetAdcParams();
        //    _AdcParams.InputMode = InputMode.NO_SYNC_E440.ToUShort();
        //    _AdcParams.IsCorrectionEnabled = true;

        //    ushort i;
        //    int k = 0;
        //    for (i = 0; i < 32; i++)
        //        if (i == 0)
        //        {
        //            _AdcParams.ControlTable[k] = (ushort)(i | (AdcInputRange.ADC_INPUT_RANGE_2500mV_E440.ToInt() << 6) | (AdcInputType.ADC_32_NOT_DIFF.ToInt() << 5));
        //            k++;
        //        }

        //    _AdcParams.AdcRate = 30;
        //    _AdcParams.InterKadrDelay = 0.0;
        //    _AdcParams.AdcFifoBaseAddress = 0x0;
        //    _AdcParams.AdcFifoLength = MAX_ADC_FIFO_SIZE_E440;

        //    for (i = 0x0; i < ADC_CALIBR_COEFS_QUANTITY_E440; i++)
        //    {
        //        _AdcParams.AdcOffsetCoefs[i] = _ModuleDescription.Adc.OffsetCalibration[i];
        //        _AdcParams.AdcScaleCoefs[i] = _ModuleDescription.Adc.ScaleCalibration[i];
        //    }

        //    _E440Bridge.SetAdcParams(_AdcParams);

        //    int DataStep = 10000;
        //    //Array^ ReadBuffer = new short[2 * DataStep];

        //    ushort RequestNumber;
        //    ulong FileBytesWritten;

        //    Overlapped[] ReadOv = new Overlapped[2];
        //    IoRequest[] IoReq = new IoRequest[2];

        //    _E440Bridge.StopAdc();

        //    //for (i = 0x0; i < 0x2; i++)
        //    //{
        //    //    ReadOv[i].EventHandle
        //    //    ReadOv[i].hEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
        //    //    // формируем структуру IoReq
        //    //    IoReq[i].Buffer = (short)(ReadBuffer + i * DataStep);
        //    //    IoReq[i].NumberOfWordsToPass = DataStep;
        //    //    IoReq[i].NumberOfWordsPassed = 0x0;
        //    //    IoReq[i].Overlapped = &ReadOv[i];
        //    //    IoReq[i].TimeOut = (DWORD)(DataStep / ap.AdcRate + 1000);
        //    //}

        //    //// делаем предварительный запрос на ввод данных
        //    //RequestNumber = 0x0;
        //    //if (!pModule->ReadData(&IoReq[RequestNumber])) { CloseHandle(ReadOv[0].hEvent); CloseHandle(ReadOv[1].hEvent); ReadThreadErrorNumber = 0x2; IsReadThreadComplete = true; return 0x0; }

        //    //// запустим АЦП
        //    //if (pModule->START_ADC())
        //    //{
        //    //    // цикл сбора данных
        //    //    for (i = 0x1; i < NDataBlock; i++)
        //    //    {
        //    //        // сделаем запрос на очередную порцию данных
        //    //        RequestNumber ^= 0x1;
        //    //        if (!pModule->ReadData(&IoReq[RequestNumber])) { ReadThreadErrorNumber = 0x2; break; }
        //    //        if (ReadThreadErrorNumber) break;

        //    //        // ждём завершения операции сбора предыдущей порции данных
        //    //        if (WaitForSingleObject(ReadOv[RequestNumber ^ 0x1].hEvent, IoReq[RequestNumber ^ 0x1].TimeOut) == WAIT_TIMEOUT) { ReadThreadErrorNumber = 0x3; break; }
        //    //        if (ReadThreadErrorNumber) break;

        //    //        // запишем полученную порцию данных в файл
        //    //        if (!WriteFile(hFile,                                               // handle to file to write to
        //    //                        IoReq[RequestNumber ^ 0x1].Buffer,                  // pointer to data to write to file
        //    //                        2 * DataStep,                                       // number of bytes to write
        //    //                        &FileBytesWritten,                                  // pointer to number of bytes written
        //    //                        NULL                                                // pointer to structure needed for overlapped I/O
        //    //                        ))
        //    //        {
        //    //            ReadThreadErrorNumber = 0x4;
        //    //            break;
        //    //        }

        //    //        if (ReadThreadErrorNumber) break;
        //    //        else if (_kbhit()) { ReadThreadErrorNumber = 0x5; break; }
        //    //        else Sleep(20);
        //    //        Counter++;
        //    //    }

        //    //    // последняя порция данных
        //    //    if (!ReadThreadErrorNumber)
        //    //    {
        //    //        RequestNumber ^= 0x1;
        //    //        // ждём окончания операции сбора последней порции данных
        //    //        if (WaitForSingleObject(ReadOv[RequestNumber ^ 0x1].hEvent, IoReq[RequestNumber ^ 0x1].TimeOut) == WAIT_TIMEOUT) ReadThreadErrorNumber = 0x3;
        //    //        // запишем последнюю порцию данных в файл
        //    //        if (!WriteFile(hFile,                                               // handle to file to write to
        //    //                        IoReq[RequestNumber ^ 0x1].Buffer,                  // pointer to data to write to file
        //    //                        2 * DataStep,                                       // number of bytes to write
        //    //                        &FileBytesWritten,                                  // pointer to number of bytes written
        //    //                        NULL                                                // pointer to structure needed for overlapped I/O
        //    //                        ))
        //    //            ReadThreadErrorNumber = 0x4;
        //    //        Counter++;
        //    //    }
        //    //}
        //    //else { ReadThreadErrorNumber = 0x6; }

        //    //// остановим работу АЦП
        //    //if (!pModule->STOP_ADC()) ReadThreadErrorNumber = 0x1;
        //    //// прервём возможно незавершённый асинхронный запрос на приём данных
        //    //if (!CancelIo(ModuleHandle)) { ReadThreadErrorNumber = 0x7; }
        //    //// освободим все идентификаторы событий
        //    //for (i = 0x0; i < 0x2; i++) CloseHandle(ReadOv[i].hEvent);
        //    //// небольшая задержка
        //    //Sleep(100);
        //    //// установим флажок завершения работы потока сбора данных
        //    //IsReadThreadComplete = true;
        //    //// теперь можно спокойно выходить из потока
        //    //return 0x0;

        //    _E440Bridge.Dispose();
        //}
    }
}
