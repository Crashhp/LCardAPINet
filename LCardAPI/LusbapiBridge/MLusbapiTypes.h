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

	// общая информация о модуле (штатный вариант)
	public value struct M_MODULE_INFO_LUSBAPI
	{
		String^	CompanyName;		// название фирмы-изготовителя изделия
		String^	DeviceName; 		// название изделия
		String^	SerialNumber;	// серийный номер изделия
		BYTE	Revision;											// ревизия изделия (латинская литера)
		BYTE	Modification;										// исполнение модуля (число);
		String^	Comment;		// строка комментария
	};

	public value struct M_INTERFACE_INFO_LUSBAPI
	{
		BOOL	Active;			// флаг достоверности остальных полей структуры
		String^	Name;			// название
		String^	Comment;		// строка комментария
	};

	// информация о ПО, работающем в испольнительном устройстве: MCU, DSP, PLD и т.д.
	public value struct M_VERSION_INFO_LUSBAPI
	{
		String^ Version;		// версия ПО для испольнительного устройства
		String^	Date;			// дата сборки ПО
		String^ Manufacturer; 	// производитель ПО
		String^ Author;		 	// автор ПО
		String^	Comment;		// строка комментария
	};

	// информация о ПО MCU, которая включает в себя информацию о прошивках
	// как основной программы, так и загрузчика
	public value struct M_MCU_VERSION_INFO_LUSBAPI
	{
		M_VERSION_INFO_LUSBAPI FwVersion;						// информация о версии прошивки основной программы 'Приложение'(Application) микроконтроллера
		M_VERSION_INFO_LUSBAPI BlVersion;						// информация о версии прошивки 'Загрузчика'(BootLoader) микроконтроллера
	};


	public value struct M_MCU_INFO_LUSBAPI
	{
		BOOL	Active;												// флаг достоверности остальных полей структуры
		String^	Name;				// название микроконтроллера
		double	ClockRate;										// тактовая частота работы микроконтроллера в кГц
		//		VERSION_INFO_LUSBAPI Version;							// информация о версии прошивки микроконтроллера
		M_MCU_VERSION_INFO_LUSBAPI Version;										// информация о версии как самой прошивки микроконтроллера, так, возможно, и прошивки 'Загрузчика'
		String^	Comment;		// строка комментария
	};

	// информация о ПЛИС (PLD)
	public value struct M_PLD_INFO_LUSBAPI										// PLD - Programmable Logic Device
	{
		BOOL	Active;											// флаг достоверности остальных полей структуры
		String^	Name;		  									// название ПЛИС
		double ClockRate;										// тактовая чатота работы ПЛИС в кГц
		M_VERSION_INFO_LUSBAPI Version;							// информация о версии прошивки ПЛИС
		String^ Comment;										// строка комментария
	};

	// информация о АЦП
	public value struct M_ADC_INFO_LUSBAPI
	{
		BOOL	Active;												// флаг достоверности остальных полей структуры
		String^	Name;				// название микросхемы АЦП
		array<double>^	OffsetCalibration = gcnew array<double>(ADC_CALIBR_COEFS_QUANTITY_LUSBAPI);	// корректировочные коэффициенты смещения нуля
		array<double>^	ScaleCalibration = gcnew array<double>(ADC_CALIBR_COEFS_QUANTITY_LUSBAPI);		// корректировочные коэффициенты масштаба
		String^	Comment;		// строка комментария
	};

	// структура с общей информацией об модуле E20-10
	public value struct M_MODULE_DESCRIPTION_E2010
	{
		M_MODULE_INFO_LUSBAPI     Module;		// общая информация о модуле
		M_INTERFACE_INFO_LUSBAPI  Interface;	// информация об интерфейсе
		M_MCU_INFO_LUSBAPI		Mcu;	// информация о микроконтроллере
		M_PLD_INFO_LUSBAPI        Pld;			// информация о ПЛИС
		M_ADC_INFO_LUSBAPI        Adc;			// информация о АЦП
		//DAC_INFO_LUSBAPI        Dac;			// информация о ЦАП
		//DIGITAL_IO_INFO_LUSBAPI DigitalIo;	// информация о цифровом вводе-выводе
	};

	// структура с параметрами синхронизации ввода данных с АЦП
	public value struct M_SYNCHRO_PARS_E2010
	{
		WORD	StartSource;				  	// тип и источник сигнала начала сбора данных с АЦП (внутренний или внешний и т.д.)
		DWORD StartDelay; 					// задержка старта сбора данных в кадрах отсчётов c АЦП (для Rev.B и выше)
		WORD	SynhroSource;					// источник тактовых импульсов запуска АЦП (внутренние или внешние и т.д.)
		DWORD StopAfterNKadrs;				// останов сбора данных после задаваемого здесь кол-ва собранных кадров отсчётов АЦП (для Rev.B и выше)
		WORD	SynchroAdMode;   				// режим аналоговой сихронизации: переход или уровень (для Rev.B и выше)
		WORD	SynchroAdChannel;				// физический канал АЦП для аналоговой синхронизации (для Rev.B и выше)
		SHORT SynchroAdPorog;  				// порог срабатывания при аналоговой синхронизации (для Rev.B и выше)
		BYTE	IsBlockDataMarkerEnabled;	// маркирование начала блока данных (удобно, например, при аналоговой синхронизации ввода по уровню) (для Rev.B и выше)
	};
	// структура с параметрами работы АЦП
	public value struct M_ADC_PARS_E2010
	{

		BOOL IsAdcCorrectionEnabled;		// управление разрешением автоматической корректировкой получаемых данных на уровне модуля (для Rev.B и выше)
		WORD OverloadMode;					// режим фиксации факта перегрузки входных каналов модуля (только для Rev.A)
		WORD InputCurrentControl;			// управление входным током смещения (для Rev.B и выше)
		M_SYNCHRO_PARS_E2010 SynchroPars;	// параметры синхронизации ввода данных с АЦП
		WORD ChannelsQuantity;				// кол-во активных каналов (размер кадра отсчётов)
		array<WORD>^ ControlTable = gcnew array<WORD>(MAX_CONTROL_TABLE_LENGTH_E2010);	// управляющая таблица с активными логическими каналами
		array<WORD>^ InputRange = gcnew array<WORD>(ADC_CHANNELS_QUANTITY_E2010); 	// индексы диапазонов входного напряжения физических каналов: 3.0В, 1.0В или 0.3В
		array<WORD>^ InputSwitch = gcnew array<WORD>(ADC_CHANNELS_QUANTITY_E2010);	// индексы типа подключения физических каналов: земля или сигнал
		double AdcRate;						// частота работы АЦП в кГц
		double InterKadrDelay;				// межкадровая задержка в мс
		double KadrRate;						// частота кадра в кГц
		array<double, 2>^ AdcOffsetCoefs = gcnew array<double, 2>(ADC_INPUT_RANGES_QUANTITY_E2010, ADC_CHANNELS_QUANTITY_E2010);	// массив коэффициентов для корректировки смещение отсчётов АЦП: (3 диапазона)*(4 канала) (для Rev.B и выше)
		array<double, 2>^ AdcScaleCoefs = gcnew array<double, 2>(ADC_INPUT_RANGES_QUANTITY_E2010,ADC_CHANNELS_QUANTITY_E2010);		// массив коэффициентов для корректировки масштаба отсчётов АЦП: (3 диапазона)*(4 канала) (для Rev.B и выше)
	};


	// структура с информацией о текущем состоянии процесса сбора данных
	public value struct M_DATA_STATE_E2010
	{
		BYTE ChannelsOverFlow;			// битовые признаки перегрузки входных аналоговых каналов (для Rev.B и выше)
		BYTE BufferOverrun;				// битовые признаки переполнения внутреннего буфера модуля
		DWORD CurBufferFilling;			// заполненность внутреннего буфера модуля Rev.B и выше, в отсчётах
		DWORD MaxOfBufferFilling;		// за время сбора максимальная заполненность внутреннего буфера модуля Rev.B и выше, в отсчётах
		DWORD BufferSize;					// размер внутреннего буфера модуля Rev.B и выше, в отсчётах
		double CurBufferFillingPercent;		// текущая степень заполнения внутреннего буфера модуля Rev.B и выше, в %
		double MaxOfBufferFillingPercent;	// за время сбора максимальная степень заполнения внутреннего буфера модуля Rev.B и выше, в %
	};

	// структура с параметрами запроса на ввод/вывод данных
	public value struct M_IO_REQUEST_LUSBAPI
	{
		array<SHORT>^ Buffer;							// указатель на буфер данных
		DWORD   NumberOfWordsToPass;			// кол-во отсчётов, которые требуется передать
		DWORD   NumberOfWordsPassed;			// реальное кол-во переданных отсчётов
		DWORD   TimeOut;						// для синхронного запроса - таймаут в мс
	};
}