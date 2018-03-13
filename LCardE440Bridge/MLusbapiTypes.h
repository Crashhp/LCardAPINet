#pragma once

#include "stdafx.h"

using namespace System;
using namespace System::Threading;

namespace LCardE440Bridge
{
	// ����� ���������� � ������ (������� �������)
	public value struct ModuleInfo
	{
		String^ CompanyName;	// �������� �����-������������ �������
		String^ DeviceName; 	// �������� �������
		String^ SerialNumber;	// �������� ����� �������
		BYTE	Revision;		// ������� ������� (��������� ������)
		BYTE	Modification;	// ���������� ������ (�����)
		String^ Comment;		// ������ �����������
	};

	// ���������� � ������������� ���������� ��� ������� � ������
	public value struct InterfaceInfo
	{
		bool	Active;		// ���� ������������� ��������� ����� ���������
		String^	Name;		// ��������
		String^	Comment;	// ������ �����������
	};

	// ���������� � ��, ���������� � ��������������� ����������: MCU, DSP, PLD � �.�.
	public value struct VersionInfo
	{
		String^ Version;		// ������ �� ��� ���������������� ����������
		String^ Date;			// ���� ������ ��
		String^ Manufacturer; 	// ������������� ��
		String^ Author;		 	// ����� ��
		String^ Comment;		// ������ �����������
	};

	// ���������� � ����������������
	template <class VersionType>
	public value struct McuInfo
	{
		bool		Active;		// ���� ������������� ��������� ����� ���������
		String^		Name;		// �������� ����������������
		double		ClockRate;	// �������� ������� ������ ���������������� � ���
		VersionType Version;	// ���������� � ������ ��� ����� �������� ����������������, ���, ��������, � �������� '����������'
		String^		Comment;	// ������ �����������
	};

	// ���������� � DSP
	public value struct DspInfo
	{
		bool					Active;		// ���� ������������� ��������� ����� ���������
		String^					Name;		// �������� DSP
		double					ClockRate;	// �������� ������� ������ DSP � ���
		VersionInfo				Version;	// ���������� � �������� DSP
		String^					Comment;	// ������ �����������
	};

	// ���������� � ���
	public value struct AdcInfo
	{
		bool			Active;				// ���� ������������� ��������� ����� ���������
		String^			Name;				// �������� ���������� ���
		array<double>^	OffsetCalibration;	// ���������������� ������������ �������� ����
		array<double>^	ScaleCalibration;	// ���������������� ������������ ��������
		String^			Comment;			// ������ �����������
	};

	// ���������� � ���
	public value struct DacInfo
	{
		bool				Active;				// ���� ������������� ��������� ����� ���������
		String^				Name;				// �������� ���������� ���
		array<double>^		OffsetCalibration;	// ���������������� ������������
		array<double>^		ScaleCalibration;	// ���������������� ������������
		String^				Comment;			// ������ �����������
	};

	// ���������� � ��������� �����-������
	public value struct DigitalIOInfo
	{
		bool	Active;				// ���� ������������� ��������� ����� ���������
		String^	Name;				// �������� �������� ����������
		WORD	InLinesQuantity;	// ���-�� ������� �����
		WORD	OutLinesQuantity; 	// ���-�� �������� �����
		String^	Comment;			// ������ �����������
	};

	// ���������, �������� ����� ������ ��� ��� ������ E-440
	public value struct AdcParamsE440
	{
		bool			IsAdcEnabled;		 	// ������ ������ ��� (������ ��� ������)
		bool			IsCorrectionEnabled;	// ���������� ����������� �������������� ������ �� ������ �������� DSP
		WORD			AdcClockSource;			// �������� �������� ��������� ������� ���: ���������� ��� �������
		WORD			InputMode;				// ����� ����� ����� � ���
		WORD			SynchroAdType;			// ��� ���������� �������������
		WORD			SynchroAdMode; 			// ����� ���������� ������������
		WORD			SynchroAdChannel;  		// ����� ��� ��� ���������� �������������
		SHORT			SynchroAdPorog; 		// ����� ������������ ��� ��� ���������� �������������
		WORD			ChannelsQuantity;		// ����� �������� �������
		array<WORD>^	ControlTable;			// ����������� ������� � ��������� ��������
		double			AdcRate;	  			// ������� ������ ��� � ���
		double			InterKadrDelay;		  	// ����������� �������� � ��
		double			KadrRate;				// ������� ����� � ���
		WORD			AdcFifoBaseAddress;		// ������� ����� FIFO ������ ���
		WORD			AdcFifoLength;			// ����� FIFO ������ ���
		array<double>^	AdcOffsetCoefs;			// ��������	���: 4 ���������
		array<double>^	AdcScaleCoefs;			// ������� ���: 4 ���������
	};

	// ��������� � ����������� �� ������ E14-440
	public value struct ModuleDescriptionE440
	{
		ModuleInfo				Module;		// ����� ���������� � ������
		InterfaceInfo			Interface;	// ���������� �� ������������ ����������
		McuInfo<VersionInfo>	Mcu;		// ���������� � ����������������
		DspInfo					Dsp;		// ���������� � DSP
		AdcInfo					Adc;		// ���������� � ���
		DacInfo					Dac;		// ���������� � ���
		DigitalIOInfo			DigitalIo;	// ���������� � �������� �����-������
	};


	public value struct IoRequest
	{
		cli::array<SHORT>^ Buffer;				// ��������� �� ����� ������
		DWORD   NumberOfWordsToPass;			// ���-�� ��������, ������� ��������� ��������
		DWORD   NumberOfWordsPassed;			// �������� ���-�� ���������� ��������
		NativeOverlapped Overlapped;			// ��� ����������� ������� � NULL, � ��� �������������
												// ������� � ��������� �� ����������� WinAPI
												// ��������� ���� OVERLAPPED
		DWORD   TimeOut;						// ��� ����������� ������� - ������� � ��
	};
}