# JWAdventOfCode (2024.12.14)

## Info
	author:  JW-Hemera
	version: v1.0 / 1.0.0.20241204181000
	status:  active unfinished

## Description
	1. Contains a basic *handler* to organise solving AdventOfCode tasks
	  by calling solving *program*.
		The handlers features are:
			+ simple access to stored tasks, inputs and tests
			+ saving results and metrics in a .csv
			+ calling extern solving programs through "JWAoCProgramInterface"
		The handler is implemented in multiple languages:
			+ JWAoCHandlerVSCSCA - see doc/handlers/JWAoCHandlerVSCSCA for more.

	2. Presenting own solutions for AoC
		+ JWAoC2024VSCSCA (active working)
		Prepared *programs* exist also for Java, Python and Haskell.

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
