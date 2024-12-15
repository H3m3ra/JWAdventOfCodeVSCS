using JWAdventOfCodeHandlerLibrary.Services;

namespace JWAoCHandlerVSCSCA.Services;

public class JWAoCIOService : IJWAoCIOService
{
    // get-methods
    public ISet<string> GetSourceFilePaths(Func<string, bool> allowFilePath, params string[] sourcePaths)
    {
        ISet<string> sourceFilePaths = new HashSet<string>();
        foreach (string sourcePath in sourcePaths)
        {
            GetSourceFilePaths(Path.GetFullPath(sourcePath), allowFilePath, sourceFilePaths);
        }
        return sourceFilePaths;
    }

    protected ISet<string> GetSourceFilePaths(string sourcePath, Func<string, bool> allowFilePath, ISet<string> sourceFilePaths)
    {
        if (!File.Exists(sourcePath))
        {
            if (sourcePath.Contains('.'))
            {
                File.Create(sourcePath);
            }
            else
            {
                Directory.CreateDirectory(sourcePath);
            }
        }

        if ((File.GetAttributes(sourcePath) & FileAttributes.Directory) == FileAttributes.Directory)
        {
            try
            {
                foreach (var filePath in Directory.GetFiles(sourcePath))
                {
                    if (allowFilePath(filePath))
                    {
                        sourceFilePaths.Add(filePath);
                    }
                }
                foreach (var directoryPath in Directory.GetDirectories(sourcePath))
                {
                    foreach (string filePath in GetSourceFilePaths(directoryPath, allowFilePath, sourceFilePaths))
                    {
                        sourceFilePaths.Add(filePath);
                    }
                }
            }
            catch
            {

            }
        }
        else if (allowFilePath(sourcePath))
        {
            sourceFilePaths.Add(sourcePath);
        }

        return sourceFilePaths;
    }
}