using System;
using System.Threading;

namespace Streamer.bot.Plugin.Interface
{
    //The core library for interacting with StreamerBot.
    //This class won't get copied over - it's just here to 
    internal class CPH
    {
        public void LogDebug(string logLine)
        {
            Console.WriteLine($"DEBUG: {logLine}");
        }

        public void LogError(string logLine)
        {
            Console.WriteLine($"ERROR: {logLine}");
        }

        public void LogInfo(string logLine)
        {
            Console.WriteLine($"INFO: {logLine}");
        }

        public void LogWarn(string logLine) {
            Console.WriteLine($"WARN: {logLine}");
        }
        public void LogVerbose(string logLine) {
            Console.WriteLine($"VERBOSE: {logLine}");
        }

        internal bool TryGetArg(string argConnection, out string argValue)
        {
            argValue = "fakeConnection";
            return true;
        }

        internal bool TryGetArg(string argConnection, out int argValue)
        {
            argValue = 213213;
            return true;
        }

        internal bool TryGetArg(string argConnection, out decimal argValue)
        {
            argValue = 123911.234m;
            return true;
        }

        internal void Wait(int ms)
        {
            Thread.Sleep(ms);
        }

        internal string ObsSendRaw(string reqSetInputSettings, string textToSend, int connection)
        {
            return "response";
        }

        internal string ObsGetCurrentScene(int connection)
        {
            return "scene";
        }
    }
}
