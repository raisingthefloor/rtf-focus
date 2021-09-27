using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.BlockWeb
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBase.Run(new BlockService());

            //new BlockService();
        }
    }
}
