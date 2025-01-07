# JWAdventOfCode (2025.01.07)

## Info
	author:  JWHemera
	version: - (See doc for handlers or solution)
	status:  active unfinished

## Description
	A project for [Advent of Code](https://adventofcode.com/) coding tasks.
	This project separates solving AoC tasks in two different parts:
		1. A program (**handler**, client) to organize storing puzzle inputs
		  and tests and start solving requests to different programs.
		2. A program (**program**, service(s)) to solve tasks.
	This creates the work to choose/create a **handler** and **program**s with
	  handling "JWAoCProgramInterface" requests/responses but
	gain the freedom of independent implementation in any coding language
	  without wasting time in reimplementing the correct puzzle input access
	  or execution of tests.
	  Solutions can be distributed or stored as completed program
	  and their calculated results can be compared and stored without any
	  additional daily effort.

	1. Contains **handler**s to organize solving AdventOfCode tasks
	  by calling any extern solving **program**.
		The **handler**'s features should be:
			+ simple access to stored tasks, inputs and tests
				+ configurate own naming system
			+ saving results and metrics in a .csv
			+ calling extern solving programs through "JWAoCProgramInterface"
				+ call raw source files by building, compiling or interpreting
				+ stop execution by a configurable timeout
				+ show errors or results
		Available **handler**s in coding languages:
			+ JWAoCHandlerVSCSCA
				v0.3.1 / 0.3.1.20250107211407 request JWAoCProgramInterface v1
				See doc/handlers/JWAoCHandlerVSCSCA for more.

	2. Presenting own solutions for AoC
		+ JWAoC2024VSCSCA (active unfinished)
			v1.0.2 / 1.0.2.20250105104438 respond JWAoCProgramInterface v1
		+ JWAoC2024VSCSCA (active unfinished)
			v1.0.0 / 1.0.0.20240912193200 respond JWAoCProgramInterface v1
		+ JWAoC2024PyCA (active unfinished)
			v1.0.0 / 1.0.0.20241218184700 respond JWAoCProgramInterface v1
		Prepared **programs** exist also for Java and Haskell.

## JWAoCProgramInterface

	See doc/JWAoCProgramInterface for more.

## Structure

	Different programming languages:
		o C#         CSharp CS        .cs
			o Visual Studio CS  VSCS  .cs
		o Java       Java             .java/.class/.jar
		o Haskell    HS               .hs/.o/.hi
		o Python     Py               .py

	Folders:
	 o-build       See for builds of a **handler** or **program**.
	 |
	 o-doc         Documentation.
	 |
	 o-examples    Examples. (Search own solutions under src or build)
	 | |
	 | o-handler/config.json    An example for a handler config.
	 |
	 o-src         Source code.
