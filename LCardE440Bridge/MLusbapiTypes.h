#pragma once

#include "stdafx.h"

using namespace System;
using namespace System::Threading;

namespace LCardE440Bridge
{
	// общая информация о модуле (штатный вариант)
	public value struct ModuleInfo
	{
		String^ CompanyName;	// название фирмы-изготовителя изделия
		String^ DeviceName; 	// название изделия
		String^ SerialNumber;	// серийный номер изделия
		BYTE	Revision;		// ревизия изделия (латинская литера)
		BYTE	Modification;	// исполнение модуля (число)
		String^ Comment;		// строка комментария
	};

	// информация о используемого интерфейса для доступа к модулю
	public value struct InterfaceInfo
	{
		bool	Active;		// флаг достоверности остальных полей структуры
		String^	Name;		// название
		String^	Comment;	// строка комментария
	};

	// информация о ПО, работающем в испольнительном устройстве: MCU, DSP, PLD и т.д.
	public value struct VersionInfo
	{
		String^ Version;		// версия ПО для испольнительного устройства
		String^ Date;			// дата сборки ПО
		String^ Manufacturer; 	// производитель ПО
		String^ Author;		 	// автор ПО
		String^ Comment;		// строка комментария
	};

	// информация о микроконтроллере
	template <class VersionType>
	public value struct McuInfo
	{
		bool		Active;		// флаг достоверности остальных полей структуры
		String^		Name;		// название микроконтроллера
		double		ClockRate;	// тактовая частота работы микроконтроллера в кГц
		VersionType Version;	// информация о версии как самой прошивки микроконтроллера, так, возможно, и прошивки 'Загрузчика'
		String^		Comment;	// строка комментария
	};

	// информация о DSP
	public value struct DspInfo
	{
		bool					Active;		// флаг достоверности остальных полей структуры
		String^					Name;		// название DSP
		double					ClockRate;	// тактовая частота работы DSP в кГц
		VersionInfo				Version;	// информация о драйвере DSP
		String^					Comment;	// строка комментария
	};

	// информация о АЦП
	public value struct AdcInfo
	{
		bool			Active;				// флаг достоверности остальных полей структуры
		String^			Name;				// название микросхемы АЦП
		array<double>^	OffsetCalibration;	// корректировочные коэффициенты смещения нуля
		array<double>^	ScaleCalibration;	// корректировочные коэффициенты масштаба
		String^			Comment;			// строка комментария
	};

	// информация о ЦАП
	public value struct DacInfo
	{
		bool				Active;				// флаг достоверности остальных полей структуры
		String^				Name;				// название микросхемы ЦАП
		array<double>^		OffsetCalibration;	// корректировочные коэффициенты
		array<double>^		ScaleCalibration;	// корректировочные коэффициенты
		String^				Comment;			// строка комментария
	};

	// информация о цифрового ввода-вывода
	public value struct DigitalIOInfo
	{
		bool	Active;				// флаг достоверности остальных полей структуры
		String^	Name;				// название цифровой микросхемы
		WORD	InLinesQuantity;	// кол-во входных линий
		WORD	OutLinesQuantity; 	// кол-во выходных линий
		String^	Comment;			// строка комментария
	};

	// структура, задающая режим работы АЦП для модуля E-440
	public value struct AdcParamsE440
	{
		bool			IsAdcEnabled;		 	// статус работы АЦП (только при чтении)
		bool			IsCorrectionEnabled;	// управление разрешением корректировкой данных на уровне драйвера DSP
		WORD			AdcClockSource;			// источник тактовых импульсов запуска АЦП: внутренние или внешние
		WORD			InputMode;				// режим ввода даных с АЦП
		WORD			SynchroAdType;			// тип аналоговой синхронизации
		WORD			SynchroAdMode; 			// режим аналоговой сихронизации
		WORD			SynchroAdChannel;  		// канал АЦП при аналоговой синхронизации
		SHORT			SynchroAdPorog; 		// порог срабатывания АЦП при аналоговой синхронизации
		WORD			ChannelsQuantity;		// число активных каналов
		array<WORD>^	ControlTable;			// управляющая таблица с активными каналами
		double			AdcRate;	  			// частота работы АЦП в кГц
		double			InterKadrDelay;		  	// Межкадровая задержка в мс
		double			KadrRate;				// частота кадра в кГц
		WORD			AdcFifoBaseAddress;		// базовый адрес FIFO буфера АЦП
		WORD			AdcFifoLength;			// длина FIFO буфера АЦП
		array<double>^	AdcOffsetCoefs;			// смещение	АЦП: 4 диапазона
		array<double>^	AdcScaleCoefs;			// масштаб АЦП: 4 диапазона
	};

	// структура с информацией об модуле E14-440
	public value struct ModuleDescriptionE440
	{
		ModuleInfo				Module;		// общая информация о модуле
		InterfaceInfo			Interface;	// информация об используемом интерфейсе
		McuInfo<VersionInfo>	Mcu;		// информация о микроконтроллере
		DspInfo					Dsp;		// информация о DSP
		AdcInfo					Adc;		// информация о АЦП
		DacInfo					Dac;		// информация о ЦАП
		DigitalIOInfo			DigitalIo;	// информация о цифровом вводе-выводе
	};


	public value struct IoRequest
	{
		cli::array<SHORT>^ Buffer;				// указатель на буфер данных
		DWORD   NumberOfWordsToPass;			// кол-во отсчётов, которые требуется передать
		DWORD   NumberOfWordsPassed;			// реальное кол-во переданных отсчётов
		NativeOverlapped Overlapped;			// для синхронного запроса – NULL, а для асинхроннного
												// запроса – указатель на стандартную WinAPI
												// структуру типа OVERLAPPED
		DWORD   TimeOut;						// для синхронного запроса - таймаут в мс
	};
}