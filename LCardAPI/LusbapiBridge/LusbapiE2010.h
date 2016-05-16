#pragma once

using namespace System;
using namespace System::Runtime::InteropServices;
#include "stdafx.h"
#include "Lusbapi.h"
#include "LusbapiTypes.h"
#include <stdio.h>
#include <fstream>
#include <iostream>
#include <conio.h>
using namespace std;
#include "MLusbapiTypes.h"

namespace LusbapiBridgeE2010 {

	public ref class LusbapiE2010
	{
	public:
		LusbapiE2010();
		virtual ~LusbapiE2010();

		// ==========================================================================
		// *************************** L-Card USB BASE ******************************
		// ==========================================================================

		// функции общего назначения для работы с USB устройствами
		BOOL WINAPI OpenLDevice(WORD VirtualSlot);
		BOOL WINAPI CloseLDevice(void);
		BOOL WINAPI ReleaseLInstance(void);
		// получение дескриптора устройства USB
		IntPtr WINAPI GetModuleHandleDevice(void);
		// получение названия используемого модуля
		String^ WINAPI GetModuleName();
		// получение текущей скорости работы шины USB
		BYTE WINAPI GetUsbSpeed();
		// управления режимом низкого электропотребления модуля
		BOOL WINAPI LowPowerMode(BOOL LowPowerFlag);
		// функция выдачи строки с последней ошибкой
		BOOL WINAPI GetLastErrorInfo(LAST_ERROR_INFO_LUSBAPI * const LastErrorInfo);


		//-----------------------------------------------------------------------------
		// интерфейс модуля E20-10
		//-----------------------------------------------------------------------------

		// загрузка ПЛИС модуля
		BOOL WINAPI LOAD_MODULE(String^ fileName);
		BOOL WINAPI TEST_MODULE(WORD TestModeMask);
		BOOL WINAPI TEST_MODULE();

		// работа с АЦП
		M_ADC_PARS_E2010 WINAPI GET_ADC_PARS();
		M_ADC_PARS_E2010 WINAPI SET_ADC_PARS(M_ADC_PARS_E2010 MAdcPars, int DataStep);
		BOOL WINAPI START_ADC(void);
		BOOL WINAPI STOP_ADC(void);
		BOOL WINAPI GET_DATA_STATE(M_DATA_STATE_E2010 ^% DataState);
		BOOL WINAPI InitReading();
		BOOL WINAPI ReadDataSync(M_IO_REQUEST_LUSBAPI % ReadRequest);

		// работа с цифровыми линиями
		BOOL WINAPI ENABLE_TTL_OUT(BOOL EnableTtlOut) ;
		BOOL WINAPI TTL_IN(WORD % const TtlIn) ;
		BOOL WINAPI TTL_OUT(WORD TtlOut);

		// однократная синхронная работа с ЦАП
		BOOL WINAPI DAC_SAMPLE(SHORT * const DacData, WORD DacChannel);

		//// работа с цифровыми линиями
		//BOOL WINAPI ENABLE_TTL_OUT(BOOL EnableTtlOut);
		//BOOL WINAPI TTL_IN(WORD * const TtlIn);
		//BOOL WINAPI TTL_OUT(WORD TtlOut);

		//// функции для работы с пользовательской информацией ППЗУ
		//BOOL WINAPI ENABLE_FLASH_WRITE(BOOL IsUserFlashWriteEnabled);
		//BOOL WINAPI READ_FLASH_ARRAY(USER_FLASH_E2010 * const UserFlash);
		//BOOL WINAPI WRITE_FLASH_ARRAY(USER_FLASH_E2010 * const UserFlash);

		// информация о модуле
		M_MODULE_DESCRIPTION_E2010 WINAPI GET_MODULE_DESCRIPTION();
		BOOL WINAPI SAVE_MODULE_DESCRIPTION(MODULE_DESCRIPTION_E2010 * const ModuleDescription);

	private:
		ILE2010* pModule;

		PSHORT AdcBuffer;
		int DataStep;
		// массив OVERLAPPED структур из двух элементов
		OVERLAPPED* ReadOv;
		// массив структур с параметрами запроса на ввод/вывод данных
		IO_REQUEST_LUSBAPI* IoReq;
		DATA_STATE_E2010* DataState;
		WORD RequestNumber = 0;
		//helper methods
		M_MODULE_DESCRIPTION_E2010 Convert(MODULE_DESCRIPTION_E2010 * const ModuleDescription);
		M_VERSION_INFO_LUSBAPI Convert(VERSION_INFO_LUSBAPI * const VersionInfo);
		M_ADC_PARS_E2010 Convert(ADC_PARS_E2010 * const AdcPars);
		ADC_PARS_E2010 Convert(M_ADC_PARS_E2010^ MAdcPars);
		void Convert(M_DATA_STATE_E2010 ^% MDataState, DATA_STATE_E2010 * const DataState);
		
	};
}