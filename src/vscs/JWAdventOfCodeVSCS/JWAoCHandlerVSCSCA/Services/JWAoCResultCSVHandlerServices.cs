﻿using JWAdventOfCodeHandlerLibrary;
using JWAdventOfCodeHandlerLibrary.Data;
using JWAdventOfCodeHandlerLibrary.Services;
using JWAdventOfCodeHandlerLibrary.Settings;

namespace JWAoCHandlerVSCSCA.Services;

public class JWAoCResultCSVHandlerServices : IJWAoCResultHandlerService
{
    // methods
    public void HandleResult(JWAoCResult result, IJWAoCSettings settings, IJWAoCCA consoleApplication)
    {
        if (!string.IsNullOrEmpty(settings.ResultTargetPath) && result.Response.StatusCode == 200)
        {
            if (!File.Exists(settings.ResultTargetPath) || File.ReadAllText(settings.ResultTargetPath).Trim().Length == 0)
            {
                File.WriteAllText(settings.ResultTargetPath, string.Join(';', new string[] { "Timestamp", "Task", "Result", "Duration", "Program", "Path", "Request", "Response" }));
            }

            consoleApplication.PrintPrefixOut();
            consoleApplication.Print($"  store result... ");
            try
            {
                File.AppendAllText(settings.ResultTargetPath, Environment.NewLine + string.Join(';', new string[] {
                    result.Timestamp.ToString("yyyy.MM.dd HH:mm:ss:fff"),
                    $"{result.TaskYear}-{result.TaskDay}{result.SubTask}",
                    result.Response.StatusCode == 200 ? result.Response.Content.ToString() : "null",
                    result.Duration.ToString(),
                    result.ProgramName,
                    result.Program.ProgramFilePath,
                    string.Join(" ", result.ProgramArgs),
                    result.Response.ToString(true),
                }));
                consoleApplication.Print($"was successful!{Environment.NewLine}");
            }
            catch
            {
                consoleApplication.Print($"failed!{Environment.NewLine}");
            }
        }
    }
}