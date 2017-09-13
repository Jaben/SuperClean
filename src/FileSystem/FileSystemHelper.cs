//  The MIT License (MIT)
//  
//  Copyright (c) 2016 CaptiveAire Limitied
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperClean.FileSystem
{
    public class FileSystemHelper
    {
        readonly IFileSystem _fileSystem;

        public FileSystemHelper(IFileSystem fileSystem)
        {
            this._fileSystem = fileSystem;
        }

        public string[] GetDirectories(
            string baseDirectory,
            string[] directoryNames,
            string[] ignoreDirectoriesNamed)
        {
            IEnumerable<string> GetDirectoriesRecursive(string currentDirectory)
            {
                Console.WriteLine("Checking Directory " + currentDirectory);

                foreach (var directory in this._fileSystem.GetDirectories(currentDirectory))
                {
                    if (ignoreDirectoriesNamed.Any(s => directory.EndsWith($@"\{s}", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        continue;
                    }

                    if (directoryNames.Any(s => directory.EndsWith(s, StringComparison.OrdinalIgnoreCase)))
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

                var nestedDirectories = this.GetDirectories(directory, new string[0], ignoreDirectoriesNamed);

                Console.WriteLine("Recurising into: " + string.Join(", ", nestedDirectories));

                foreach (var r in this.DeleteFilesInDirectories(nestedDirectories, fileMasks, ignoreDirectoriesNamed))
                    yield return r;
            }
        }

        public List<IOperationResult> DeleteFiles(string directory, string[] fileMasks)
        {
            List<IOperationResult> results = new List<IOperationResult>();

            foreach (var mask in fileMasks)
            {
                results.AddRange(this.DeleteFiles(directory, mask));
            }

            if (results.Any())
            {
                var success = results.OfType<IOperationResultSuccess>().ToList();
                if (success.Any())
                {
                    Console.WriteLine($"Deleted {success.Count} Files in Directory {directory}");
                }

                var failure = results.OfType<IOperationResultFailure>().ToList();
                if (failure.Any())
                {
                    failure.SelectMany(s => s.Messages).ToList().ForEach(s => Console.WriteLine(s));
                }
            }

            return results;
        }

        IEnumerable<IOperationResult> DeleteFiles(string directory, string mask)
        {
            foreach (var file in this._fileSystem.GetFiles(directory, mask))
            {
                string failureMessage = null;

                try
                {
                    this._fileSystem.DeleteFile(file);
                }
                catch (UnauthorizedAccessException)
                {
                    failureMessage = $"Access denied deleting file: {file}";
                }

                if (!string.IsNullOrEmpty(failureMessage))
                {
                    yield return OperationResult.Failure(new[] { failureMessage });
                }
                else
                {
                    yield return OperationResult.Success(new[] { file });
                }
            }
        }
    }
}