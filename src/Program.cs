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

using System.Reflection;

using Serilog;

using SuperClean;
using SuperClean.FileSystem;

OutputConsoleHeader();

using var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

var root = Directory.GetCurrentDirectory();

var searchDirectoriesNamed = new[] { "bin", "obj" };
var fileMasks = new[] { "*.dll", "*.pdb", "*.exe", "*.cache" };
var ignoreDirectoriesNamed = new[] { ".git", ".vs", ".build", ".nuget", "node_modules", "packages" };

try
{
    var fileSystemHelper = new FileSystemHelper(new DefaultFileSystem(), logger);

    var foundDirectories = fileSystemHelper.GetDirectories(root, searchDirectoriesNamed, ignoreDirectoriesNamed);

    var totalSuccess = fileSystemHelper.DeleteFilesInDirectories(foundDirectories, fileMasks, ignoreDirectoriesNamed)
        .OfType<IOperationResultSuccess>()
        .ToList();

    if (totalSuccess.Count == 0)
    {
        logger.Information("No files found");
    }
}
catch (Exception ex)
{
    logger.Fatal(ex, "Unhandled exception");
    return 1;
}

Console.WriteLine();

return 0;

static void OutputConsoleHeader()
{
    var version = Assembly.GetEntryAssembly()?.GetName().Version;
    var versionText = version is null ? "" : $" (v{version.Major}.{version.Minor})";

    WriteLineColor(ConsoleColor.DarkCyan, new string('*', 60));
    WriteColor(ConsoleColor.DarkCyan, "* ");
    WriteLineColor(ConsoleColor.Cyan, $"SuperClean{versionText} - Copyright 2016-2026 Jaben Cargman");
    WriteColor(ConsoleColor.DarkCyan, "* ");
    WriteLineColor(ConsoleColor.Gray, "https://github.com/Jaben/SuperClean");
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
