using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace Morphic.BlockService
{
    class Program
    {
        static void Main(string[] args)
        {
            //EventLog.WriteEntry("Main");
            ServiceBase.Run(new BlockingService());

            //new BlockingService();
        }
    }
}
