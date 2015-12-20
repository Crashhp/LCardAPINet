﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Caliburn.Micro;
using LCard.API.Data;
using LCard.API.Data.E2010;
using LCard.API.Interfaces;
using LCard.Core.Logger;
using LCard.E2010GUI.Startup;
using LusbapiBridgeE2010;

namespace LCard.E2010GUI.ViewModels.Actions
{
    public class DeviceSettingsViewModel : PropertyChangedBase
    {
        private ADC_INPUTV aDC_INPUTV = ADC_INPUTV.ADC_INPUT_RANGE_300mV_E2010;

        private string _selectedRangeIndex = "0.3, V";

        public List<string> RangeIndex
        {
            get
            {
                return new List<string> { "0.3, V", "1.0, V", "3.0, V" };
            }
        }

        public string SelectedRangeIndex
        {
            get
            {
                return _selectedRangeIndex;
            }
            set
            {
                _selectedRangeIndex = value;
                if (_selectedRangeIndex == RangeIndex[0]) aDC_INPUTV = ADC_INPUTV.ADC_INPUT_RANGE_300mV_E2010;
                if (_selectedRangeIndex == RangeIndex[1]) aDC_INPUTV = ADC_INPUTV.ADC_INPUT_RANGE_1000mV_E2010;
                if (_selectedRangeIndex == RangeIndex[2]) aDC_INPUTV = ADC_INPUTV.ADC_INPUT_RANGE_3000mV_E2010;
                NotifyOfPropertyChange(() => this.SelectedRangeIndex);
            }
        }

        public void ButtonSetDeviceSettingsDefault()
        {
            Logger.Current.Info("Set Default Adc Params");
            SetDefaultAdcParams();
        }

        private static void SetDefaultAdcParams()
        {
            var e2020 = UnityConfig.GetConfiguredContainer().Resolve<IE2010>();
            if (e2020.Inited == false) e2020.Init();

            var moduleDescription = e2020.GET_MODULE_DESCRIPTION();
            var ap = e2020.GET_ADC_PARS();

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
            const double AdcRate = 1000.0;
            ap.AdcRate = AdcRate;
            int DataStep;
            // частота работы АЦП в кГц
            var usbSpeed = e2020.GetUsbSpeed();
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
                ap.InputRange[i] = (ushort)ADC_INPUTV.ADC_INPUT_RANGE_3000mV_E2010;  	// входной диапазоне 3В
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
            e2020.SET_ADC_PARS(ap, DataStep);
        }
    }
}
