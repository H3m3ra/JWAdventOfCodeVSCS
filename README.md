# JWAdventOfCode (2024.12.24)

## Info
	author:  JWHemera
	version: - (See doc for handlers or solution)
	status:  active unfinished

## Description
	1. Contains *handler*s to organise solving AdventOfCode tasks
	  by calling any extern solving *program*.
		The *handler*'s features should be:
			+ simple access to stored tasks, inputs and tests
				+ configurate own naming system
			+ saving results and metrics in a .csv
			+ calling extern solving programs through "JWAoCProgramInterface"
				+ call raw source files by building, compiling or interpreting
				+ stop execution by a configurable timeout
				+ show errors or results
		Available *handler*s in coding languages:
			+ JWAoCHandlerVSCSCA
				v1.1 / 1.1.1.20241223213457 request JWAoCProgramInterface v1
				See doc/handlers/JWAoCHandlerVSCSCA for more.

	2. Presenting own solutions for AoC
		+ JWAoC2024VSCSCA (active unfinished)
			v1.0 / 1.0.0.20240912193200 respond JWAoCProgramInterface v1
		+ JWAoC2024PyCA (active unfinished)
			v1.0 / 1.0.0.20241218184700 respond JWAoCProgramInterface v1
		Prepared *programs* exist also for Java and Haskell.

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
	 o-build       See for builds of a *handler* or *program*.
	 |
	 o-doc         Documentation.
	 |
	 o-examples    Examples. (Search own solutions under src or build)
	 | |
	 | o-handler/config.json    An example for a handler config.
	 |
	 o-src         Source code.
