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
        if (!program.IsAvailable()) return new JWAoCHTTPErrorResponse(new JWAoCHTTPProblemDetails("Program is not available!", 503));

        if (program.ProgramType == JWAoCProgramType.EXE) return CallProgramWithLocalHTTP(program.TimeOut, program.ProgramFilePath, args);
        else if (program.ProgramType == JWAoCProgramType.RAW) return CallRawProgramWithLocalHTTP(program.TimeOut, program, args);

        return new JWAoCHTTPErrorResponse(new JWAoCHTTPProblemDetails("Program type not supported!", 400));
    }

    protected IJWAoCHTTPResponse CallRawProgramWithLocalHTTP(TimeSpan timeoutSpan, JWAoCProgram program, params string[] args)
    {
        if (program.ProgramFilePath.ToLower().EndsWith(".exe"))
        {
            return CallProgramWithLocalHTTP(timeoutSpan, program.ProgramFilePath, args);
        }
        else if (program.ProgramHandler != null)
        {
            if (program.ProgramHandler.BuilderFilePath != null)
            {

            }

            if (program.ProgramHandler.InterpreterFilePath != null)
            {
                return CallProgramWithLocalHTTP(
                    timeoutSpan,
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

    protected IJWAoCHTTPResponse CallProgramWithLocalHTTP(TimeSpan timeoutSpan, string programFilePath, params string[] args)
    {
        var process = CreateProcess(programFilePath, args);
        if (process == null) return new JWAoCHTTPErrorResponse(new JWAoCHTTPProblemDetails("Process creation failed!", 404));

        var respondedLines = new List<string>();

        var taskCompletionSource = new TaskCompletionSource();
        var task = taskCompletionSource.Task;
        Task.Factory.StartNew(() =>
        {
            using (process)
            {
                using (var sr = process.StandardOutput)
                {
                    string? line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        respondedLines.Add(line);
                    }
                }
                process.WaitForExit();
            }
            if(!task.IsCanceled) taskCompletionSource.SetResult();
        });

        var timeout = false;
        var timeoutEndDate = DateTime.Now + timeoutSpan;
        while (!task.IsCompleted && !timeout)
        {
            if (DateTime.Now >= timeoutEndDate) timeout = true;
        }

        if (timeout)
        {
            taskCompletionSource.SetCanceled();
            return new JWAoCHTTPResponse(408, string.Join('\n', respondedLines));
        }

        return GetResponseOfRespondedLines(respondedLines);
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

    protected IJWAoCHTTPResponse GetResponseOfRespondedLines(IList<string> respondedLines)
    {
        try
        {
            var currentHTTPHeaders = new List<string>();
            string? currentHTTPBodyText = null;
            foreach (var line in respondedLines)
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
}