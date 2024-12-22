import json
import re
import sys

class JWAoCHTTPResponseBase(object):
	STATUS_CODE_NAMES = {
		200: "OK",
		400: "Bad Request",
		404: "Not Found",
		422: "Unprocessable Entity",
		500: "Internal Server Error",
		501: "Not Implemented",
		503: "Service Unavailable"
	};
	BAD_REQUEST = None

	def __init__(self):
		self.content = None
		self.version = "1.1"
		self.status_code = 500
		self.headers = {}

	# static-get-methods
	@staticmethod
	def getStatusNameOf(statusCode):
		return JWAoCHTTPResponseBase.STATUS_CODE_NAMES[statusCode] if statusCode in JWAoCHTTPResponseBase.STATUS_CODE_NAMES else "Unknown"

	# to-methods
	def toString(self, inline = False):
		builder = "HTTP/{} {} {}".format(self.version, self.status_code, self.status_name)
		for headerKey, headerValue in self.headers.items():
			builder += '\n'
			builder += headerKey
			builder += ": "
			builder += headerVal
		if self.content is not None:
			builder += '\n'
			builder += '\n'
			if isinstance(self.content, JWAoCHTTPProblemDetails):
				builder += json.dumps(self.content, cls=JWAoCHTTPProblemDetailsEncoder) if inline else json.dumps(self.content, indent = "  ", cls=JWAoCHTTPProblemDetailsEncoder)
			else:
				builder += json.dumps(self.content) if inline else json.dumps(self.content, indent = "  ")
		return builder.replace('\n', "\\n") if inline else builder

	# getter
	@property
	def status_name(self):
		return JWAoCHTTPResponseBase.getStatusNameOf(self.status_code)

class JWAoCHTTPResponse(JWAoCHTTPResponseBase):
	def __init__(self, status_code = 400, content = None):
		super().__init__()
		self.status_code = status_code
		self.content = content

JWAoCHTTPResponseBase.BAD_REQUEST = JWAoCHTTPResponse()

class JWAoCHTTPProblemDetails(object):
	def __init__(self, message, http_status):
		self.type = None
		self.title = None
		self.status = http_status
		self.detail = message
		self.instance = None

class JWAoCHTTPProblemDetailsEncoder(json.JSONEncoder):
		def default(self, o):
			return o.__dict__

class JWAoCHTTPErrorResponse(JWAoCHTTPResponseBase):
	def __init__(self, problemDetails):
		super().__init__()
		self._status_code = problemDetails.status
		self.content = problemDetails

	# getter
	@property
	def status_code(self):
		return self._status_code

	# setter
	@status_code.setter
	def status_code(self, value):
		self._status_code = value
		if self.content is not None:
			self.content.status = value

class JWAoCHTTPService:
	SPECIAL_URI_SIGNS = " !#$%&'()*+,/:;=?@[]"
	SPECIAL_URI_CODES = list(map(lambda s: "%{:0>2X}".format(ord(s)), SPECIAL_URI_SIGNS))
	HTTP_SPECIAL_SIGN = '%'
	HTTP_PATH_SEPARATOR = '/'
	HTTP_PARAMETERS_SEPARATOR = '?'
	HTTP_PARAMETER_SEPARATOR = '&'
	HTTP_PARAMETER_VALUE_SEPARATOR = '='

	# static-to-methods
	@staticmethod
	def toURIStringFromString(source):
		if source is None or len(source) == 0:
			return source

		result = ""
		for currentChar in source:
			if currentChar in SPECIAL_URI_SIGNS:
				result += currentChar
			else:
				result += JWAoCHTTPService.SPECIAL_URI_CODES[JWAoCHTTPService.SPECIAL_URI_SIGNS.index(currentChar)]
		return source

	@staticmethod
	def toStringFromURIString(source):
		if source is None or len(source) == 0:
			return source

		result = ""
		for c in range(0, len(source)):
			if source[c] == JWAoCHTTPService.HTTP_SPECIAL_SIGN and c+2 < len(source) and source[c:c+3] in JWAoCHTTPService.SPECIAL_URI_CODES:
				result += JWAoCHTTPService.SPECIAL_URI_SIGNS[JWAoCHTTPService.SPECIAL_URI_SIGNS.index(source[c:c+3])]
			else:
				result += source[c]
		return source

	# static-get-methods
	@staticmethod
	def getRouteFromLocalURIString(source):
		source = source[:source.index(JWAoCHTTPService.HTTP_PARAMETERS_SEPARATOR)] if JWAoCHTTPService.HTTP_PARAMETERS_SEPARATOR in source else source
		source = source[1:] if source.startswith(JWAoCHTTPService.HTTP_PATH_SEPARATOR) else source
		return list(map(lambda p: JWAoCHTTPService.toStringFromURIString(p), source.split(JWAoCHTTPService.HTTP_PATH_SEPARATOR)));

	@staticmethod
	def getParametersFromLocalURIString(source):
		parameters = {}

		source = source[source.index(JWAoCHTTPService.HTTP_PARAMETERS_SEPARATOR)+1:] if JWAoCHTTPService.HTTP_PARAMETERS_SEPARATOR in source else None
		if source is None or len(source) == 0:
			return parameters

		for parameterSource in source.split(JWAoCHTTPService.HTTP_PARAMETER_SEPARATOR):
			paramParts = parameterSource.split(JWAoCHTTPService.HTTP_PARAMETER_VALUE_SEPARATOR)
			parameters[JWAoCHTTPService.toStringFromURIString(paramParts[0])] = JWAoCHTTPService.toStringFromURIString(paramParts[1])

		return parameters



def solve_2024_01a(filePath):
	with open(filePath, 'r') as file:
		content = file.read()
		ns = list(map(lambda l: int(l.split("   ")[0]), content.splitlines()))
		ms = list(map(lambda l: int(l.split("   ")[1]), content.splitlines()))
	sum = 0
	for i in range(0, len(ns)):
		sum += abs(ns[i] - ms[i])
	return sum

def solve_2024_15a(filePath):
	with open(filePath, 'r') as file:
		lines = file.read().splitlines()
		field = []
		for i in range(0, len(lines)-2):
			field.append(lines[i].split(""))
		operations = lines[-1]

	x = 0
	y = 0
	for i in range(0, len(field)):
		for j in range(0, len(field[0])):
			if field[i][j] == '@':
				y = i
				x = j

	def move(a, b):
		nonlocal field
		nonlocal x
		nonlocal y
		if field[y+a][x+b] == '.':
			field[y+a][x+b] = '@'
			field[y][x] = '.'
			x += b
			y += a
		elif field[y+a][x+b] == '0':
			while field[y+a][x+b] == '0':
				a += a
				b += b
			if field[y+a][x+b] != '#':
				while field[y+a][x+b] != '@':
					field[y+a][x+b] = '0'
					a -= a
					b -= b
				field[y+a][x+b] = '@'
				field[y][x] = '.'
				x += b
				y += a

	for o in operations:
		

	sum = 0
	for i in range(0, len(field)):
		for j in range(0, len(field[0])):
			if field[i][j] == 'O':
				sum += 100*i+j
	return sum

def solve_2024_19a(filePath):
	with open(filePath, 'r') as file:
		lines = file.read().splitlines()
		towelPatterns = lines[0].split(", ")
		towels = []
		for i in range(2, len(lines)):
			towels.append(lines[i])

	def isTowel(towel):
		nonlocal towelPatterns

		next_towels = set()
		for p in towelPatterns:
			if towel == p:
				return True
			elif towel.startswith(p):
				next_towels.add(towel[len(p):])

		for n in list(next_towels):
			if isTowel(n):
				return True
		return False

	sum = 0
	for towel in towels:
		if isTowel(towel):
			sum += 1
	return sum
def solve_2024_19b(filePath):
	with open(filePath, 'r') as file:
		lines = file.read().splitlines()
		towelPatterns = lines[0].split(", ")
		towels = []
		for i in range(2, len(lines)):
			towels.append(lines[i])

	def isTowel(towel):
		nonlocal towelPatterns

		next_towels = set()
		for p in towelPatterns:
			if towel == p:
				return True
			elif towel.startswith(p):
				next_towels.add(towel[len(p):])

		for n in list(next_towels):
			if isTowel(n):
				return True
		return False

	sum = 0
	for towel in towels:
		if isTowel(towel):
			sum += 1
	return sum



class JWAoCProgramCABase(object):
	# methods
	def consoleResponseToLocalHTTPGetRequestFromURIString(self, requestURIString):
		route = JWAoCHTTPService.getRouteFromLocalURIString(requestURIString)
		parameters = JWAoCHTTPService.getParametersFromLocalURIString(requestURIString)

		self.consoleResponseToLocalHTTPGetRequest(route, parameters)

	def consoleResponseToLocalHTTPGetRequest(self, route, parameters):
		REGEX_NUMBER = "^\\d+$"

		if len(route) == 1 and route[0] == "versions":
			self.show(JWAoCHTTPResponse(200, self.http_api_versions))
		elif len(route) == 1 and route[0] in self.http_api_versions:
			self.show(JWAoCHTTPResponse(200, self.program_helps[self.http_api_versions.index(route[0])]))
		elif len(route) == 2 and route[0] in self.http_api_versions and route[1] == "author":
			self.show(JWAoCHTTPResponse(200, self.program_authors[self.http_api_versions.index(route[0])]))
		elif len(route) == 2 and route[0] in self.http_api_versions and route[1] == "version":
			self.show(JWAoCHTTPResponse(200, self.program_versions[self.http_api_versions.index(route[0])]))
		elif len(route) == 4 and route[0] in self.http_api_versions and re.match(REGEX_NUMBER, route[1]) is not None and re.match(REGEX_NUMBER, route[2]) is not None and "input" in parameters:
			self.consoleResponseToLocalHTTPSolveRequest(route[0], int(route[1]), int(route[2]), route[3], parameters)
		else:
			self.show(JWAoCHTTPResponse.BAD_REQUEST)

	def consoleResponseToLocalHTTPSolveRequest(self, version, taskYear, taskDay, subTask, parameters):
		raise NotImplementedError("JWAoCProgramCABase._consoleResponseToLocalHTTPSolveRequest(self, version, taskYear, taskDay, subTask, parameters) is not implemented!")

	# show-methods
	def show(self, response):
		print(response.toString())

class JWAoC2024VSCS(JWAoCProgramCABase):
	# static-main-method
	@staticmethod
	def main(*args):
		if len(args) > 0 and args[0] == "http":
			program = JWAoC2024VSCS()
			if len(args) == 3 and args[0] == "http" and args[1] == "GET":
				program.consoleResponseToLocalHTTPGetRequestFromURIString(args[2])
			else:
				program.show(JWAoCHTTPResponseBase.BAD_REQUEST)

	# methods
	def consoleResponseToLocalHTTPSolveRequest(self, version, taskYear, taskDay, subTask, parameters):
		if (version == "v1"):
			self.consoleResponseToLocalHTTPSolveRequestV1(taskYear, taskDay, subTask, parameters)
			return None

		self.show(JWAoCHTTPErrorResponse(JWAoCHTTPProblemDetails("Version not found!", 404)))

	def consoleResponseToLocalHTTPSolveRequestV1(self, taskYear, taskDay, subTask, parameters):
		if taskDay < 1 or taskDay > 25:
			self.show(JWAoCHTTPErrorResponse(JWAoCHTTPProblemDetails("Day not found!", 404)))
		elif taskYear == 2024:
			input = parameters["input"];
			try:
				if subTask == "a":
					if taskDay == 1:
						self.show(JWAoCHTTPResponse(200, solve_2024_01a(input)))
					elif taskDay == 15:
						self.show(JWAoCHTTPResponse(200, solve_2024_15a(input)))
					elif taskDay == 19:
						self.show(JWAoCHTTPResponse(200, solve_2024_19a(input)))
					else:
						self.show(JWAoCHTTPResponse(501))
				elif subTask == "b":
					if taskDay == 0:
						pass
					else:
						self.show(JWAoCHTTPResponse(501))
				else:
					self.show(JWAoCHTTPErrorResponse(JWAoCHTTPProblemDetails("Sub task not found!", 404)))
			except Exception as ex:
				self.show(JWAoCHTTPErrorResponse(JWAoCHTTPProblemDetails(str(ex), 500)))
		else:
			self.show(JWAoCHTTPErrorResponse(JWAoCHTTPProblemDetails("Year not found!", 404)))

	# getter
	@property
	def http_api_versions(self):
		return ["v1"]

	@property
	def program_helps(self):
		return ["Specific Help not implemented!"]

	@property
	def program_authors(self):
		return ["JWHemera"]

	@property
	def program_versions(self):
		return ["1.0.0.20241218184700"]



if __name__ == "__main__":
	JWAoC2024VSCS.main(*sys.argv[1:])