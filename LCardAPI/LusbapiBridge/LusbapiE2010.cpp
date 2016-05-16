#include "stdafx.h"
#include "LusbapiE2010.h"


namespace LusbapiBridgeE2010 {



	LusbapiE2010::LusbapiE2010() {

		DWORD DllVersion;
		// проверим версию используемой библиотеки Lusbapi.dll
		if ((DllVersion = GetDllVersion()) != CURRENT_VERSION_LUSBAPI)
		{
			char String[128];
			printf(String, " Lusbapi.dll Version Error!!!\n   Current: %1u.%1u. Required: %1u.%1u",
				DllVersion >> 0x10, DllVersion & 0xFFFF,
				CURRENT_VERSION_LUSBAPI >> 0x10, CURRENT_VERSION_LUSBAPI & 0xFFFF);

		}
		else printf(" Lusbapi.dll Version --> OK\n");

		// попробуем получить указатель на интерфейс
		pModule = static_cast<ILE2010 *>(CreateLInstance("e2010"));
		if (!pModule) printf(" Module Interface --> Bad\n");
		else printf(" Module Interface --> OK\n");

		this->ReadOv = new OVERLAPPED[2];
		this->IoReq = new IO_REQUEST_LUSBAPI[2];
		this->DataState = new DATA_STATE_E2010[1];
	}

	// функции общего назначения для работы с USB устройствами
	BOOL WINAPI LusbapiE2010::OpenLDevice(WORD VirtualSlot){
		return pModule->OpenLDevice(VirtualSlot);
	}
	BOOL WINAPI LusbapiE2010::CloseLDevice(void){
		return pModule->CloseLDevice();
	}
	BOOL WINAPI LusbapiE2010::ReleaseLInstance(void){
		return pModule->ReleaseLInstance();
	}
	// получение дескриптора устройства USB
	IntPtr WINAPI LusbapiE2010::GetModuleHandleDevice(void){	
		return IntPtr(this->pModule->GetModuleHandle());
	}
	// получение названия используемого модуля
	String^ WINAPI LusbapiE2010::GetModuleName(){
		const PCHAR nativeString = new char[7];
		BOOL res = pModule->GetModuleName(nativeString);
		String^ resStr = gcnew System::String("");
		if (res > 0)
			resStr = gcnew System::String(nativeString);
		return resStr;
	}

	// получение текущей скорости работы шины USB
	BYTE WINAPI LusbapiE2010::GetUsbSpeed(){
		BYTE nativeString;
		BOOL res = pModule->GetUsbSpeed(&nativeString);
		if (res > 0)
			return nativeString;
		else
			return -1;
	}
	// управления режимом низкого электропотребления модуля
	BOOL WINAPI LusbapiE2010::LowPowerMode(BOOL LowPowerFlag){
		return pModule->LowPowerMode(LowPowerFlag);
	}
	// функция выдачи строки с последней ошибкой
	BOOL WINAPI LusbapiE2010::GetLastErrorInfo(LAST_ERROR_INFO_LUSBAPI * const LastErrorInfo){
		return pModule->GetLastErrorInfo(LastErrorInfo);
	}

	// загрузка ПЛИС модуля
	BOOL WINAPI LusbapiE2010::LOAD_MODULE(String^ fileName){
		if (fileName->Length > 0){
			IntPtr FileNamePtr = Marshal::StringToHGlobalAnsi(fileName);
			const PCHAR FileName = static_cast<char*>(FileNamePtr.ToPointer());
			return pModule->LOAD_MODULE(FileName);
		}
		else {
			return pModule->LOAD_MODULE();
		}
	}
	BOOL WINAPI LusbapiE2010::TEST_MODULE(WORD TestModeMask){
		return pModule->TEST_MODULE(TestModeMask);
	}
	BOOL WINAPI LusbapiE2010::TEST_MODULE(){
		return pModule->TEST_MODULE();
	}

	// работа с АЦП
	M_ADC_PARS_E2010 WINAPI LusbapiE2010::GET_ADC_PARS(){
		M_ADC_PARS_E2010 res;
		ADC_PARS_E2010 AdcPars;
		BOOL resAdcPars = pModule->GET_ADC_PARS(&AdcPars);
		if (resAdcPars > 0){
			res = this->Convert(&AdcPars);
		}

		return res;
	}

	M_ADC_PARS_E2010 WINAPI LusbapiE2010::SET_ADC_PARS(M_ADC_PARS_E2010 MAdcPars, int DataStep){
		M_ADC_PARS_E2010 res;
		ADC_PARS_E2010 AdcPars = Convert(MAdcPars);
		BOOL resSetParams = pModule->SET_ADC_PARS(&AdcPars);
		if (resSetParams > 0){
			res = Convert(&AdcPars);
			//set buffers
			// формируем необходимые для сбора данных структуры
			this->DataStep = DataStep;
			AdcBuffer = new short[2 * DataStep];
			int i;
			for (i = 0x0; i < 0x2; i++)
			{
				// инициализация структуры типа OVERLAPPED
				ZeroMemory(&ReadOv[i], sizeof(OVERLAPPED));
				// создаём событие для асинхронного запроса
				ReadOv[i].hEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
				// формируем структуру IoReq
				IoReq[i].Buffer = AdcBuffer + i*DataStep;
				IoReq[i].NumberOfWordsToPass = DataStep;
				IoReq[i].NumberOfWordsPassed = 0x0;
				IoReq[i].Overlapped = &ReadOv[i];
				IoReq[i].TimeOut = (DWORD)(DataStep / AdcPars.KadrRate + 1000);
			}
		}
		return res;
	}

	BOOL WINAPI LusbapiE2010::START_ADC(void){
		return pModule->START_ADC();
	}
	BOOL WINAPI LusbapiE2010::STOP_ADC(void){
		return pModule->STOP_ADC();
	}
	BOOL WINAPI LusbapiE2010::GET_DATA_STATE(M_DATA_STATE_E2010 ^% DataState){
		static DATA_STATE_E2010 dataState;
		BOOL resDataState = pModule->GET_DATA_STATE(&dataState);
		if (resDataState > 0){
			Convert(DataState, &dataState);
		}
		return resDataState;
	}

	BOOL WINAPI LusbapiE2010::InitReading(){
		RequestNumber = 0;
		if (!pModule->ReadData(&IoReq[RequestNumber])) { CloseHandle(ReadOv[0].hEvent); CloseHandle(ReadOv[1].hEvent); }
		return 1;
	}
	BOOL WINAPI LusbapiE2010::ReadDataSync(M_IO_REQUEST_LUSBAPI % ReadRequest){
		BOOL res = 1;
		//printf("RequestNumber = %d", RequestNumber);
		// сделаем запрос на очередную порцию данных

		RequestNumber ^= 0x1;
		if (!pModule->ReadData(&IoReq[RequestNumber])) { res = 0x2; return res; }

		// ждём завершения операции сбора предыдущей порции данных
		if (WaitForSingleObject(ReadOv[RequestNumber ^ 0x1].hEvent, IoReq[RequestNumber ^ 0x1].TimeOut) == WAIT_TIMEOUT) { res = 0x3; return res; }

		// попробуем получить текущее состояние процесса сбора данных
		if (!pModule->GET_DATA_STATE(&(DataState[0]))) { res = 0x7; return res; }
		// теперь можно проверить этот признак переполнения внутреннего буфера модуля
		else if ((DataState[0]).BufferOverrun == (0x1 << BUFFER_OVERRUN_E2010)) { res = 0x8; return res; }

		// запишем полученную порцию данных в буффер
		int i = 0;
		ReadRequest.NumberOfWordsPassed = IoReq[RequestNumber ^ 0x1].NumberOfWordsPassed;
		ReadRequest.NumberOfWordsToPass = IoReq[RequestNumber ^ 0x1].NumberOfWordsToPass;
		ReadRequest.TimeOut = IoReq[RequestNumber ^ 0x1].TimeOut;
		// handle to file to write to
		for (i = 0; i < DataStep; i++)// number of bytes to write
		{
			ReadRequest.Buffer[i] = IoReq[RequestNumber ^ 0x1].Buffer[i];// pointer to data to write to file
		}

		return res;
	}

	// однократная синхронная работа с ЦАП
	BOOL WINAPI LusbapiE2010::DAC_SAMPLE(SHORT * const DacData, WORD DacChannel){
		return pModule->DAC_SAMPLE(DacData, DacChannel);
	}

	// информация о модуле
	M_MODULE_DESCRIPTION_E2010 WINAPI LusbapiE2010::GET_MODULE_DESCRIPTION(){
		MODULE_DESCRIPTION_E2010 ModuleDescription;
		BOOL resModule = pModule->GET_MODULE_DESCRIPTION(&ModuleDescription);
		M_MODULE_DESCRIPTION_E2010 res;
		if (resModule > 0)
		{
			res = this->Convert(&ModuleDescription);
		}
		return res;
	}

	BOOL WINAPI LusbapiE2010::SAVE_MODULE_DESCRIPTION(MODULE_DESCRIPTION_E2010 * const ModuleDescription){
		return pModule->SAVE_MODULE_DESCRIPTION(ModuleDescription);
	}

	
	BOOL WINAPI LusbapiE2010::ENABLE_TTL_OUT(BOOL EnableTtlOut) {
		return pModule->ENABLE_TTL_OUT(EnableTtlOut);
	}

	BOOL WINAPI LusbapiE2010::TTL_IN(WORD % const TtlIn){
		WORD ttlInRes;
		BOOL res = pModule->TTL_IN(&ttlInRes);
		TtlIn = ttlInRes;
		return res;
	}



	BOOL WINAPI LusbapiE2010::TTL_OUT(WORD TtlOut){
		return pModule->TTL_OUT(TtlOut);
	}

	M_MODULE_DESCRIPTION_E2010 LusbapiE2010::Convert(MODULE_DESCRIPTION_E2010 * const ModuleDescription){
		M_MODULE_DESCRIPTION_E2010 res;

		res.Module.CompanyName = gcnew System::String(reinterpret_cast<PCHAR>(ModuleDescription->Module.CompanyName));
		res.Module.DeviceName = gcnew System::String(reinterpret_cast<PCHAR>(ModuleDescription->Module.DeviceName));
		res.Module.SerialNumber = gcnew System::String(reinterpret_cast<PCHAR>(ModuleDescription->Module.SerialNumber));
		res.Module.Revision = ModuleDescription->Module.Revision;
		res.Module.Modification = ModuleDescription->Module.Modification;
		res.Module.Comment = gcnew System::String(reinterpret_cast<PCHAR>(ModuleDescription->Module.Comment));

		res.Interface.Active = ModuleDescription->Interface.Active;
		res.Interface.Name = gcnew System::String(reinterpret_cast<PCHAR>(ModuleDescription->Interface.Name));
		res.Interface.Comment = gcnew System::String(reinterpret_cast<PCHAR>(ModuleDescription->Interface.Comment));

		res.Mcu.Active = ModuleDescription->Mcu.Active;
		res.Mcu.Name = gcnew System::String(reinterpret_cast<PCHAR>(ModuleDescription->Mcu.Name));
		res.Mcu.ClockRate = ModuleDescription->Mcu.ClockRate;
		res.Mcu.Version.BlVersion = Convert(&ModuleDescription->Mcu.Version.BlVersion);
		res.Mcu.Version.FwVersion = Convert(&ModuleDescription->Mcu.Version.FwVersion);
		res.Mcu.Comment = gcnew System::String(reinterpret_cast<PCHAR>(ModuleDescription->Mcu.Comment));

		res.Pld.Active = ModuleDescription->Pld.Active;
		res.Pld.Name = gcnew System::String(reinterpret_cast<PCHAR>(ModuleDescription->Pld.Name));
		res.Pld.ClockRate = ModuleDescription->Pld.ClockRate;
		res.Pld.Version = Convert(&ModuleDescription->Pld.Version);
		res.Pld.Comment = gcnew System::String(reinterpret_cast<PCHAR>(ModuleDescription->Pld.Comment));

		int i;
		res.Adc.Active = ModuleDescription->Adc.Active;
		res.Adc.Name = gcnew System::String(reinterpret_cast<PCHAR>(ModuleDescription->Adc.Name));
		res.Adc.OffsetCalibration = gcnew array<double>(ADC_CALIBR_COEFS_QUANTITY_LUSBAPI);
		res.Adc.ScaleCalibration = gcnew array<double>(ADC_CALIBR_COEFS_QUANTITY_LUSBAPI);
		for (i = 0; i < ADC_CALIBR_COEFS_QUANTITY_LUSBAPI; i++){
			res.Adc.OffsetCalibration[i] = ModuleDescription->Adc.OffsetCalibration[i];
			res.Adc.ScaleCalibration[i] = ModuleDescription->Adc.ScaleCalibration[i];
		}
		res.Adc.Comment = gcnew System::String(reinterpret_cast<PCHAR>(ModuleDescription->Adc.Comment));
		//TODO extend description


		return res;
	}

	M_VERSION_INFO_LUSBAPI LusbapiE2010::Convert(VERSION_INFO_LUSBAPI * const VersionInfo){
		M_VERSION_INFO_LUSBAPI res;
		res.Version = gcnew System::String(reinterpret_cast<PCHAR>(VersionInfo->Version));
		res.Date = gcnew System::String(reinterpret_cast<PCHAR>(VersionInfo->Date));
		res.Manufacturer = gcnew System::String(reinterpret_cast<PCHAR>(VersionInfo->Manufacturer));
		res.Author = gcnew System::String(reinterpret_cast<PCHAR>(VersionInfo->Author));
		res.Comment = gcnew System::String(reinterpret_cast<PCHAR>(VersionInfo->Comment));
		return res;
	}

	M_ADC_PARS_E2010 LusbapiE2010::Convert(ADC_PARS_E2010 * const AdcPars){
		M_ADC_PARS_E2010 res;
		int i, j;
		res.IsAdcCorrectionEnabled = AdcPars->IsAdcCorrectionEnabled;
		res.OverloadMode = AdcPars->OverloadMode;
		res.InputCurrentControl = AdcPars->InputCurrentControl;

		res.SynchroPars.StartSource = AdcPars->SynchroPars.StartSource;
		res.SynchroPars.StartDelay = AdcPars->SynchroPars.StartDelay;
		res.SynchroPars.SynhroSource = AdcPars->SynchroPars.SynhroSource;
		res.SynchroPars.StopAfterNKadrs = AdcPars->SynchroPars.StopAfterNKadrs;
		res.SynchroPars.SynchroAdMode = AdcPars->SynchroPars.SynchroAdMode;
		res.SynchroPars.SynchroAdChannel = AdcPars->SynchroPars.SynchroAdChannel;
		res.SynchroPars.SynchroAdPorog = AdcPars->SynchroPars.SynchroAdPorog;
		res.SynchroPars.IsBlockDataMarkerEnabled = AdcPars->SynchroPars.IsBlockDataMarkerEnabled;

		res.ChannelsQuantity = AdcPars->ChannelsQuantity;
		res.ControlTable = gcnew array<WORD>(MAX_CONTROL_TABLE_LENGTH_E2010);
		for (i = 0; i < MAX_CONTROL_TABLE_LENGTH_E2010; i++){
			res.ControlTable[i] = AdcPars->ControlTable[i];
		}
		res.InputRange = gcnew array<WORD>(ADC_CHANNELS_QUANTITY_E2010);
		for (i = 0; i < ADC_CHANNELS_QUANTITY_E2010; i++){
			res.InputRange[i] = AdcPars->InputRange[i];
		}
		res.InputSwitch = gcnew array<WORD>(ADC_CHANNELS_QUANTITY_E2010);
		for (i = 0; i < ADC_CHANNELS_QUANTITY_E2010; i++){
			res.InputSwitch[i] = AdcPars->InputSwitch[i];
		}

		res.AdcRate = AdcPars->AdcRate;
		res.InterKadrDelay = AdcPars->InterKadrDelay;
		res.KadrRate = AdcPars->KadrRate;

		res.AdcOffsetCoefs = gcnew array<double, 2>(ADC_INPUT_RANGES_QUANTITY_E2010, ADC_CHANNELS_QUANTITY_E2010);
		res.AdcScaleCoefs = gcnew array<double, 2>(ADC_INPUT_RANGES_QUANTITY_E2010, ADC_CHANNELS_QUANTITY_E2010);

		for (i = 0; i < ADC_INPUT_RANGES_QUANTITY_E2010; i++){
			for (j = 0; j < ADC_CHANNELS_QUANTITY_E2010; j++){

				res.AdcOffsetCoefs[i, j] = AdcPars->AdcOffsetCoefs[i][j];
				res.AdcScaleCoefs[i, j] = AdcPars->AdcScaleCoefs[i][j];
			}
		}


		return res;
	}

	ADC_PARS_E2010 LusbapiE2010::Convert(M_ADC_PARS_E2010^ MAdcPars){
		ADC_PARS_E2010 res;

		int i, j;
		res.IsAdcCorrectionEnabled = MAdcPars->IsAdcCorrectionEnabled;
		res.OverloadMode = MAdcPars->OverloadMode;
		res.InputCurrentControl = MAdcPars->InputCurrentControl;

		res.SynchroPars.StartSource = MAdcPars->SynchroPars.StartSource;
		res.SynchroPars.StartDelay = MAdcPars->SynchroPars.StartDelay;
		res.SynchroPars.SynhroSource = MAdcPars->SynchroPars.SynhroSource;
		res.SynchroPars.StopAfterNKadrs = MAdcPars->SynchroPars.StopAfterNKadrs;
		res.SynchroPars.SynchroAdMode = MAdcPars->SynchroPars.SynchroAdMode;
		res.SynchroPars.SynchroAdChannel = MAdcPars->SynchroPars.SynchroAdChannel;
		res.SynchroPars.SynchroAdPorog = MAdcPars->SynchroPars.SynchroAdPorog;
		res.SynchroPars.IsBlockDataMarkerEnabled = MAdcPars->SynchroPars.IsBlockDataMarkerEnabled;

		res.ChannelsQuantity = MAdcPars->ChannelsQuantity;
		for (i = 0; i < MAX_CONTROL_TABLE_LENGTH_E2010; i++){
			res.ControlTable[i] = MAdcPars->ControlTable[i];
		}
		for (i = 0; i < ADC_CHANNELS_QUANTITY_E2010; i++){
			res.InputRange[i] = MAdcPars->InputRange[i];
		}
		for (i = 0; i < ADC_CHANNELS_QUANTITY_E2010; i++){
			res.InputSwitch[i] = MAdcPars->InputSwitch[i];
		}

		res.AdcRate = MAdcPars->AdcRate;
		res.InterKadrDelay = MAdcPars->InterKadrDelay;
		res.KadrRate = MAdcPars->KadrRate;

		for (i = 0; i < ADC_INPUT_RANGES_QUANTITY_E2010; i++){
			for (j = 0; j < ADC_CHANNELS_QUANTITY_E2010; j++){

				res.AdcOffsetCoefs[i][j] = MAdcPars->AdcOffsetCoefs[i,j];
				res.AdcScaleCoefs[i][j] = MAdcPars->AdcScaleCoefs[i,j];
			}
		}

		return res;
	}

	void LusbapiE2010::Convert(M_DATA_STATE_E2010 ^% MDataState, DATA_STATE_E2010 * const DataState){

		MDataState->ChannelsOverFlow = DataState->ChannelsOverFlow;
		MDataState->BufferOverrun = DataState->BufferOverrun;
		MDataState->CurBufferFilling = DataState->CurBufferFilling;
		MDataState->MaxOfBufferFilling = DataState->MaxOfBufferFilling;
		MDataState->BufferSize = DataState->BufferSize;
		MDataState->CurBufferFillingPercent = DataState->CurBufferFillingPercent;
		MDataState->MaxOfBufferFillingPercent = DataState->MaxOfBufferFillingPercent;
	}

	LusbapiE2010::~LusbapiE2010()
	{
		pModule->STOP_ADC();
		pModule = NULL;
	}
}