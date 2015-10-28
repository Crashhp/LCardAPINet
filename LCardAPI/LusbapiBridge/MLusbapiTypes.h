#pragma once

using namespace System;
using namespace System::Runtime::InteropServices;
#include "stdafx.h"
#include "Lusbapi.h"
#include "LusbapiTypes.h"
#include <stdio.h>
#include <fstream>
#include <iostream>
using namespace std;

namespace LusbapiBridgeE2010 {

	// ����� ���������� � ������ (������� �������)
	public value struct M_MODULE_INFO_LUSBAPI
	{
		String^	CompanyName;		// �������� �����-������������ �������
		String^	DeviceName; 		// �������� �������
		String^	SerialNumber;	// �������� ����� �������
		BYTE	Revision;											// ������� ������� (��������� ������)
		BYTE	Modification;										// ���������� ������ (�����);
		String^	Comment;		// ������ �����������
	};

	public value struct M_INTERFACE_INFO_LUSBAPI
	{
		BOOL	Active;			// ���� ������������� ��������� ����� ���������
		String^	Name;			// ��������
		String^	Comment;		// ������ �����������
	};

	// ���������� � ��, ���������� � ��������������� ����������: MCU, DSP, PLD � �.�.
	public value struct M_VERSION_INFO_LUSBAPI
	{
		String^ Version;		// ������ �� ��� ���������������� ����������
		String^	Date;			// ���� ������ ��
		String^ Manufacturer; 	// ������������� ��
		String^ Author;		 	// ����� ��
		String^	Comment;		// ������ �����������
	};

	// ���������� � �� MCU, ������� �������� � ���� ���������� � ���������
	// ��� �������� ���������, ��� � ����������
	public value struct M_MCU_VERSION_INFO_LUSBAPI
	{
		M_VERSION_INFO_LUSBAPI FwVersion;						// ���������� � ������ �������� �������� ��������� '����������'(Application) ����������������
		M_VERSION_INFO_LUSBAPI BlVersion;						// ���������� � ������ �������� '����������'(BootLoader) ����������������
	};


	public value struct M_MCU_INFO_LUSBAPI
	{
		BOOL	Active;												// ���� ������������� ��������� ����� ���������
		String^	Name;				// �������� ����������������
		double	ClockRate;										// �������� ������� ������ ���������������� � ���
		//		VERSION_INFO_LUSBAPI Version;							// ���������� � ������ �������� ����������������
		M_MCU_VERSION_INFO_LUSBAPI Version;										// ���������� � ������ ��� ����� �������� ����������������, ���, ��������, � �������� '����������'
		String^	Comment;		// ������ �����������
	};

	// ���������� � ���� (PLD)
	public value struct M_PLD_INFO_LUSBAPI										// PLD - Programmable Logic Device
	{
		BOOL	Active;											// ���� ������������� ��������� ����� ���������
		String^	Name;		  									// �������� ����
		double ClockRate;										// �������� ������ ������ ���� � ���
		M_VERSION_INFO_LUSBAPI Version;							// ���������� � ������ �������� ����
		String^ Comment;										// ������ �����������
	};

	// ���������� � ���
	public value struct M_ADC_INFO_LUSBAPI
	{
		BOOL	Active;												// ���� ������������� ��������� ����� ���������
		String^	Name;				// �������� ���������� ���
		array<double>^	OffsetCalibration = gcnew array<double>(ADC_CALIBR_COEFS_QUANTITY_LUSBAPI);	// ���������������� ������������ �������� ����
		array<double>^	ScaleCalibration = gcnew array<double>(ADC_CALIBR_COEFS_QUANTITY_LUSBAPI);		// ���������������� ������������ ��������
		String^	Comment;		// ������ �����������
	};

	// ��������� � ����� ����������� �� ������ E20-10
	public value struct M_MODULE_DESCRIPTION_E2010
	{
		M_MODULE_INFO_LUSBAPI     Module;		// ����� ���������� � ������
		M_INTERFACE_INFO_LUSBAPI  Interface;	// ���������� �� ����������
		M_MCU_INFO_LUSBAPI		Mcu;	// ���������� � ����������������
		M_PLD_INFO_LUSBAPI        Pld;			// ���������� � ����
		M_ADC_INFO_LUSBAPI        Adc;			// ���������� � ���
		//DAC_INFO_LUSBAPI        Dac;			// ���������� � ���
		//DIGITAL_IO_INFO_LUSBAPI DigitalIo;	// ���������� � �������� �����-������
	};

	// ��������� � ����������� ������������� ����� ������ � ���
	public value struct M_SYNCHRO_PARS_E2010
	{
		WORD	StartSource;				  	// ��� � �������� ������� ������ ����� ������ � ��� (���������� ��� ������� � �.�.)
		DWORD StartDelay; 					// �������� ������ ����� ������ � ������ �������� c ��� (��� Rev.B � ����)
		WORD	SynhroSource;					// �������� �������� ��������� ������� ��� (���������� ��� ������� � �.�.)
		DWORD StopAfterNKadrs;				// ������� ����� ������ ����� ����������� ����� ���-�� ��������� ������ �������� ��� (��� Rev.B � ����)
		WORD	SynchroAdMode;   				// ����� ���������� ������������: ������� ��� ������� (��� Rev.B � ����)
		WORD	SynchroAdChannel;				// ���������� ����� ��� ��� ���������� ������������� (��� Rev.B � ����)
		SHORT SynchroAdPorog;  				// ����� ������������ ��� ���������� ������������� (��� Rev.B � ����)
		BYTE	IsBlockDataMarkerEnabled;	// ������������ ������ ����� ������ (������, ��������, ��� ���������� ������������� ����� �� ������) (��� Rev.B � ����)
	};
	// ��������� � ����������� ������ ���
	public value struct M_ADC_PARS_E2010
	{

		BOOL IsAdcCorrectionEnabled;		// ���������� ����������� �������������� �������������� ���������� ������ �� ������ ������ (��� Rev.B � ����)
		WORD OverloadMode;					// ����� �������� ����� ���������� ������� ������� ������ (������ ��� Rev.A)
		WORD InputCurrentControl;			// ���������� ������� ����� �������� (��� Rev.B � ����)
		M_SYNCHRO_PARS_E2010 SynchroPars;	// ��������� ������������� ����� ������ � ���
		WORD ChannelsQuantity;				// ���-�� �������� ������� (������ ����� ��������)
		array<WORD>^ ControlTable = gcnew array<WORD>(MAX_CONTROL_TABLE_LENGTH_E2010);	// ����������� ������� � ��������� ����������� ��������
		array<WORD>^ InputRange = gcnew array<WORD>(ADC_CHANNELS_QUANTITY_E2010); 	// ������� ���������� �������� ���������� ���������� �������: 3.0�, 1.0� ��� 0.3�
		array<WORD>^ InputSwitch = gcnew array<WORD>(ADC_CHANNELS_QUANTITY_E2010);	// ������� ���� ����������� ���������� �������: ����� ��� ������
		double AdcRate;						// ������� ������ ��� � ���
		double InterKadrDelay;				// ����������� �������� � ��
		double KadrRate;						// ������� ����� � ���
		array<double, 2>^ AdcOffsetCoefs = gcnew array<double, 2>(ADC_INPUT_RANGES_QUANTITY_E2010, ADC_CHANNELS_QUANTITY_E2010);	// ������ ������������� ��� ������������� �������� �������� ���: (3 ���������)*(4 ������) (��� Rev.B � ����)
		array<double, 2>^ AdcScaleCoefs = gcnew array<double, 2>(ADC_INPUT_RANGES_QUANTITY_E2010,ADC_CHANNELS_QUANTITY_E2010);		// ������ ������������� ��� ������������� �������� �������� ���: (3 ���������)*(4 ������) (��� Rev.B � ����)
	};


	// ��������� � ����������� � ������� ��������� �������� ����� ������
	public value struct M_DATA_STATE_E2010
	{
		BYTE ChannelsOverFlow;			// ������� �������� ���������� ������� ���������� ������� (��� Rev.B � ����)
		BYTE BufferOverrun;				// ������� �������� ������������ ����������� ������ ������
		DWORD CurBufferFilling;			// ������������� ����������� ������ ������ Rev.B � ����, � ��������
		DWORD MaxOfBufferFilling;		// �� ����� ����� ������������ ������������� ����������� ������ ������ Rev.B � ����, � ��������
		DWORD BufferSize;					// ������ ����������� ������ ������ Rev.B � ����, � ��������
		double CurBufferFillingPercent;		// ������� ������� ���������� ����������� ������ ������ Rev.B � ����, � %
		double MaxOfBufferFillingPercent;	// �� ����� ����� ������������ ������� ���������� ����������� ������ ������ Rev.B � ����, � %
	};

	// ��������� � ����������� ������� �� ����/����� ������
	public value struct M_IO_REQUEST_LUSBAPI
	{
		array<SHORT>^ Buffer;							// ��������� �� ����� ������
		DWORD   NumberOfWordsToPass;			// ���-�� ��������, ������� ��������� ��������
		DWORD   NumberOfWordsPassed;			// �������� ���-�� ���������� ��������
		DWORD   TimeOut;						// ��� ����������� ������� - ������� � ��
	};
}