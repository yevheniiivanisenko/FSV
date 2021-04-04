using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace FSV.Lib
{
    public delegate bool FilterFileEntry(string input);

    public class FileSystemVisitor
    {
        private int _counter = 0;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public FileSystemVisitor()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            FilterFileEntryDel = (name) => true;
        }

        public FileSystemVisitor(FilterFileEntry filterFileEntry) : this()
        {
            FilterFileEntryDel = filterFileEntry;
        }

        public FilterFileEntry FilterFileEntryDel;
        public event EventHandler SearchStarted;
        public event EventHandler SearchFinished;
        public event EventHandler<FileEntryReachedEventArgs> FileFound;
        public event EventHandler<FileEntryReachedEventArgs> DirectoryFound;
        public event EventHandler<FileEntryReachedEventArgs> FilteredFileFound;
        public event EventHandler<FileEntryReachedEventArgs> FilteredDirectoryFound;

        public IEnumerable<string> GetFiles(string folder)
        {
            // The method has not made a recursive call yet.
            if (_counter == 0)
            {
                OnSearchStarted(EventArgs.Empty);
            }

            _counter++;

            IEnumerable<string> files = null;
            try
            {
                files = Directory.GetFileSystemEntries(folder);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            foreach (var file in files)
            {
                var isDirectory = Directory.Exists(file);
                var eventArgs = new FileEntryReachedEventArgs(file, DateTime.Now, _cancellationTokenSource);
                var isFiltered = FilterFileEntryDel?.Invoke(file) == true;

                if (_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    Console.WriteLine("The search has been terminated.");
                    break;
                }

                if (isDirectory)
                {
                    OnDirectoryFound(eventArgs);
                }

                if (!isDirectory)
                {
                    OnFileFound(eventArgs);
                }

                // Fire a corresponding event to every file that was filtered.
                if (isFiltered && isDirectory)
                {
                    OnFilteredDirectoryFound(eventArgs);
                }

                if (isFiltered && !isDirectory)
                {
                    OnFilteredFileFound(eventArgs);
                }

                if (isFiltered && isDirectory)
                {
                    foreach (var f in GetFiles(file))
                    {
                        yield return f;
                    }
                }

                if (eventArgs.Exclude)
                {
                    Console.WriteLine("File {0} was excluded.", file);
                    continue;
                }
                else
                {
                    yield return file;
                }
            }

            _counter--;

            // All recursive calls have been executed. 
            if (_counter == 0)
            {
                OnSearchFinished(EventArgs.Empty);
            }
        }

        protected virtual void OnSearchStarted(EventArgs e)
        {
            EventHandler handler = SearchStarted;
            handler?.Invoke(this, e);
        }

        protected virtual void OnSearchFinished(EventArgs e)
        {
            EventHandler handler = SearchFinished;
            handler?.Invoke(this, e);
        }

        protected virtual void OnFileFound(FileEntryReachedEventArgs e)
        {
            EventHandler<FileEntryReachedEventArgs> handler = FileFound;
            handler?.Invoke(this, e);
        }

        protected virtual void OnDirectoryFound(FileEntryReachedEventArgs e)
        {
            EventHandler<FileEntryReachedEventArgs> handler = DirectoryFound;
            handler?.Invoke(this, e);
        }

        protected virtual void OnFilteredFileFound(FileEntryReachedEventArgs e)
        {
            EventHandler<FileEntryReachedEventArgs> handler = FilteredFileFound;
            handler?.Invoke(this, e);
        }

        protected virtual void OnFilteredDirectoryFound(FileEntryReachedEventArgs e)
        {
            EventHandler<FileEntryReachedEventArgs> handler = FilteredDirectoryFound;
            handler?.Invoke(this, e);
        }
    }
}
