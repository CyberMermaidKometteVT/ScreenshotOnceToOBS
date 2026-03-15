using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScreenshotOnceToOBS
{
    internal class Program
    {
        ManualResetEventSlim _quitEvent = new ManualResetEventSlim();

        public static void Main()
        {
            new CPHInline().Execute();
        }


        
    }
}
