namespace JWAdventOfCodeHandlerLibrary.Services;

public class JWAoCFileService
{
    public static IList<string> GetSourceFilePaths(string sourcePath, Func<string, bool> allowFilePath)
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
                IList<string> sourceFilePaths = new List<string>();
                foreach (var filePath in Directory.GetFiles(sourcePath))
                {
                    if (allowFilePath(filePath))
                    {
                        sourceFilePaths.Add(filePath);
                    }
                }
                foreach (var directoryPath in Directory.GetDirectories(sourcePath))
                {
                    foreach (string filePath in GetSourceFilePaths(directoryPath, allowFilePath))
                    {
                        sourceFilePaths.Add(filePath);
                    }
                }
                return sourceFilePaths;
            }
            catch
            {

            }
        }
        else
        {
            if (allowFilePath(sourcePath))
            {
                return new string[] { sourcePath };
            }
        }
        return new string[] { };
    }

}