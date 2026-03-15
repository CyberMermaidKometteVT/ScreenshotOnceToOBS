using System;
using System.Threading.Tasks;

namespace ScreenshotOnceToOBS
{
    public partial class CPHInline
    {
        public bool Execute()
        {
            // your main code goes here

            GrabOneScreenshotAndPutItInOBS()
                .Wait();
            return true;
        }

        public async Task GrabOneScreenshotAndPutItInOBS()
        {

            //Console.CancelKeyPress += (_, cancelEventArgs) =>
            //{
            //    Console.WriteLine("Ctrl+C received, shutting down...");
            //    cancelEventArgs.Cancel = true; // prevent immediate process kill
            //    _quitEvent.Set();
            //};

            try
            {
                var processMonitor = new SnippingToolProcessWatcher(CPH);
                var processMonitorTask = processMonitor.StartMonitoring();

                // Launch Snip & Sketch (Windows 10 and later)
                string pathToExplorerDotExe = Environment.ExpandEnvironmentVariables(@"%WINDIR%\explorer.exe");
                System.Diagnostics.Process.Start(pathToExplorerDotExe, "ms-screenclip:");

                await processMonitorTask;

                //The screenshot process is complete - we either took a screenshot or canceled.
                if (processMonitor.State == WatchingState.Canceled)
                {
                    CPH.LogInfo(@"Screenshot canceled. Exiting prorgam.");
                    return;
                }

                if (processMonitor.State == WatchingState.FoundScreenshot)
                {
                    CPH.LogInfo($"Screenshot found at path: {processMonitor.FoundScreenshotPath}");

                    //Send the screenshot to OBS
                    new ScreenshotSender(CPH).Execute(processMonitor.FoundScreenshotPath);
                }

                //WaitForExitSignal();y
                CPH.LogInfo("Exiting screenshot tool.");
            }
            catch (Exception ex)
            {
                CPH.LogError($"An unhandled exception occurred at the Program.cs level: {ex.Message}");
                //CPH.LogInfo("Press Enter to exit...");
                //Console.ReadLine();
            }

        }

        //void WaitForExitSignal()
        //{
        //    // Wait until Enter OR Ctrl+C triggers the quit event
        //    ThreadPool.QueueUserWorkItem(_ =>
        //    {
        //        Console.ReadLine();
        //        _quitEvent.Set();
        //    });

        //    _quitEvent.Wait();
        //}





    }
}
