# JWAoCProgramInterface (2024.12.14)

	The *program interface* to make any AoC *program* callable by an AoC *handler*
	  separating organized execution and implemented solving of AoC tasks.


## Local http-like call

	The *handler* acts like a client sending requests to different solution services
	  called *program* that respond with writing to the standard output.
	The start with leading argument "http" should start this process and terminate
	  with any http responding message including the http-status and a separated
	  http body if the program successfully terminated or none if an exception occurred
	  and especially if it is a custom message as the http problem details.
	  (HTTP Status)[https://de.wikipedia.org/wiki/HTTP-Statuscode]
	  (HTTP Promblem Details)[https://www.rfc-editor.org/rfc/rfc9457.html#name-members-of-a-problem-detail]
	Notice that this http like calls are build of a path with optional leading **/**,
	  and query arguments after **?** separated by **&** in name and value by **=**.
	  Make sure to replace ** ** in the *input* argument with **%20** to be URI like.
	  (Special signs " !#$%&'()*+,/:;=?@[]")
	  (URI RFC)[https://www.ietf.org/rfc/rfc3986.txt]

### Interface

	Version v1:
		o solve GET /versions    Get possible JWAoCProgramInterface versions.
		o solve GET /v1          Get specific help for the solution program.
		o solve GET /v1/author     Get the author of the solution program.
		o solve GET /v1/version    Get the version of the solution program.
		o solve GET /v1/{year}/{task_day}/{sub_task}?input={input_file_path}
			Parameters can be any name value pair especially:
				input    required  Path to the input file.
				debug    optional  If the content should be debug information.
				check    optional  If the request should be validated as much as possible
				                     by return any ProblemDetails without solving the task.
				args     optional  Collection of any unnamed arguments.

	Examples:
		Request:  ```http GET /versions```
		Response:
			```
			http/1.1 200 OK
			
			[
			  "v1"
			]
			```

		Request:  ```http GET /v1/2024/1/a?src=my%20file%20&fast=true```
		Response:
			```
			http/1.1 200 OK
			
			1024
			```

		Request:  ```anything arg0 arg1 arg2```
		Response: No valid endpoint expected - any or no response allowed

		Request:  ```http```
		Response: ```http/1.1 400 Bad Request```

		Request:  ```http GET```
		Response: ```http/1.1 400 Bad Request```