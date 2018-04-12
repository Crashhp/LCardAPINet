// LCardE440Bridge.h

#pragma once

using namespace System;
using namespace System::Threading::Tasks;
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

		int __clrcall InitDevice(double adcRate, int duration, array<int>^ chNums, array<int>^ chRanges, int length);
		int __clrcall ReadData();
		array<short>^ GetResult();

	private:
		ILE440* pModule;
		HANDLE _ModuleHandle;
		SHORT * _ReadBuffer;
		int _NDataBlock, _DataStep;
		array<short>^ _Data;

		bool WINAPI ReleaseLInstance(void);

		ModuleDescriptionE440 Convert(MODULE_DESCRIPTION_E440 * const moduleDescription);
		VersionInfo Convert(VERSION_INFO_LUSBAPI * const versionInfo);
		AdcParamsE440 Convert(ADC_PARS_E440 * const adcParams);
		ADC_PARS_E440 Convert(AdcParamsE440^ mAdcParams);
	};
}