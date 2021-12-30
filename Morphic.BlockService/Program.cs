using Morphic.Data.Services;
using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace Morphic.BlockService
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                BlockingService service1 = new BlockingService();
                service1.TestStartupAndStop(args);
            }
            else
            {
                ServiceBase.Run(new BlockingService());
            }
        }
    }
}
