import System.Environment
import System.IO

main :: IO()
main = do
  args <- getArgs;
  mainHelper(args);

mainHelper :: [String] -> IO()
mainHelper ("http":"GET":u:[]) = do consoleResponseToLocalHTTPGetRequest(u);
mainHelper ("http":xs) = do putStrLn $ "HTTP/1.1 400 Bad Request";
mainHelper xs = do putStr $ "";

consoleResponseToLocalHTTPGetRequest :: str -> IO()
consoleResponseToLocalHTTPGetRequest curretURIString = do
  putStrLn $ "HTTP/1.1 501 Not Implemented"