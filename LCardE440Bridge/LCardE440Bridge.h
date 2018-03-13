// LCardE440Bridge.h

#pragma once

using namespace System;
#include "Lusbapi.h"
#include "LusbapiTypes.h"
#include "MLusbapiTypes.h"

namespace LCardE440Bridge
{
	public ref class E440Bridge
	{
	public:
		ModuleDescriptionE440 ModuleDescription;
		AdcParamsE440 AdcParams;

		E440Bridge();
		virtual ~E440Bridge();

		int __clrcall InitDevice(double adcRate, int duration, int chNums[], int chRanges[], int length);
		//int __clrcall ReadData();

		/*IntPtr GetModuleHandleDevice(void);
		String^ WINAPI GetModuleName();
		BYTE WINAPI GetUsbSpeed();
		bool WINAPI LoadModule();
		bool WINAPI TestModule();
		ModuleDescriptionE440 WINAPI GetModuleDescription();
		AdcParamsE440 WINAPI GetAdcParams();*/
		bool WINAPI SetAdcParams(AdcParamsE440 mAdcParams);
		bool WINAPI StartAdc(void);
		bool WINAPI StopAdc(void);

		//DWORD WINAPI ServiceReadThread(void);

		//bool WINAPI CloseLDevice(void);
		//bool WINAPI LowPowerMode(bool lowPowerFlag);
		//bool WINAPI GetLastErrorInfo(LAST_ERROR_INFO_LUSBAPI * const lastErrorInfo);

	private:
		ILE440* pModule;
		HANDLE _ModuleHandle;
		SHORT * ReadBuffer;

		bool WINAPI ReleaseLInstance(void);

		ModuleDescriptionE440 Convert(MODULE_DESCRIPTION_E440 * const moduleDescription);
		VersionInfo Convert(VERSION_INFO_LUSBAPI * const versionInfo);
		AdcParamsE440 Convert(ADC_PARS_E440 * const adcParams);
		ADC_PARS_E440 E440Bridge::Convert(AdcParamsE440^ mAdcParams);
	};
}