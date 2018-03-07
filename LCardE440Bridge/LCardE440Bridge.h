// LCardE440Bridge.h

#pragma once

using namespace System;
#include "Lusbapi.h"
#include "LusbapiTypes.h"
#include "MLusbapiTypes.h"

namespace LCardE440Bridge
{
	//public enum LUsbSpeed
	//{
	//	USB11_LUSBAPI = 0,
	//	USB20_LUSBAPI,
	//	INVALID_USB_SPEED_LUSBAPI
	//};

	//public enum LError
	//{
	//	COMPLETE_WITHOUT_ERROR = 0x00,
	//	DLL_VERSION_ERROR = 0x01,
	//	MODULE_INTERFACE_ERROR = 0x02,
	//	OPEN_DEVICE_ERROR = 0x03,
	//	INVALID_MODULE_HANDLE_ERROR = 0x04,
	//	GET_MODULE_NAME_ERROR = 0x05,
	//	NOT_E440_MODULE_ERROR = 0x06,
	//	USB_SPEED_ERROR = 0x07,
	//	LOAD_MODULE_ERROR = 0x08
	//	
	//};

	public ref class E440Bridge
	{

	public:
		E440Bridge();
		virtual ~E440Bridge();

		DWORD WINAPI DllVersion();
		bool WINAPI CreateInstance();
		bool WINAPI OpenLDevice(WORD virtualSlot);
		IntPtr GetModuleHandleDevice(void);
		String^ WINAPI GetModuleName();
		BYTE WINAPI GetUsbSpeed();
		bool WINAPI LoadModule();
		bool WINAPI TestModule();
		M_MODULE_DESCRIPTION_E440 WINAPI GetModuleDescription();
		M_ADC_PARS_E440 WINAPI GET_ADC_PARS();
		bool WINAPI SET_ADC_PARS(M_ADC_PARS_E440 mAdcParams, int dataStep);
		bool WINAPI E440Bridge::START_ADC(void);
		bool WINAPI E440Bridge::STOP_ADC(void);

		//bool WINAPI CloseLDevice(void);
		//bool WINAPI ReleaseLInstance(void);
		//bool WINAPI LowPowerMode(bool lowPowerFlag);
		//bool WINAPI GetLastErrorInfo(LAST_ERROR_INFO_LUSBAPI * const lastErrorInfo);

	private:
		ILE440* pModule;

		bool WINAPI ReleaseLInstance(void);

		M_MODULE_DESCRIPTION_E440 Convert(MODULE_DESCRIPTION_E440 * const moduleDescription);
		M_VERSION_INFO_LUSBAPI Convert(VERSION_INFO_LUSBAPI * const versionInfo);
		M_ADC_PARS_E440 Convert(ADC_PARS_E440 * const adcParams);
		ADC_PARS_E440 E440Bridge::Convert(M_ADC_PARS_E440^ mAdcParams);
	};
}