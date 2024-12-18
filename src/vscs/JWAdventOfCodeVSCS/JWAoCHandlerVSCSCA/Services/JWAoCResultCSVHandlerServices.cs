using JWAdventOfCodeHandlerLibrary.Data;
using JWAdventOfCodeHandlerLibrary.Services;
using JWAdventOfCodeHandlerLibrary.Settings;

namespace JWAoCHandlerVSCSCA.Services;

public class JWAoCResultCSVHandlerServices : IJWAoCResultHandlerService
{
    // methods
    public void HandleResult(JWAoCResult result, IJWAoCSettings settings, IJWAoCIOConsoleService currcentIOConsoleService)
    {
        if (!string.IsNullOrEmpty(settings.ResultsTargetPathPattern) && result.Response.StatusCode == 200)
        {
            var resultTargetPath = settings.GetResultTargetPath(
                result.TaskYear,
                result.TaskDay,
                result.SubTask,
                result.ProgramName,
                result.ProgramVersion,
                result.ProgramAuthor
            );
            try
            {
                if (!File.Exists(resultTargetPath) || File.ReadAllText(resultTargetPath).Trim().Length == 0)
                {
                    File.WriteAllText(resultTargetPath, string.Join(';', new string[] { "Timestamp", "Task", "Result", "Duration", "Program", "Path", "Request", "Response" }));
                }

                currcentIOConsoleService.PrintPrefixOut();
                currcentIOConsoleService.Print($"  store result... ");
                try
                {
                    File.AppendAllText(resultTargetPath, Environment.NewLine + string.Join(';', new string[] {
                    result.Timestamp.ToString("yyyy.MM.dd HH:mm:ss:fff"),
                    $"{result.TaskYear}-{result.TaskDay}{result.SubTask}",
                    result.Response.StatusCode == 200 ? result.Response.Content.ToString() : "null",
                    result.Duration.ToString(),
                    result.ProgramName,
                    result.Program.ProgramFilePath,
                    string.Join(" ", result.ProgramArgs),
                    result.Response.ToString(true),
                }));
                    currcentIOConsoleService.Print($"was successful!{Environment.NewLine}");
                }
                catch
                {
                    currcentIOConsoleService.Print($"failed!{Environment.NewLine}");
                }
            }
            catch(Exception ex)
            {
                currcentIOConsoleService.PrintLineOut($"  ERROR {ex.Message}");
            }
        }
    }
}