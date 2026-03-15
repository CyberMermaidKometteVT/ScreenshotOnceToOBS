using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScreenshotOnceToOBS
{

    internal class SnippingToolProcessWatcher : IDisposable
    {
        public string FoundScreenshotPath { get; private set; } = null;
        public WatchingState State { get; private set; } = WatchingState.NotStarted;

        private CancellationTokenSource _cancellationTokenSource;
        private FileSystemWatcher _activeWatcher;
        private readonly dynamic CPH;

        public SnippingToolProcessWatcher(dynamic cph)
        {
            CPH = cph;
        }

        public async Task StartMonitoring()
        {
            StopMonitoring();

            State = WatchingState.WaitingForProcessStart;

            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;

            int lastCount = CountScreenshotProcesses();

            while (!token.IsCancellationRequested)
            {
                lock (this)
                {
                    //Waiting for state: one new process started
                    int currentCount = CountScreenshotProcesses();
                    if (State == WatchingState.WaitingForProcessStart && currentCount > lastCount)
                    {
                        CPH.LogInfo("Snipping Tool process started, waiting for screenshot...");
                        StartWatchingFileSystem();
                        State = WatchingState.WaitingForScreenshotOrProcessExit;
                        lastCount = currentCount;
                    }

                    //Waiting for state: Screenshot was taken OR the snipping tool process was closed


                    //Screenshot was taken: return the screenshot path
                    if (State == WatchingState.FoundScreenshot)
                    {
                        CPH.LogInfo("Returning screenshot path: " + FoundScreenshotPath);
                        return;
                    }

                    //Process was closed but no screenshot was taken: return null
                    if (State == WatchingState.WaitingForScreenshotOrProcessExit && currentCount < lastCount)
                    {
                        CPH.LogInfo("Snipping Tool process closed without taking a screenshot.");
                        State = WatchingState.Canceled;
                        Dispose();
                        return;
                    }

                    //Otherwise, delay and continue polling
                }

                await Task.Delay(100, token);
            }
        }

        public void StopMonitoring()
        {
            _cancellationTokenSource?.Cancel();
        }

        public void StopWatching()
        {
            _activeWatcher?.Dispose();
            _cancellationTokenSource?.Dispose();
            State = WatchingState.NotStarted;
        }

        public void Dispose()
        {
            StopWatching();
        }

        private void StartWatchingFileSystem()
        {
            _activeWatcher?.Dispose();

            _activeWatcher = new FileSystemWatcher
            {
                Path = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Pictures\Screenshots")
            };
            _activeWatcher.Created += (_, fileSystemEventArgs) => OnFileCreated(fileSystemEventArgs);
            _activeWatcher.EnableRaisingEvents = true;
        }

        private static int CountScreenshotProcesses()
        {
            var currentProcesses = Process.GetProcesses();
            var screenshotProcessCount = currentProcesses
                .Where(currentProcess => string.Equals(currentProcess.ProcessName, "SnippingTool", StringComparison.OrdinalIgnoreCase))
                .Count();
            return screenshotProcessCount;
        }

        private void OnFileCreated(FileSystemEventArgs fileSystemEventArgs)
        {
            CPH.LogInfo($"New screenshot detected: {fileSystemEventArgs.FullPath}");
            FoundScreenshotPath = fileSystemEventArgs.FullPath;
            State = WatchingState.FoundScreenshot;
            _activeWatcher?.Dispose();
        }

    }


    public enum WatchingState
    {
        NotStarted,
        WaitingForProcessStart,
        WaitingForScreenshotOrProcessExit,
        FoundScreenshot,
        Canceled,
    }
}