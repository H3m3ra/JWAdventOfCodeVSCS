﻿using JWAdventOfCodeHandlerLibrary.Command;
using JWAdventOfCodeHandlerLibrary.Services;
using JWAoCHandlerVSCSCA.Command.Commands.StringCommands;
using System.Text.RegularExpressions;

namespace JWAoCHandlerVSCSCA.Services.CommandHandlers;

public class JWAoCCurrentCommandHandler : IJWAoCCommandHandlerService
{
    public JWAoCHandlerVSCS Handler { get; set; }

    // methods
    public bool HandleCommand(IJWAoCCommand command)
    {
        if (command == null) return true;

        if (command is JWAoCCallCommand)
        {
            Handler.CurrentYear = 2024;
            Handler.CurrentDay = 4;
            Handler.CurrentSub = "a";
            return ((JWAoCCallCommand)command).Execute(Handler.Settings, Handler);
        }
        else if (command is JWAoCGetCommand)
        {
            var currentCommand = (JWAoCGetCommand)command;
            if (Handler.LoadSettrings("  Cannot ", true))
            {
                Handler.PrintLinesOut(currentCommand.GetValues(Handler.Settings, Handler.ProgramExecutionService).ToArray());
            }
        }
        else if (command is JWAoCSetCommand)
        {
            var currentCommand = (JWAoCSetCommand)command;
            if(Handler.LoadSettrings("  Cannot ", true))
            {
                if(Handler.StoreSettings("  Cannot ", true))
                {
                    currentCommand.SetValues(Handler.Settings);
                }
            }
        }
        else if (command is JWAoCSimpleStringCommand)
        {
            var currentCommand = (JWAoCSimpleStringCommand)command;

            if (currentCommand.Name.StartsWith("?") || currentCommand.Name.StartsWith("h"))
            {
                Handler.Help();
            }
            else if (currentCommand.Name.StartsWith("ch") || currentCommand.Name.StartsWith("y"))
            {
                var parts = currentCommand.Args;
                if (parts.Length == 0)
                {
                    if (Handler.CurrentYear != null)
                    {
                        if (Handler.CurrentDay != null)
                        {
                            if (Handler.CurrentSub != null)
                            {
                                Handler.CurrentSub = null;
                            }
                            else
                            {
                                Handler.CurrentDay = null;
                            }
                        }
                        else
                        {
                            Handler.CurrentYear = null;
                        }
                    }
                }
                else
                {
                    try
                    {
                        var value = int.Parse(parts[0]);
                        if (Handler.CurrentYear != null && (value >= 1 && value <= 31))
                        {
                            Handler.CurrentDay = value;
                        }
                        else
                        {
                            Handler.CurrentYear = value;
                            if (Handler.CurrentYear < 100)
                            {
                                var currentFullYear = DateTime.Now.Year;
                                var currentShortYear = currentFullYear % 100;
                                currentFullYear -= currentShortYear;
                                if (Handler.CurrentYear > currentShortYear)
                                {
                                    currentFullYear -= 100;
                                }
                                Handler.CurrentYear += currentFullYear;
                            }
                        }
                    }
                    catch
                    {
                        if (Handler.CurrentYear != null)
                        {
                            var value = parts[0].Trim();
                            string currentSub = "";
                            if (value.Length >= 2 && Char.IsDigit(value[1]))
                            {
                                Handler.CurrentDay = int.Parse(value.Substring(0, 2));
                                currentSub = value.Substring(2).Trim();
                            }
                            else if (Char.IsDigit(value[0]))
                            {
                                Handler.CurrentDay = int.Parse(value.Substring(0, 1));
                                currentSub = value.Substring(1).Trim();
                            }
                            else
                            {
                                currentSub = value;
                            }

                            if (Handler.CurrentDay != null)
                            {
                                Handler.CurrentSub = (currentSub.Length == 0 ? null : currentSub);
                            }
                        }
                    }
                }
            }
            else if (currentCommand.Name.StartsWith("cr"))
            {
                var taskFilePath = Handler.GetSourceFilePaths(new string[] { Handler.Settings.TasksSourcePath }, new Regex("task", RegexOptions.IgnoreCase)).FirstOrDefault();
                if (string.IsNullOrEmpty(taskFilePath))
                {
                    taskFilePath = $"{Handler.Settings.TasksSourcePath}{Path.DirectorySeparatorChar}{Handler.CurrentYear}{Path.DirectorySeparatorChar}{(Handler.CurrentDay < 10 ? "0" : "")}{Handler.CurrentDay}{Handler.CurrentSub}_task.txt";
                }

                IList<string> lines = new List<string>();
                string line = null;
                while ((line = Console.ReadLine()).Length > 0 || lines.Count < 1 || lines.Last().Length > 0)
                {
                    lines.Add(line);
                }
                lines.RemoveAt(lines.Count - 1);
                File.WriteAllText(taskFilePath, string.Join(Environment.NewLine, lines));
            }
            else if (currentCommand.Name.StartsWith("sh"))
            {
                Handler.PrintLinesOut(Handler.GetSourceFilePaths(new string[] { Handler.Settings.TasksSourcePath }, JWAoCHandlerVSCS.TASK_REGEX).ToArray());
                Handler.PrintLinesOut(Handler.GetSourceFilePaths(new string[] { Handler.Settings.InputsSourcePath }, JWAoCHandlerVSCS.INPUT_REGEX).ToArray());
                Handler.PrintLinesOut(Handler.GetSourceFilePaths(new string[] { Handler.Settings.TestsSourcePath }, JWAoCHandlerVSCS.TEST_REGEX).ToArray());
            }
        }
        return true;
    }

    // get-methods
    public bool CanHandle(IJWAoCCommand command)
    {
        if (
            command == null ||
            command is JWAoCCallCommand ||
            command is JWAoCGetCommand ||
            command is JWAoCSetCommand
        )
        {
            return true;
        }
        if(command is JWAoCSimpleStringCommand)
        {
            var currentCommand = (JWAoCSimpleStringCommand)command;

            return (
                (currentCommand.Name.StartsWith("?") || currentCommand.Name.StartsWith("h")) ||
                (currentCommand.Name.StartsWith("ch") || currentCommand.Name.StartsWith("y")) ||
                currentCommand.Name.StartsWith("cr") ||
                currentCommand.Name.StartsWith("sh")
            );
        }
        return false;
    }
}