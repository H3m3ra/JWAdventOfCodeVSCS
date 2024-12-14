public class JWAoC2024JavaCA{
	// main-method
	public static void main(String... args){
		if(args.length > 0 && args[0].equals("http")){
			if(args.length == 3 && args[1].equals("GET")){
				consoleResponseToLocalHTTPGetRequest(args[2]);
			}
			else{
				System.out.println("HTTP/1.1 400 Bad Request");
			}
		}
	}

	// static-methods
	public static void consoleResponseToLocalHTTPGetRequest(String currentURIString){
		System.out.println("HTTP/1.1 501 Not Implemented");
	}
}