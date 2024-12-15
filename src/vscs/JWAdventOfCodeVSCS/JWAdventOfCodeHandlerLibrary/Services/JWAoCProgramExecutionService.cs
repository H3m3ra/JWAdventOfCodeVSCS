using JWAdventOfCodeHandlerLibrary.Settings.Program;
using JWAdventOfCodeHandlingLibrary.HTTP;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace JWAdventOfCodeHandlerLibrary.Services;

public class JWAoCProgramExecutionService : IJWAoCProgramExecutionService
{
    // methods
    public IJWAoCHTTPResponse CallProgramWithLocalHTTPGet(JWAoCProgram program, string currentHTTPURIString)
    {
        return CallProgramWithLocalHTTP(program, "http", "GET", currentHTTPURIString);
    }

    public IJWAoCHTTPResponse CallProgramWithLocalHTTP(JWAoCProgram program, params string[] args)
    {
        if (!program.IsAvailable())
        {
            return new JWAoCHTTPErrorResponse(new JWAoCHTTPProblemDetails("Program is not available!", 503));
        }

        if (program.ProgramType == JWAoCProgramType.EXE)
        {
            var startInfo = new ProcessStartInfo();
            startInfo.Arguments = string.Join(' ', args);
            startInfo.CreateNoWindow = false;
            startInfo.FileName = program.ProgramFilePath;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            try
            {
                IList<string> currentHTTPHeaders = new List<string>();
                string currentHTTPBodyText = null;

                using (Process process = Process.Start(startInfo))
                {
                    using (var sr = process.StandardOutput)
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (string.IsNullOrEmpty(line))
                            {
                                currentHTTPBodyText = "";
                            }
                            else if (currentHTTPBodyText == null)
                            {
                                currentHTTPHeaders.Add(line);
                            }
                            else
                            {
                                currentHTTPBodyText += line;
                            }
                        }
                    }
                    process.WaitForExit();
                }

                return new JWAoCHTTPResponse()
                    {
                        Version = new Regex(@"HTTP/\d+\.\d+").Match(currentHTTPHeaders.First()).Value.Substring(5),
                        StatusCode = int.Parse(new Regex("\\d\\d\\d").Match(currentHTTPHeaders.First()).Value),
                        Headers = new Dictionary<string, string> (
                                currentHTTPHeaders
                                .Skip(1)
                                .Select(l => { var ps = l.Split(": "); return KeyValuePair.Create(ps[0], ps[1]); })
                                .ToList()
                            ),
                        Content = string.IsNullOrEmpty(currentHTTPBodyText) ? null : currentHTTPBodyText
                    };
            }
            catch (Exception ex)
            {
                return new JWAoCHTTPErrorResponse(new JWAoCHTTPProblemDetails(ex.Message, 422));
            }

        }
        else if (program.ProgramType == JWAoCProgramType.RAW)
        {
            return new JWAoCHTTPErrorResponse(new JWAoCHTTPProblemDetails("Program type not supported!", 501));
        }
        else
        {
            return new JWAoCHTTPErrorResponse(new JWAoCHTTPProblemDetails("Program type not supported!", 400));
        }
    }
}