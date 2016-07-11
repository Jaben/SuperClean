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
using System.Reflection;

using SuperClean.Helpers;

namespace SuperClean
{
    internal class Program
    {
        static string Version => _version.Value;
        static readonly Lazy<string> _version = new Lazy<string>(
            () =>
                {
                    Version version = Assembly.GetExecutingAssembly().GetName().Version;
                    return $"{version.Major}.{version.Minor}";
                });

        private static void Main(string[] args)
        {
            OutputConsoleHeader();

            var root = Directory.GetCurrentDirectory();

            var searchDirectoriesNamed = new[] { "bin", "debug", "release" };
            var fileMasks = new[] { "*.dll", "*.pdb", "*.exe" };

            try
            {
                var foundDirectories = IOHelpers.GetDirectories(root, searchDirectoriesNamed).OrderBy(s => s.Length).ToList();

                var totalSuccess = new List<IOperationResultSuccess>();

                foreach (var directory in foundDirectories)
                {
                    List<IOperationResult> results = new List<IOperationResult>();

                    try
                    {
                        foreach (var mask in fileMasks)
                        {
                            results.AddRange(IOHelpers.DeleteFiles(directory, mask));
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine($"Unable to access directory: {directory}");
                    }

                    if (results.Any())
                    {
                        var success = results.OfType<IOperationResultSuccess>().ToList();
                        if (success.Any())
                        {
                            Console.WriteLine($"Deleted {success.Count} Files in Directory {directory}");
                            totalSuccess.AddRange(success);
                        }

                        var failure = results.OfType<IOperationResultFailure>().ToList();
                        if (failure.Any())
                        {
                            failure.SelectMany(s => s.Messages).ToList().ForEach(s => Console.WriteLine(s));
                        }
                    }
                }

                if (!totalSuccess.Any())
                {
                    Console.WriteLine("No Files Found");
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failure: " + ex);
                Environment.Exit(1);
            }
        }

        static void OutputConsoleHeader()
        {
            WriteLineColor(ConsoleColor.DarkCyan, new string('*', 60));
            WriteColor(ConsoleColor.DarkCyan, "* ");
            WriteLineColor(ConsoleColor.Cyan, $"SuperClean (v{Version}) - Copyright 2016 CaptiveAire");
            WriteColor(ConsoleColor.DarkCyan, "* ");
            WriteLineColor(ConsoleColor.Gray, "https://github.com/CaptiveAire/SuperClean");
            WriteLineColor(ConsoleColor.DarkCyan, new string('*', 60));
            Console.WriteLine();
            Console.ResetColor();
        }

        static void WriteColor(ConsoleColor color, string line)
        {
            Console.ForegroundColor = color;
            Console.Write(line);
        }

        static void WriteLineColor(ConsoleColor color, string line)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(line);
        }
    }
}