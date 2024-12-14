import sys

def main(*args):
	if len(args) > 0 and args[0] == "http":
		if len(args) == 3 and args[1] == "GET":
			consoleResponseToLocalHTTPGetRequest(args[2])
		else:
			print("HTTP/1.1 400 Bad Request")

def consoleResponseToLocalHTTPGetRequest(currentURIString):
	print("HTTP/1.1 501 Not Implemented")

if __name__ == "__main__":
	main(*sys.argv[1:])