//  The MIT License (MIT)
//
//  Copyright (c) 2016 Jaben Cargman
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy of
//  this software and associated documentation files (the "Software"), to deal in
//  the Software without restriction, including without limitation the rights to
//  use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
//  the Software, and to permit persons to whom the Software is furnished to do so,
//  subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//  FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//  COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//  IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//  CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using Serilog;

namespace SuperClean.FileSystem;

public class FileSystemHelper
{
    readonly IFileSystem _fileSystem;
    readonly ILogger _logger;

    public FileSystemHelper(IFileSystem fileSystem, ILogger logger)
    {
        this._fileSystem = fileSystem;
        this._logger = logger;
    }

    public string[] GetDirectories(
        string baseDirectory,
        string[]? includeDirectoriesNamed,
        string[]? excludeDirectoriesNamed)
    {
        IEnumerable<string> GetDirectoriesRecursive(string currentDirectory)
        {
            foreach (var directory in this._fileSystem.GetDirectories(currentDirectory))
            {
                var directoryName = Path.GetFileName(directory);

                if (excludeDirectoriesNamed != null && excludeDirectoriesNamed.Contains(directoryName, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (includeDirectoriesNamed == null || includeDirectoriesNamed.Contains(directoryName, StringComparer.OrdinalIgnoreCase))
                {
                    yield return directory;
                }

                foreach (var inner in GetDirectoriesRecursive(directory))
                {
                    yield return inner;
                }
            }
        }

        return GetDirectoriesRecursive(baseDirectory).OrderBy(s => s.Length).ToArray();
    }

    public IEnumerable<IOperationResult> DeleteFilesInDirectories(string[] directories, string[] fileMasks, string[] ignoreDirectoriesNamed)
    {
        foreach (var directory in directories)
        {
            foreach (var r in this.DeleteFiles(directory, fileMasks))
                yield return r;

            var nestedDirectories = this.GetDirectories(directory, null, ignoreDirectoriesNamed);

            foreach (var r in this.DeleteFilesInDirectories(nestedDirectories, fileMasks, ignoreDirectoriesNamed))
                yield return r;
        }
    }

    public List<IOperationResult> DeleteFiles(string directory, string[] fileMasks)
    {
        var results = new List<IOperationResult>();

        foreach (var mask in fileMasks)
        {
            results.AddRange(this.DeleteFiles(directory, mask));
        }

        var successCount = results.OfType<IOperationResultSuccess>().Count();
        if (successCount > 0)
        {
            this._logger.Information("Deleted {SuccessCountFiles} File(s) in Directory {Directory}", successCount, directory);
        }

        return results;
    }

    IEnumerable<IOperationResult> DeleteFiles(string directory, string mask = "*")
    {
        foreach (var file in this._fileSystem.GetFiles(directory, mask))
        {
            string? failureMessage = null;

            try
            {
                this._fileSystem.DeleteFile(file);
            }
            catch (UnauthorizedAccessException)
            {
                this._logger.Warning("Access denied when deleting file {File}", file);
                failureMessage = $"Access denied deleting file: {file}";
            }
            catch (IOException ex)
            {
                this._logger.Warning("Unable to delete file {File}: {Reason}", file, ex.Message);
                failureMessage = $"Unable to delete file: {file}";
            }

            if (!string.IsNullOrEmpty(failureMessage))
            {
                yield return OperationResult.Failure([failureMessage]);
            }
            else
            {
                yield return OperationResult.Success([file]);
            }
        }
    }
}
