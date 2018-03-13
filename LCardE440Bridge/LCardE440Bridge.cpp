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

	int E440Bridge::InitDevice(double adcRate, int duration, int chNums[], int chRanges[], int count)
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
		if(pModule->GetUsbSpeed(&Usb) == 0)
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

		if (pModule->GET_ADC_PARS(&adcParams) == 0)
			return 0x11;

		AdcParams = Convert(&adcParams);

		int count = adcRate * duration;
		int kb = p.count / 1024;

		if (kb > 1)
		{
			// подсчитаем максимально доступный объем одного забора (< 64k для некоторых плат)
			if (kb > 64)
			{
				for (i = 64; i > 0; i--)
				{
					if (kb % i == 0)
					{
						// делится нацело!
						NDataBlock = kb / i;
						DataStep = i * 1024;
						break;
					}
				}
			}
			else
			{
				NDataBlock = 1;
				DataStep = p.count;	// <= 64k
			}

		ReadBuffer = new SHORT[2 * DataStep];
		if (!ReadBuffer) AbortProgram(" Can not allocate memory\n");

		return 0x00;
	}

	//int E440Bridge::ReadData()
	//{
	//	ReadBuffer = new SHORT[2 * DataStep];

	//	hReadThread = CreateThread(0, 0x2000, ServiceReadThread, 0, 0, &ReadTid);

	//	// цикл записи получаемых данных и ожидания окончания работы приложения
	//	while (!IsReadThreadComplete)
	//	{
	//		if (OldCounter != Counter) { printf(" Counter %3u from %3u\r", Counter, NDataBlock); OldCounter = Counter; }
	//	}

	//	// ждём окончания работы потока ввода данных
	//	WaitForSingleObject(hReadThread, INFINITE);
	//}

	//DWORD E440Bridge::ServiceReadThread()
	//{
	//	WORD i;
	//	WORD RequestNumber;
	//	DWORD FileBytesWritten;
	//	// массив OVERLAPPED структур из двух элементов
	//	OVERLAPPED ReadOv[2];
	//	// массив структур с параметрами запроса на ввод/вывод данных
	//	IO_REQUEST_LUSBAPI IoReq[2];

	//	// остановим работу АЦП и одновременно сбросим USB-канал чтения данных
	//	if (!pModule->STOP_ADC()) { ReadThreadErrorNumber = 0x1; IsReadThreadComplete = true; return 0x0; }

	//	// формируем необходимые для сбора данных структуры
	//	for (i = 0x0; i < 0x2; i++)
	//	{
	//		// инициализация структуры типа OVERLAPPED
	//		ZeroMemory(&ReadOv[i], sizeof(OVERLAPPED));
	//		// создаём событие для асинхронного запроса
	//		ReadOv[i].hEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
	//		// формируем структуру IoReq
	//		IoReq[i].Buffer = ReadBuffer + i * 100000;
	//		IoReq[i].NumberOfWordsToPass = 100000;
	//		IoReq[i].NumberOfWordsPassed = 0x0;
	//		IoReq[i].Overlapped = &ReadOv[i];
	//		IoReq[i].TimeOut = (DWORD)(100000 / 30 + 1000);
	//	}

	//	// делаем предварительный запрос на ввод данных
	//	RequestNumber = 0x0;
	//	if (!pModule->ReadData(&IoReq[RequestNumber])) { CloseHandle(ReadOv[0].hEvent); CloseHandle(ReadOv[1].hEvent); ReadThreadErrorNumber = 0x2; IsReadThreadComplete = true; return 0x0; }

	//	// запустим АЦП
	//	if (pModule->START_ADC())
	//	{
	//		// цикл сбора данных
	//		for (i = 0x1; i < NDataBlock; i++)
	//		{
	//			// сделаем запрос на очередную порцию данных
	//			RequestNumber ^= 0x1;
	//			if (!pModule->ReadData(&IoReq[RequestNumber])) { ReadThreadErrorNumber = 0x2; break; }
	//			if (ReadThreadErrorNumber) break;

	//			// ждём завершения операции сбора предыдущей порции данных
	//			if (WaitForSingleObject(ReadOv[RequestNumber ^ 0x1].hEvent, IoReq[RequestNumber ^ 0x1].TimeOut) == WAIT_TIMEOUT) { ReadThreadErrorNumber = 0x3; break; }
	//			if (ReadThreadErrorNumber) break;

	//			short* data = IoReq[RequestNumber ^ 0x1].Buffer;

	//			if (ReadThreadErrorNumber) break;
	//			else Sleep(20);
	//			Counter++;
	//		}

	//		// последняя порция данных
	//		if (!ReadThreadErrorNumber)
	//		{
	//			RequestNumber ^= 0x1;
	//			// ждём окончания операции сбора последней порции данных
	//			if (WaitForSingleObject(ReadOv[RequestNumber ^ 0x1].hEvent, IoReq[RequestNumber ^ 0x1].TimeOut) == WAIT_TIMEOUT) ReadThreadErrorNumber = 0x3;

	//			Counter++;
	//		}
	//	}
	//	else { ReadThreadErrorNumber = 0x6; }

	//	// остановим работу АЦП
	//	if (!pModule->STOP_ADC()) ReadThreadErrorNumber = 0x1;
	//	// прервём возможно незавершённый асинхронный запрос на приём данных
	//	/*if (!CancelIo(ModuleHandle)) { ReadThreadErrorNumber = 0x7; }*/
	//	// освободим все идентификаторы событий
	//	for (i = 0x0; i < 0x2; i++) CloseHandle(ReadOv[i].hEvent);
	//	// небольшая задержка
	//	Sleep(100);
	//	// установим флажок завершения работы потока сбора данных
	//	IsReadThreadComplete = true;
	//	// теперь можно спокойно выходить из потока
	//	return 0x0;
	//}

	bool WINAPI E440Bridge::SetAdcParams(AdcParamsE440 mAdcParams)
	{
		ADC_PARS_E440 adcParams = Convert(mAdcParams);

		return pModule->SET_ADC_PARS(&adcParams) != 0;
	}

	bool WINAPI E440Bridge::StartAdc(void)
	{
		return pModule->START_ADC() != 0;
	}

	bool WINAPI E440Bridge::StopAdc(void)
	{
		return pModule->STOP_ADC() != 0;
	}

	//bool E440Bridge::CloseLDevice(void)
	//{
	//	return pModule->CloseLDevice();
	//}

	//bool E440Bridge::LowPowerMode(bool LowPowerFlag)
	//{
	//	return 0;
	//}

	//bool E440Bridge::GetLastErrorInfo(LAST_ERROR_INFO_LUSBAPI * const LastErrorInfo)
	//{
	//	return 0;
	//}

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