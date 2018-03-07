#pragma once

#include "stdafx.h"

using namespace System;

namespace LCardE440Bridge
{
	// ����� ���������� � ������ (������� �������)
	public value struct M_MODULE_INFO_LUSBAPI
	{
		String^ CompanyName;	// �������� �����-������������ �������
		String^ DeviceName; 	// �������� �������
		String^ SerialNumber;	// �������� ����� �������
		BYTE	Revision;		// ������� ������� (��������� ������)
		BYTE	Modification;	// ���������� ������ (�����)
		String^ Comment;		// ������ �����������
	};

	// ���������� � ������������� ���������� ��� ������� � ������
	public value struct M_INTERFACE_INFO_LUSBAPI
	{
		BOOL	Active;		// ���� ������������� ��������� ����� ���������
		String^	Name;		// ��������
		String^	Comment;	// ������ �����������
	};

	// ���������� � ��, ���������� � ��������������� ����������: MCU, DSP, PLD � �.�.
	public value struct M_VERSION_INFO_LUSBAPI
	{
		String^ Version;		// ������ �� ��� ���������������� ����������
		String^ Date;			// ���� ������ ��
		String^ Manufacturer; 	// ������������� ��
		String^ Author;		 	// ����� ��
		String^ Comment;		// ������ �����������
	};

	// ���������� � ����������������
	//template <class VersionType>
	public value struct M_MCU_INFO_LUSBAPI
	{
		BOOL		Active;		// ���� ������������� ��������� ����� ���������
		String^		Name;		// �������� ����������������
		double		ClockRate;	// �������� ������� ������ ���������������� � ���
		M_VERSION_INFO_LUSBAPI Version;	// ���������� � ������ ��� ����� �������� ����������������, ���, ��������, � �������� '����������'
										//VersionType Version;	// ���������� � ������ ��� ����� �������� ����������������, ���, ��������, � �������� '����������'
		String^		Comment;	// ������ �����������
	};

	// ���������� � DSP
	public value struct M_DSP_INFO_LUSBAPI
	{
		BOOL					Active;		// ���� ������������� ��������� ����� ���������
		String^					Name;		// �������� DSP
		double					ClockRate;	// �������� ������� ������ DSP � ���
		M_VERSION_INFO_LUSBAPI	Version;	// ���������� � �������� DSP
		String^					Comment;	// ������ �����������
	};

	// ���������� � ���
	public value struct M_ADC_INFO_LUSBAPI
	{
		BOOL			Active;																		// ���� ������������� ��������� ����� ���������
		String^			Name;																		// �������� ���������� ���
		cli::array<double>^	OffsetCalibration = gcnew cli::array<double>(ADC_CALIBR_COEFS_QUANTITY_LUSBAPI);	// ���������������� ������������ �������� ����
		cli::array<double>^	ScaleCalibration = gcnew cli::array<double>(ADC_CALIBR_COEFS_QUANTITY_LUSBAPI);	// ���������������� ������������ ��������
		String^			Comment;																	// ������ �����������
	};

	// ���������� � ���
	public value struct M_DAC_INFO_LUSBAPI
	{
		BOOL			Active;																		// ���� ������������� ��������� ����� ���������
		String^			Name;																		// �������� ���������� ���
		cli::array<double>^	OffsetCalibration = gcnew cli::array<double>(DAC_CALIBR_COEFS_QUANTITY_LUSBAPI);	// ���������������� ������������
		cli::array<double>^	ScaleCalibration = gcnew cli::array<double>(DAC_CALIBR_COEFS_QUANTITY_LUSBAPI);	// ���������������� ������������
		String^			Comment;																	// ������ �����������
	};

	// ���������� � ��������� �����-������
	public value struct M_DIGITAL_IO_INFO_LUSBAPI
	{
		BOOL	Active;				// ���� ������������� ��������� ����� ���������
		String^	Name;				// �������� �������� ����������
		WORD	InLinesQuantity;	// ���-�� ������� �����
		WORD	OutLinesQuantity; 	// ���-�� �������� �����
		String^	Comment;			// ������ �����������
	};

	// ���������, �������� ����� ������ ��� ��� ������ E-440
	public value struct M_ADC_PARS_E440
	{
		BOOL			IsAdcEnabled;		 													// ������ ������ ��� (������ ��� ������)
		BOOL			IsCorrectionEnabled;													// ���������� ����������� �������������� ������ �� ������ �������� DSP
		WORD			AdcClockSource;															// �������� �������� ��������� ������� ���: ���������� ��� �������
		WORD			InputMode;																// ����� ����� ����� � ���
		WORD			SynchroAdType;															// ��� ���������� �������������
		WORD			SynchroAdMode; 															// ����� ���������� ������������
		WORD			SynchroAdChannel;  														// ����� ��� ��� ���������� �������������
		SHORT			SynchroAdPorog; 														// ����� ������������ ��� ��� ���������� �������������
		WORD			ChannelsQuantity;														// ����� �������� �������
		cli::array<WORD>^	ControlTable = gcnew cli::array<WORD>(MAX_CONTROL_TABLE_LENGTH_E440);		// ����������� ������� � ��������� ��������
		double			AdcRate;	  			  												// ������� ������ ��� � ���
		double			InterKadrDelay;		  													// ����������� �������� � ��
		double			KadrRate;																// ������� ����� � ���
		WORD			AdcFifoBaseAddress;														// ������� ����� FIFO ������ ���
		WORD			AdcFifoLength;															// ����� FIFO ������ ���
		cli::array<double>^	AdcOffsetCoefs = gcnew cli::array<double>(ADC_CALIBR_COEFS_QUANTITY_E440);	// ��������	���: 4���������
		cli::array<double>^	AdcScaleCoefs = gcnew cli::array<double>(ADC_CALIBR_COEFS_QUANTITY_E440);	// ������� ���	: 4���������
	};

	// ��������� � ����������� �� ������ E14-440
	public value struct M_MODULE_DESCRIPTION_E440
	{
		M_MODULE_INFO_LUSBAPI						Module;		// ����� ���������� � ������
		M_INTERFACE_INFO_LUSBAPI					Interface;	// ���������� �� ������������ ����������
		M_MCU_INFO_LUSBAPI/*<M_VERSION_INFO_LUSBAPI>*/	Mcu;		// ���������� � ����������������
		M_DSP_INFO_LUSBAPI							Dsp;		// ���������� � DSP
		M_ADC_INFO_LUSBAPI							Adc;		// ���������� � ���
		M_DAC_INFO_LUSBAPI							Dac;		// ���������� � ���
		M_DIGITAL_IO_INFO_LUSBAPI					DigitalIo;	// ���������� � �������� �����-������
	};
}