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
using System.IO;
using System.Linq;

using ZetaLongPaths;

namespace SuperClean.Helpers
{
    public static class IOHelpers
    {
        public static IEnumerable<string> GetDirectories(string currentDirectory, IReadOnlyCollection<string> directoryNames)
        {
            foreach (var directoryInfo in new ZlpDirectoryInfo(currentDirectory).GetDirectories("*.*", SearchOption.AllDirectories))
            {
                if (directoryNames.Any(s => directoryInfo.Name.EndsWith(s, StringComparison.OrdinalIgnoreCase)))
                {
                    yield return directoryInfo.FullName;
                }

                foreach (var inner in GetDirectories(directoryInfo.FullName, directoryNames).ToArray())
                {
                    yield return inner;
                }
            }
        }

        public static IEnumerable<IOperationResult> DeleteFiles(string directory, string mask)
        {
            foreach (var file in new ZlpDirectoryInfo(directory).GetFiles(mask))
            {
                string failureMessage = null;

                try
                {
                    file.Delete();
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
                    yield return OperationResult.Success(new[] { file.FullName });
                }
            }
        }
    }
}