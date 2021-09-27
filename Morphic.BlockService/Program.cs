using System;
using System.ServiceProcess;

namespace Morphic.BlockService
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBase.Run(new BlockingService());

//            new BlockingService();
        }
    }
}
