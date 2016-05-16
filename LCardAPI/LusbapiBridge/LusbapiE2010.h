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

		// ������� ������ ���������� ��� ������ � USB ������������
		BOOL WINAPI OpenLDevice(WORD VirtualSlot);
		BOOL WINAPI CloseLDevice(void);
		BOOL WINAPI ReleaseLInstance(void);
		// ��������� ����������� ���������� USB
		IntPtr WINAPI GetModuleHandleDevice(void);
		// ��������� �������� ������������� ������
		String^ WINAPI GetModuleName();
		// ��������� ������� �������� ������ ���� USB
		BYTE WINAPI GetUsbSpeed();
		// ���������� ������� ������� ������������������ ������
		BOOL WINAPI LowPowerMode(BOOL LowPowerFlag);
		// ������� ������ ������ � ��������� �������
		BOOL WINAPI GetLastErrorInfo(LAST_ERROR_INFO_LUSBAPI * const LastErrorInfo);


		//-----------------------------------------------------------------------------
		// ��������� ������ E20-10
		//-----------------------------------------------------------------------------

		// �������� ���� ������
		BOOL WINAPI LOAD_MODULE(String^ fileName);
		BOOL WINAPI TEST_MODULE(WORD TestModeMask);
		BOOL WINAPI TEST_MODULE();

		// ������ � ���
		M_ADC_PARS_E2010 WINAPI GET_ADC_PARS();
		M_ADC_PARS_E2010 WINAPI SET_ADC_PARS(M_ADC_PARS_E2010 MAdcPars, int DataStep);
		BOOL WINAPI START_ADC(void);
		BOOL WINAPI STOP_ADC(void);
		BOOL WINAPI GET_DATA_STATE(M_DATA_STATE_E2010 ^% DataState);
		BOOL WINAPI InitReading();
		BOOL WINAPI ReadDataSync(M_IO_REQUEST_LUSBAPI % ReadRequest);

		// ������ � ��������� �������
		BOOL WINAPI ENABLE_TTL_OUT(BOOL EnableTtlOut) ;
		BOOL WINAPI TTL_IN(WORD % const TtlIn) ;
		BOOL WINAPI TTL_OUT(WORD TtlOut);

		// ����������� ���������� ������ � ���
		BOOL WINAPI DAC_SAMPLE(SHORT * const DacData, WORD DacChannel);

		//// ������ � ��������� �������
		//BOOL WINAPI ENABLE_TTL_OUT(BOOL EnableTtlOut);
		//BOOL WINAPI TTL_IN(WORD * const TtlIn);
		//BOOL WINAPI TTL_OUT(WORD TtlOut);

		//// ������� ��� ������ � ���������������� ����������� ����
		//BOOL WINAPI ENABLE_FLASH_WRITE(BOOL IsUserFlashWriteEnabled);
		//BOOL WINAPI READ_FLASH_ARRAY(USER_FLASH_E2010 * const UserFlash);
		//BOOL WINAPI WRITE_FLASH_ARRAY(USER_FLASH_E2010 * const UserFlash);

		// ���������� � ������
		M_MODULE_DESCRIPTION_E2010 WINAPI GET_MODULE_DESCRIPTION();
		BOOL WINAPI SAVE_MODULE_DESCRIPTION(MODULE_DESCRIPTION_E2010 * const ModuleDescription);

	private:
		ILE2010* pModule;

		PSHORT AdcBuffer;
		int DataStep;
		// ������ OVERLAPPED �������� �� ���� ���������
		OVERLAPPED* ReadOv;
		// ������ �������� � ����������� ������� �� ����/����� ������
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