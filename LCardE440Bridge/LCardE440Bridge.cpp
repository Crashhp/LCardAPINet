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

	M_MODULE_DESCRIPTION_E440 E440Bridge::Convert(MODULE_DESCRIPTION_E440 * const moduleDescription)
	{
		M_MODULE_DESCRIPTION_E440 res;

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
		res.Adc.OffsetCalibration = gcnew cli::array<double>(ADC_CALIBR_COEFS_QUANTITY_LUSBAPI);
		res.Adc.ScaleCalibration = gcnew cli::array<double>(ADC_CALIBR_COEFS_QUANTITY_LUSBAPI);
		for (int i = 0; i < ADC_CALIBR_COEFS_QUANTITY_LUSBAPI; i++)
		{
			res.Adc.OffsetCalibration[i] = moduleDescription->Adc.OffsetCalibration[i];
			res.Adc.ScaleCalibration[i] = moduleDescription->Adc.ScaleCalibration[i];
		}
		res.Adc.Comment = gcnew System::String(reinterpret_cast<PCHAR>(moduleDescription->Adc.Comment));

		// DAC_INFO_LUSBAPI
		res.Dac.Active = moduleDescription->Dac.Active;
		res.Dac.Name = gcnew System::String(reinterpret_cast<PCHAR>(moduleDescription->Dac.Name));
		res.Dac.OffsetCalibration = gcnew cli::array<double>(DAC_CALIBR_COEFS_QUANTITY_LUSBAPI);
		res.Dac.ScaleCalibration = gcnew cli::array<double>(DAC_CALIBR_COEFS_QUANTITY_LUSBAPI);
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

	M_VERSION_INFO_LUSBAPI E440Bridge::Convert(VERSION_INFO_LUSBAPI * const versionInfo)
	{
		M_VERSION_INFO_LUSBAPI res;
		res.Version = gcnew System::String(reinterpret_cast<PCHAR>(versionInfo->Version));
		res.Date = gcnew System::String(reinterpret_cast<PCHAR>(versionInfo->Date));
		res.Manufacturer = gcnew System::String(reinterpret_cast<PCHAR>(versionInfo->Manufacturer));
		res.Author = gcnew System::String(reinterpret_cast<PCHAR>(versionInfo->Author));
		res.Comment = gcnew System::String(reinterpret_cast<PCHAR>(versionInfo->Comment));
		return res;
	}

	M_ADC_PARS_E440 E440Bridge::Convert(ADC_PARS_E440 * const adcParams)
	{
		M_ADC_PARS_E440 res;

		res.IsAdcEnabled = adcParams->IsAdcEnabled;
		res.IsCorrectionEnabled = adcParams->IsCorrectionEnabled;
		res.AdcClockSource = adcParams->AdcClockSource;
		res.InputMode = adcParams->InputMode;
		res.SynchroAdType = adcParams->SynchroAdType;
		res.SynchroAdMode = adcParams->SynchroAdMode;
		res.SynchroAdChannel = adcParams->SynchroAdChannel;
		res.SynchroAdPorog = adcParams->SynchroAdPorog;
		res.ChannelsQuantity = adcParams->ChannelsQuantity;
		for (int i = 0; i < MAX_CONTROL_TABLE_LENGTH_E440; i++)
		{
			res.ControlTable[i] = adcParams->ControlTable[i];
		}
		res.AdcRate = adcParams->AdcRate;
		res.InterKadrDelay = adcParams->InterKadrDelay;
		res.KadrRate = adcParams->KadrRate;
		res.AdcFifoBaseAddress = adcParams->AdcFifoBaseAddress;
		res.AdcFifoLength = adcParams->AdcFifoLength;
		for (int i = 0; i < ADC_CALIBR_COEFS_QUANTITY_E440; i++)
		{
			res.AdcOffsetCoefs[i] = adcParams->AdcOffsetCoefs[i];
			res.AdcScaleCoefs[i] = adcParams->AdcScaleCoefs[i];
		}

		return res;
	}

	ADC_PARS_E440 E440Bridge::Convert(M_ADC_PARS_E440^ mAdcParams)
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

	DWORD E440Bridge::DllVersion()
	{
		return GetDllVersion();
	}

	bool E440Bridge::CreateInstance()
	{
		pModule = static_cast<ILE440 *>(CreateLInstance("e440"));
		return pModule != nullptr;
	}

	bool E440Bridge::OpenLDevice(WORD virtualSlot)
	{
		if (virtualSlot >= 0 && virtualSlot <= MAX_VIRTUAL_SLOTS_QUANTITY_LUSBAPI)
			return pModule->OpenLDevice(virtualSlot) != 0;

		return false;
	}

	IntPtr E440Bridge::GetModuleHandleDevice(void)
	{
		return IntPtr(pModule->GetModuleHandle());
	}

	String^ E440Bridge::GetModuleName()
	{
		char moduleName[7];
		String^ name = gcnew System::String("");

		if (pModule->GetModuleName(moduleName))
			name = gcnew System::String(moduleName);

		return name;
	}

	BYTE E440Bridge::GetUsbSpeed()
	{
		BYTE Usb;

		if (pModule->GetUsbSpeed(&Usb))
			return Usb;

		return -1;
	}

	bool E440Bridge::LoadModule()
	{
		return pModule->LOAD_MODULE() != 0;
	}

	bool E440Bridge::TestModule()
	{
		return pModule->TEST_MODULE() != 0;
	}

	M_MODULE_DESCRIPTION_E440 E440Bridge::GetModuleDescription()
	{
		MODULE_DESCRIPTION_E440 moduleDescription;
		M_MODULE_DESCRIPTION_E440 convertDescription;

		if (pModule->GET_MODULE_DESCRIPTION(&moduleDescription))
			convertDescription = this->Convert(&moduleDescription);

		return convertDescription;
	}

	M_ADC_PARS_E440 WINAPI E440Bridge::GET_ADC_PARS()
	{
		ADC_PARS_E440 adcParams;
		M_ADC_PARS_E440 convertParams;

		if (pModule->GET_ADC_PARS(&adcParams))
			convertParams = this->Convert(&adcParams);

		return convertParams;
	}

	bool WINAPI E440Bridge::SET_ADC_PARS(M_ADC_PARS_E440 mAdcParams, int dataStep)
	{
		ADC_PARS_E440 adcParams = Convert(mAdcParams);

		return pModule->SET_ADC_PARS(&adcParams) != 0;
	}

	bool WINAPI E440Bridge::START_ADC(void)
	{
		return pModule->START_ADC() != 0;
	}

	bool WINAPI E440Bridge::STOP_ADC(void)
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