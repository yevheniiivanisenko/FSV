using System;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using FSV.Lib;

namespace FSV.App
{
    class Program
    {
        private static CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();
        public const string Folder = "./files";
        public const string SearchFile = "./files/notes.txt";
        public const string ExcludeFile = "./files/list.txt";

        static void Main(string[] args)
        {
            var pattern = @"\.txt$";
            FileSystemVisitor fs = new(name => Regex.IsMatch(name, pattern));

            // Subscribe to events
            fs.SearchFinished += c_SearchFinished;
            fs.SearchStarted += c_SearchStarted;
            fs.FileFound += c_FileFound;
            fs.DirectoryFound += c_DirectoryFound;
            fs.FilteredFileFound += c_FilteredFileFound;
            fs.FilteredDirectoryFound += c_FilteredDirectoryFound;

            foreach (var file in fs.GetFiles(Folder))
            {
                continue;
            }
        }

        static void c_SearchFinished(object sender, EventArgs e)
        {
            Console.WriteLine("Search finished.");
        }

        static void c_SearchStarted(object sender, EventArgs e)
        {
            Console.WriteLine("Search started.");
        }

        static void c_FileFound(object sender, FileEntryReachedEventArgs e)
        {
            Console.WriteLine("{0} {1} file was found.", e.TimeReached, e.Name);
        }

        static void c_DirectoryFound(object sender, FileEntryReachedEventArgs e)
        {
            Console.WriteLine("{0} {1} directory was found.", e.TimeReached, e.Name);
        }

        static void c_FilteredFileFound(object sender, FileEntryReachedEventArgs e)
        {
            if (e.Name == ExcludeFile)
            {
                e.Exclude = true;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0} {1} file was returned.", e.TimeReached, e.Name);
            Console.ResetColor();
        }

        static void c_FilteredDirectoryFound(object sender, FileEntryReachedEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0} {1} direcrory was returned.", e.TimeReached, e.Name);
            Console.ResetColor();
        }
    }
}
