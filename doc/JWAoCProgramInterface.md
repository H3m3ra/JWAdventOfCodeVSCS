# JWAoCProgramInterface (2025.01.07)
	The API to make any AoC **program** callable by an AoC **handler**
	  separating organized execution and implemented solving of AoC tasks.

## Local http-like call
	The **handler** acts like a client sending requests to different solution services
	  called **program** that respond with writing to the standard output.
	The start with leading argument "http" should start this process and terminate
	  with any http responding message including the http-status and a separated
	  http body if the program successfully terminated or none if an exception occurred
	  and especially if it is a custom message as the http problem details.
	  (HTTP Status)[https://de.wikipedia.org/wiki/HTTP-Statuscode]
	  (HTTP Promblem Details)[https://www.rfc-editor.org/rfc/rfc9457.html#name-members-of-a-problem-detail]
	Notice that this http like calls are build of a path with optional leading **/**,
	  and query arguments after **?** separated by **&** in name and value by **=**.
	  Make sure to replace ** ** in the **input** argument with **%20** to be URI like.
	  (Special signs " !#$%&'()*+,/:;=?@[]")
	  (URI RFC)[https://www.ietf.org/rfc/rfc3986.txt]

### Interface
	Available API endpoits per version.

	General:
		o http GET /versions      Get possible JWAoCProgramInterface versions.
		                          Should be requested from handlers first to change or check the available
		                            version(s) for **handler** requests and **program** responses.
		                            (Must obviously be equals on both sides to request/respond perfectly)

	Version v1:
		o http GET /v1            Get specific human readable help for the solution program.
		o http GET /v1/author     Get the author of the solution program.
		                            This metadata could be requested when storing results.
		o http GET /v1/version    Get the version of the solution program.
		                            This metadata could be requested when storing results.
		o http GET /v1/{year}/{task_day}/{sub_task}?input={input_file_path}
			Parameters can be any name value pair especially:
				input  required    Path to the puzzle input file.
				debug  optional    If the response content could be any customized debug information
				                     during execution that allows debugging or inspection.
				                   INFO: The result is not required so the response
				                     should be presented only by a **handler**.
				check  optional    If the request should be validated as much as possible
				                     by return any ProblemDetails without solving the task.
				                   WARNING: If a **handler** use this lazily checking if a request should be
				                     requested again and your implementation does NOT respond especially
				                     it will produce duplicate requests may consuming much more time.
				                     (Metrics should be measured once per request with
				                       debug=false&check=false anyway and not evaluated on check requests)
				                     The laziest valid **program** response is ```http/1.1 200 OK``` causing
				                     a **handler** to may be surprised by a ```http/1.1 404 NOT FOUND```
				                     to a validation on an API endpoint but should NOT cause errors on the
				                     **handler** cause even laziest handler must accept ProblemDetails
				                     as responses of any request to this API.
				args   optional    Collection of any unnamed arguments for example customizing execution.
				?      ?           Any other parameter pairs for example customizing execution.

	Examples:
		Request:  ```http GET /versions```
		Response:
			```
			http/1.1 200 OK
			
			[
			  "v1"
			]
			```

		Request:  ```http GET /v1/2024/1/a?input=my%20file%20&fast=true```
		Response:
			```
			http/1.1 200 OK
			
			1024
			```

		Request:  ```http GET /v1/2024/31/b?input=myInput_24-31-b.txt&check=true```
		Response:
			```
			http/1.1 404 Not Found
			
			{
			  "type": null,
			  "title": "Invalid request!",
			  "status": 404,
			  "detail": "Day "31" not allowed!",
			  "instance": null
			}
			```

		Request:  ```anything arg0 arg1 arg2```
		Response: No valid endpoint expected - any or no response allowed.