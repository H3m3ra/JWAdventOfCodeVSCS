# JWAoCHandlerVSCSCA (2024.12.18)

## Info
	author:  JW-Hemera
	version: v1.0 / 1.0.0.20241215221000
	status:  active unfinished

## Description
	A handler for AdventOfCode tasks written in C# by Visual Studio.

## Args
	-i    Interactive Mode
	        Wait for interactive commands and for confirmation if terminated.
	-s    Silent mode
	        Do not show any console output.
	-f    File
	        Process a given file of commands.

## Config
	An multi config.json to store multiple configs related to a program name and version.
	  If no config.json or no fitting name and version is available it will be generated.

	o task_type      Text to mark task files.
	o tasks_src      Where to search from for any task files.
	o tasks_trg      A **target path pattern** text to describe the storing for task files.
	o input_type     See task_type but for input files.
	o inputs_src     See tasks_src but for input files.
	o inputs_trg     See tasks_trg but for input files.
	o test_type      See task_type but for test files.
	o tests_src      See tasks_src but for test files.
	o tests_trg      See tasks_trg but for test files.
	o results_trg    See tasks_trg but for result files.
	o programs       Any programs.
		o type           exe | raw    
		o src            Program file path.
		o handler        If the program is linked raw this can be used to handle it anyways.
			o builder        May build an exe of a program.
			o interpreter    May execute the compiled program.
			o compiler       May compile a program.

	A **target path pattern** is a path including parameters:
		o %yy% %yyyy%              Replaced by the task year.
		o %d% %dd%                 Replaced by the task day.
		o %s% %s%                  Replaced by the sub task name.
		o %t% %tt% %ttt% %tttt%    Replaced by a part of 1 to 3 letter or the full file type name.
		o %n%                      Replaced by any increasing test number.
		O %p% %v% %a%              Replaced by any program name, program version or program author.
		Add a - or + to any to chose between lower or upper case.

		Examples when storing 1. 2024-1a x or 2. 2024-17b Input:
			o "\advent_of_code\%-tttt%s\%yy%\%d%%S%.txt"
				1. "\advent_of_code\xs\24\1a.txt"
				2. "\advent_of_code\inputs\24\17b.txt"
			o "\advent_of_code\%yyyy%\%+tt%S\%dd%%S%%yy%.txt"
				1. "\advent_of_code\2024\XS\01a24.txt"
				2. "\advent_of_code\2024\INS\17b24.txt"
			o "\advent_of_code\%yyyy%\%dd%%s%_%t%.txt"
				1. "\advent_of_code\2024\01a_x.txt"
				2. "\advent_of_code\2024\17b_I.txt"
		Examples when storing a result 1. 2024-1a Solution MyCode CurVers Me:
			o "\advent_of_code\%yyyy%\%tttt%.csv"
				1. "\advent_of_code\2024\Solution.csv"
			o "\advent_of_code\%-ttt%%yy%\%p%%v%.csv"
				1. "\advent_of_code\sol24\MyCodeCurVers.csv"
			o "\advent_of_code\%+tttt%S\%a%.csv"
				1. "\advent_of_code\SOLUTIONS\Me.csv"