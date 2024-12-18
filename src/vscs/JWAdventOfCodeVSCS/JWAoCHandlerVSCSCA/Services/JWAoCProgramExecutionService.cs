using JWAdventOfCodeHandlerLibrary.Services;
using JWAdventOfCodeHandlerLibrary.Settings.Program;
using JWAdventOfCodeHandlingLibrary.HTTP;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace JWAoCHandlerVSCSCA.Services;

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
            return CallProgramWithLocalHTTP(program.ProgramFilePath, args);
        }
        else if (program.ProgramType == JWAoCProgramType.RAW)
        {
            return CallRawProgramWithLocalHTTP(program, args);
        }
        else
        {
            return new JWAoCHTTPErrorResponse(new JWAoCHTTPProblemDetails("Program type not supported!", 400));
        }
    }

    protected IJWAoCHTTPResponse CallRawProgramWithLocalHTTP(JWAoCProgram program, params string[] args)
    {
        if (program.ProgramFilePath.ToLower().EndsWith(".exe"))
        {
            return CallProgramWithLocalHTTP(program.ProgramFilePath, args);
        }
        else if (program.ProgramHandler != null)
        {
            if (program.ProgramHandler.BuilderFilePath != null)
            {

            }

            if (program.ProgramHandler.InterpreterFilePath != null)
            {
                return CallProgramWithLocalHTTP(
                    program.ProgramHandler.InterpreterFilePath,
                    Enumerable.Concat(new List<string>() { program.ProgramFilePath }, args).ToArray()
                );
            }

            if (program.ProgramHandler.CompilerFilePath != null)
            {

            }
        }

        return new JWAoCHTTPErrorResponse(new JWAoCHTTPProblemDetails("Program configuration not supported!", 400));
    }

    protected IJWAoCHTTPResponse CallProgramWithLocalHTTP(string programFilePath, params string[] args)
    {
        IList<string> currentHTTPHeaders = new List<string>();
        string? currentHTTPBodyText = null;
        try
        {
            using (var process = CreateProcess(programFilePath, args))
            {
                using (var sr = process?.StandardOutput)
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
                process?.WaitForExit();
            }

            return new JWAoCHTTPResponse()
            {
                Version = new Regex(@"HTTP/\d+\.\d+").Match(currentHTTPHeaders.First()).Value.Substring(5),
                StatusCode = int.Parse(new Regex("\\d\\d\\d").Match(currentHTTPHeaders.First()).Value),
                Headers = new Dictionary<string, string>(
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

    protected Process? CreateProcess(string programFilePath, params string[] args)
    {
        var startInfo = new ProcessStartInfo();
        startInfo.Arguments = string.Join(' ', args);
        startInfo.CreateNoWindow = false;
        startInfo.FileName = programFilePath;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.UseShellExecute = false;
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        return Process.Start(startInfo);
    }
}