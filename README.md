# JWAdventOfCode

## Info
	author:  JW-Hemera
	version: v1.0 / 1.0.0.20241204181000

## Description
	1. Contains a basic handler to solve AdventOfCode tasks.
		The handlers features are:
			+ simple access to stored tasks, inputs and tests
			+ saving results and metrics in a .csv
			+ calling extern solving programs through "JWAoCProgramInterface"
		The handler is implemented in multiple languages:
			+ JWAoCHandlerVSCSCA

	2. Presenting own solutions
		+ JWAoC2024VSCS

## JWAoCHandlerVSCSCA

## JWAoCProgramInterface
	The program interface to make a program callable by the AoC-Handler.
	The handler acts like a client sending requests to different solution services.

	Even a local program should be callable as a http-service.
	Version v1:
		o solve GET /versions    Get possible JWAoCProgramInterface versions.
		o solve GET /v1          Get specific help for the solution program.
		o solve GET /v1/author     Get the author of the solution program.
		o solve GET /v1/version    Get the version of the solution program.
		o solve GET /v1/{year}/{task_day}/{sub_task}?input={input_file_path}
			Parameters are "input", any spefic name or "args" for unnamed.
	Example inputs:
		```http GET /v1/2024/1/a?src=my%20file%20&fast=true```
	Example outputs:
		```
		http/1.1 200 OK
		...
		
		1024
		```