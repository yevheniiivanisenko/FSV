using System;
using System.Threading;

namespace FSV.Lib
{
    public class FileEntryReachedEventArgs : EventArgs
    {
        private CancellationTokenSource _cancellationTokenSource;

        public FileEntryReachedEventArgs(string name, DateTime timeReached, CancellationTokenSource cancellationTokenSource)
        {
            Name = name;
            TimeReached = timeReached;
            _cancellationTokenSource = cancellationTokenSource;
            Exclude = false;
        }

        public string Name { get; private set; }

        public DateTime TimeReached { get; private set; }

        public bool Exclude { get; set; }

        public void Terminate()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}