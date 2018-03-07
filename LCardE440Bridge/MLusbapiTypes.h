#pragma once

#include "stdafx.h"

using namespace System;

namespace LCardE440Bridge
{
	// общая информация о модуле (штатный вариант)
	public value struct M_MODULE_INFO_LUSBAPI
	{
		String^ CompanyName;	// название фирмы-изготовителя изделия
		String^ DeviceName; 	// название изделия
		String^ SerialNumber;	// серийный номер изделия
		BYTE	Revision;		// ревизия изделия (латинская литера)
		BYTE	Modification;	// исполнение модуля (число)
		String^ Comment;		// строка комментария
	};

	// информация о используемого интерфейса для доступа к модулю
	public value struct M_INTERFACE_INFO_LUSBAPI
	{
		BOOL	Active;		// флаг достоверности остальных полей структуры
		String^	Name;		// название
		String^	Comment;	// строка комментария
	};

	// информация о ПО, работающем в испольнительном устройстве: MCU, DSP, PLD и т.д.
	public value struct M_VERSION_INFO_LUSBAPI
	{
		String^ Version;		// версия ПО для испольнительного устройства
		String^ Date;			// дата сборки ПО
		String^ Manufacturer; 	// производитель ПО
		String^ Author;		 	// автор ПО
		String^ Comment;		// строка комментария
	};

	// информация о микроконтроллере
	//template <class VersionType>
	public value struct M_MCU_INFO_LUSBAPI
	{
		BOOL		Active;		// флаг достоверности остальных полей структуры
		String^		Name;		// название микроконтроллера
		double		ClockRate;	// тактовая частота работы микроконтроллера в кГц
		M_VERSION_INFO_LUSBAPI Version;	// информация о версии как самой прошивки микроконтроллера, так, возможно, и прошивки 'Загрузчика'
										//VersionType Version;	// информация о версии как самой прошивки микроконтроллера, так, возможно, и прошивки 'Загрузчика'
		String^		Comment;	// строка комментария
	};

	// информация о DSP
	public value struct M_DSP_INFO_LUSBAPI
	{
		BOOL					Active;		// флаг достоверности остальных полей структуры
		String^					Name;		// название DSP
		double					ClockRate;	// тактовая частота работы DSP в кГц
		M_VERSION_INFO_LUSBAPI	Version;	// информация о драйвере DSP
		String^					Comment;	// строка комментария
	};

	// информация о АЦП
	public value struct M_ADC_INFO_LUSBAPI
	{
		BOOL			Active;																		// флаг достоверности остальных полей структуры
		String^			Name;																		// название микросхемы АЦП
		cli::array<double>^	OffsetCalibration = gcnew cli::array<double>(ADC_CALIBR_COEFS_QUANTITY_LUSBAPI);	// корректировочные коэффициенты смещения нуля
		cli::array<double>^	ScaleCalibration = gcnew cli::array<double>(ADC_CALIBR_COEFS_QUANTITY_LUSBAPI);	// корректировочные коэффициенты масштаба
		String^			Comment;																	// строка комментария
	};

	// информация о ЦАП
	public value struct M_DAC_INFO_LUSBAPI
	{
		BOOL			Active;																		// флаг достоверности остальных полей структуры
		String^			Name;																		// название микросхемы ЦАП
		cli::array<double>^	OffsetCalibration = gcnew cli::array<double>(DAC_CALIBR_COEFS_QUANTITY_LUSBAPI);	// корректировочные коэффициенты
		cli::array<double>^	ScaleCalibration = gcnew cli::array<double>(DAC_CALIBR_COEFS_QUANTITY_LUSBAPI);	// корректировочные коэффициенты
		String^			Comment;																	// строка комментария
	};

	// информация о цифрового ввода-вывода
	public value struct M_DIGITAL_IO_INFO_LUSBAPI
	{
		BOOL	Active;				// флаг достоверности остальных полей структуры
		String^	Name;				// название цифровой микросхемы
		WORD	InLinesQuantity;	// кол-во входных линий
		WORD	OutLinesQuantity; 	// кол-во выходных линий
		String^	Comment;			// строка комментария
	};

	// структура, задающая режим работы АЦП для модуля E-440
	public value struct M_ADC_PARS_E440
	{
		BOOL			IsAdcEnabled;		 													// статус работы АЦП (только при чтении)
		BOOL			IsCorrectionEnabled;													// управление разрешением корректировкой данных на уровне драйвера DSP
		WORD			AdcClockSource;															// источник тактовых импульсов запуска АЦП: внутренние или внешние
		WORD			InputMode;																// режим ввода даных с АЦП
		WORD			SynchroAdType;															// тип аналоговой синхронизации
		WORD			SynchroAdMode; 															// режим аналоговой сихронизации
		WORD			SynchroAdChannel;  														// канал АЦП при аналоговой синхронизации
		SHORT			SynchroAdPorog; 														// порог срабатывания АЦП при аналоговой синхронизации
		WORD			ChannelsQuantity;														// число активных каналов
		cli::array<WORD>^	ControlTable = gcnew cli::array<WORD>(MAX_CONTROL_TABLE_LENGTH_E440);		// управляющая таблица с активными каналами
		double			AdcRate;	  			  												// частота работы АЦП в кГц
		double			InterKadrDelay;		  													// Межкадровая задержка в мс
		double			KadrRate;																// частота кадра в кГц
		WORD			AdcFifoBaseAddress;														// базовый адрес FIFO буфера АЦП
		WORD			AdcFifoLength;															// длина FIFO буфера АЦП
		cli::array<double>^	AdcOffsetCoefs = gcnew cli::array<double>(ADC_CALIBR_COEFS_QUANTITY_E440);	// смещение	АЦП: 4диапазона
		cli::array<double>^	AdcScaleCoefs = gcnew cli::array<double>(ADC_CALIBR_COEFS_QUANTITY_E440);	// масштаб АЦП	: 4диапазона
	};

	// структура с информацией об модуле E14-440
	public value struct M_MODULE_DESCRIPTION_E440
	{
		M_MODULE_INFO_LUSBAPI						Module;		// общая информация о модуле
		M_INTERFACE_INFO_LUSBAPI					Interface;	// информация об используемом интерфейсе
		M_MCU_INFO_LUSBAPI/*<M_VERSION_INFO_LUSBAPI>*/	Mcu;		// информация о микроконтроллере
		M_DSP_INFO_LUSBAPI							Dsp;		// информация о DSP
		M_ADC_INFO_LUSBAPI							Adc;		// информация о АЦП
		M_DAC_INFO_LUSBAPI							Dac;		// информация о ЦАП
		M_DIGITAL_IO_INFO_LUSBAPI					DigitalIo;	// информация о цифровом вводе-выводе
	};
}