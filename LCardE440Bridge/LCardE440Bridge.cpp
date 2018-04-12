// This is the main DLL file.

#include "stdafx.h"
#include "LCardE440Bridge.h"

namespace LCardE440Bridge
{

#pragma region PrivateMethods

	bool E440Bridge::ReleaseLInstance(void)
	{
		return pModule->ReleaseLInstance() != 0;
	}

	ModuleDescriptionE440 E440Bridge::Convert(MODULE_DESCRIPTION_E440 * const moduleDescription)
	{
		ModuleDescriptionE440 res;

		// MODULE_INFO_LUSBAPI
		res.Module.CompanyName = gcnew System::String(reinterpret_cast<PCHAR>(moduleDescription->Module.CompanyName));
		res.Module.DeviceName = gcnew System::String(reinterpret_cast<PCHAR>(moduleDescription->Module.DeviceName));
		res.Module.SerialNumber = gcnew System::String(reinterpret_cast<PCHAR>(moduleDescription->Module.SerialNumber));
		res.Module.Revision = moduleDescription->Module.Revision;
		res.Module.Modification = moduleDescription->Module.Modification;
		res.Module.Comment = gcnew System::String(reinterpret_cast<PCHAR>(moduleDescription->Module.Comment));

		// INTERFACE_INFO_LUSBAPI
		res.Interface.Active = moduleDescription->Interface.Active;
		res.Interface.Name = gcnew System::String(reinterpret_cast<PCHAR>(moduleDescription->Interface.Name));
		res.Interface.Comment = gcnew System::String(reinterpret_cast<PCHAR>(moduleDescription->Interface.Comment));

		// MCU_INFO_LUSBAPI
		res.Mcu.Active = moduleDescription->Mcu.Active;
		res.Mcu.Name = gcnew System::String(reinterpret_cast<PCHAR>(moduleDescription->Mcu.Name));
		res.Mcu.ClockRate = moduleDescription->Mcu.ClockRate;
		res.Mcu.Version = Convert(&moduleDescription->Mcu.Version);
		res.Mcu.Comment = gcnew System::String(reinterpret_cast<PCHAR>(moduleDescription->Mcu.Comment));

		// DSP_INFO_LUSBAPI
		res.Dsp.Active = moduleDescription->Dsp.Active;
		res.Dsp.Name = gcnew System::String(reinterpret_cast<PCHAR>(moduleDescription->Dsp.Name));
		res.Dsp.ClockRate = moduleDescription->Dsp.ClockRate;
		res.Dsp.Version = Convert(&moduleDescription->Dsp.Version);
		res.Dsp.Comment = gcnew System::String(reinterpret_cast<PCHAR>(moduleDescription->Dsp.Comment));

		// ADC_INFO_LUSBAPI
		res.Adc.Active = moduleDescription->Adc.Active;
		res.Adc.Name = gcnew System::String(reinterpret_cast<PCHAR>(moduleDescription->Adc.Name));
		res.Adc.OffsetCalibration = gcnew array<double>(ADC_CALIBR_COEFS_QUANTITY_LUSBAPI);
		res.Adc.ScaleCalibration = gcnew array<double>(ADC_CALIBR_COEFS_QUANTITY_LUSBAPI);
		for (int i = 0; i < ADC_CALIBR_COEFS_QUANTITY_LUSBAPI; i++)
		{
			res.Adc.OffsetCalibration[i] = moduleDescription->Adc.OffsetCalibration[i];
			res.Adc.ScaleCalibration[i] = moduleDescription->Adc.ScaleCalibration[i];
		}
		res.Adc.Comment = gcnew System::String(reinterpret_cast<PCHAR>(moduleDescription->Adc.Comment));

		// DAC_INFO_LUSBAPI
		res.Dac.Active = moduleDescription->Dac.Active;
		res.Dac.Name = gcnew System::String(reinterpret_cast<PCHAR>(moduleDescription->Dac.Name));
		res.Dac.OffsetCalibration = gcnew array<double>(DAC_CALIBR_COEFS_QUANTITY_LUSBAPI);
		res.Dac.ScaleCalibration = gcnew array<double>(DAC_CALIBR_COEFS_QUANTITY_LUSBAPI);
		for (int i = 0; i < DAC_CALIBR_COEFS_QUANTITY_LUSBAPI; i++)
		{
			res.Dac.OffsetCalibration[i] = moduleDescription->Dac.OffsetCalibration[i];
			res.Dac.ScaleCalibration[i] = moduleDescription->Dac.ScaleCalibration[i];
		}
		res.Dac.Comment = gcnew System::String(reinterpret_cast<PCHAR>(moduleDescription->Dac.Comment));

		// DIGITAL_IO_INFO_LUSBAPI
		res.DigitalIo.Active = moduleDescription->DigitalIo.Active;
		res.DigitalIo.Name = gcnew System::String(reinterpret_cast<PCHAR>(moduleDescription->DigitalIo.Name));
		res.DigitalIo.InLinesQuantity = moduleDescription->DigitalIo.InLinesQuantity;
		res.DigitalIo.OutLinesQuantity = moduleDescription->DigitalIo.OutLinesQuantity;
		res.DigitalIo.Comment = gcnew System::String(reinterpret_cast<PCHAR>(moduleDescription->DigitalIo.Comment));

		return res;
	}

	VersionInfo E440Bridge::Convert(VERSION_INFO_LUSBAPI * const versionInfo)
	{
		VersionInfo res;
		res.Version = gcnew System::String(reinterpret_cast<PCHAR>(versionInfo->Version));
		res.Date = gcnew System::String(reinterpret_cast<PCHAR>(versionInfo->Date));
		res.Manufacturer = gcnew System::String(reinterpret_cast<PCHAR>(versionInfo->Manufacturer));
		res.Author = gcnew System::String(reinterpret_cast<PCHAR>(versionInfo->Author));
		res.Comment = gcnew System::String(reinterpret_cast<PCHAR>(versionInfo->Comment));
		return res;
	}

	AdcParamsE440 E440Bridge::Convert(ADC_PARS_E440 * const adcParams)
	{
		AdcParamsE440 res;

		res.IsAdcEnabled = adcParams->IsAdcEnabled;
		res.IsCorrectionEnabled = adcParams->IsCorrectionEnabled;
		res.AdcClockSource = adcParams->AdcClockSource;
		res.InputMode = adcParams->InputMode;
		res.SynchroAdType = adcParams->SynchroAdType;
		res.SynchroAdMode = adcParams->SynchroAdMode;
		res.SynchroAdChannel = adcParams->SynchroAdChannel;
		res.SynchroAdPorog = adcParams->SynchroAdPorog;
		res.ChannelsQuantity = adcParams->ChannelsQuantity;
		res.ControlTable = gcnew array<WORD>(MAX_CONTROL_TABLE_LENGTH_E440);
		for (int i = 0; i < MAX_CONTROL_TABLE_LENGTH_E440; i++)
		{
			res.ControlTable[i] = adcParams->ControlTable[i];
		}
		res.AdcRate = adcParams->AdcRate;
		res.InterKadrDelay = adcParams->InterKadrDelay;
		res.KadrRate = adcParams->KadrRate;
		res.AdcFifoBaseAddress = adcParams->AdcFifoBaseAddress;
		res.AdcFifoLength = adcParams->AdcFifoLength;
		res.AdcOffsetCoefs = gcnew array<double>(ADC_CALIBR_COEFS_QUANTITY_E440);
		res.AdcScaleCoefs = gcnew array<double>(ADC_CALIBR_COEFS_QUANTITY_E440);
		for (int i = 0; i < ADC_CALIBR_COEFS_QUANTITY_E440; i++)
		{
			res.AdcOffsetCoefs[i] = adcParams->AdcOffsetCoefs[i];
			res.AdcScaleCoefs[i] = adcParams->AdcScaleCoefs[i];
		}

		return res;
	}

	ADC_PARS_E440 E440Bridge::Convert(AdcParamsE440^ mAdcParams)
	{
		ADC_PARS_E440 res;

		res.IsAdcEnabled = mAdcParams->IsAdcEnabled;
		res.IsCorrectionEnabled = mAdcParams->IsCorrectionEnabled;
		res.AdcClockSource = mAdcParams->AdcClockSource;
		res.InputMode = mAdcParams->InputMode;
		res.SynchroAdType = mAdcParams->SynchroAdType;
		res.SynchroAdMode = mAdcParams->SynchroAdMode;
		res.SynchroAdChannel = mAdcParams->SynchroAdChannel;
		res.SynchroAdPorog = mAdcParams->SynchroAdPorog;
		res.ChannelsQuantity = mAdcParams->ChannelsQuantity;
		for (int i = 0; i < MAX_CONTROL_TABLE_LENGTH_E440; i++)
		{
			res.ControlTable[i] = mAdcParams->ControlTable[i];
		}
		res.AdcRate = mAdcParams->AdcRate;
		res.InterKadrDelay = mAdcParams->InterKadrDelay;
		res.KadrRate = mAdcParams->KadrRate;
		res.AdcFifoBaseAddress = mAdcParams->AdcFifoBaseAddress;
		res.AdcFifoLength = mAdcParams->AdcFifoLength;
		for (int i = 0; i < ADC_CALIBR_COEFS_QUANTITY_E440; i++)
		{
			res.AdcOffsetCoefs[i] = mAdcParams->AdcOffsetCoefs[i];
			res.AdcScaleCoefs[i] = mAdcParams->AdcScaleCoefs[i];
		}

		return res;
	}

#pragma endregion

#pragma region PublicMethods

	E440Bridge::E440Bridge()
	{

	}

	int E440Bridge::InitDevice(double adcRate, int duration, array<int>^ chNums, array<int>^ chRanges, int count)
	{
		if (GetDllVersion() != CURRENT_VERSION_LUSBAPI)
			return 0x01;

		pModule = static_cast<ILE440 *>(CreateLInstance("e440"));
		if (pModule == nullptr)
			return 0x02;

		if (pModule->OpenLDevice(0) == 0)
			return 0x03;

		_ModuleHandle = pModule->GetModuleHandle();
		if (_ModuleHandle == INVALID_HANDLE_VALUE)
			return 0x04;

		char moduleName[7];
		if (pModule->GetModuleName(moduleName) == 0)
			return 0x05;

		if (strcmp(moduleName, "E440") != 0)
			return 0x06;

		BYTE Usb;
		if (pModule->GetUsbSpeed(&Usb) == 0)
			return 0x07;

		if (pModule->LOAD_MODULE() == 0)
			return 0x08;

		if (pModule->TEST_MODULE() == 0)
			return 0x09;

		MODULE_DESCRIPTION_E440 moduleDescription;
		if (pModule->GET_MODULE_DESCRIPTION(&moduleDescription) == 0)
			return 0x10;

		ModuleDescription = Convert(&moduleDescription);

		ADC_PARS_E440 adcParams;
		if (pModule->GET_ADC_PARS(&adcParams) == 0)
			return 0x11;

		adcParams.IsCorrectionEnabled = TRUE;
		adcParams.InputMode = NO_SYNC_E440;
		adcParams.ChannelsQuantity = count;

		for (int i = 0; i < count; i++)
			adcParams.ControlTable[i] = (WORD)(chNums[i] | (chRanges[i] << 6) | (1 << 5));

		adcParams.AdcRate = adcRate;
		adcParams.InterKadrDelay = 0.0;
		adcParams.AdcFifoBaseAddress = 0x0;
		adcParams.AdcFifoLength = MAX_ADC_FIFO_SIZE_E440;

		for (int i = 0x0; i < ADC_CALIBR_COEFS_QUANTITY_E440; i++)
		{
			adcParams.AdcOffsetCoefs[i] = ModuleDescription.Adc.OffsetCalibration[i];
			adcParams.AdcScaleCoefs[i] = ModuleDescription.Adc.ScaleCalibration[i];
		}

		if (pModule->SET_ADC_PARS(&adcParams) == 0)
			return 0x12;

		AdcParams = Convert(&adcParams);

		int pCount = AdcParams.AdcRate * duration * 1000;
		int numBlocks = pCount / 32;
		_NDataBlock = 1;
		_DataStep = pCount;

		if (numBlocks > 2047)
			for (int i = 2048; i > 0; i--)
				if (numBlocks % i == 0)
				{
					_NDataBlock = numBlocks / i;
					_DataStep = i * 32;
					break;
				}

		return 0x00;
	}

	int E440Bridge::ReadData()
	{
		_ReadBuffer = new short[2 * _DataStep];
		_Data = gcnew array<short>(_NDataBlock * _DataStep);

		if (pModule->STOP_ADC() == 0)
			return 0x13;

		int RequestNumber;
		IO_REQUEST_LUSBAPI ioRequest[2];
		OVERLAPPED readOverlapped[2];

		for (int i = 0; i < 2; i++)
		{
			readOverlapped[i].hEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
			ioRequest[i].Buffer = _ReadBuffer + i * _DataStep;
			ioRequest[i].NumberOfWordsToPass = _DataStep;
			ioRequest[i].NumberOfWordsPassed = 0x0;
			ioRequest[i].Overlapped = &readOverlapped[i];
			ioRequest[i].TimeOut = (DWORD)(_DataStep / AdcParams.AdcRate + 1000);
		}

		RequestNumber = 0x0;
		if (pModule->ReadData(&ioRequest[RequestNumber]) == 0)
		{
			CloseHandle(readOverlapped[0].hEvent);
			CloseHandle(readOverlapped[1].hEvent);
			return 0x14;
		}

		short* data;

		if (pModule->START_ADC() != 0)
		{
			int i;
			for (i = 1; i < _NDataBlock; i++)
			{
				RequestNumber ^= 0x1;
				if (pModule->ReadData(&ioRequest[RequestNumber]) == 0)
					return 0x14;

				if (WaitForSingleObject(readOverlapped[RequestNumber ^ 0x1].hEvent, ioRequest[RequestNumber ^ 0x1].TimeOut) == WAIT_TIMEOUT)
					return 0x15;

				data = ioRequest[RequestNumber ^ 0x1].Buffer;
				for (int j = 0; j < _DataStep; j++)
					_Data[(i - 1) * _DataStep + j] = data[j];
			}

			RequestNumber ^= 0x1;

			if (WaitForSingleObject(readOverlapped[RequestNumber ^ 0x1].hEvent, ioRequest[RequestNumber ^ 0x1].TimeOut) == WAIT_TIMEOUT)
				return 0x15;

			data = ioRequest[RequestNumber ^ 0x1].Buffer;
			for (int j = 0; j < _DataStep; j++)
				_Data[(i - 1) * _DataStep + j] = data[j];
		}
		else
			return 0x16;

		pModule->STOP_ADC();

		if (CancelIo(_ModuleHandle) == 0)
			return 0x17;

		CloseHandle(readOverlapped[0].hEvent);
		CloseHandle(readOverlapped[1].hEvent);

		Sleep(100);

		if (_ReadBuffer) { delete[] _ReadBuffer; _ReadBuffer = NULL; }

		return 0x00;
	}

	array<short>^ E440Bridge::GetResult()
	{
		return _Data;
	}

	E440Bridge::~E440Bridge()
	{
		if (pModule)
		{
			pModule->STOP_ADC();
			pModule->ReleaseLInstance();
			pModule = NULL;
		}
	}

#pragma endregion

}