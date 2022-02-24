using System;
using System.ServiceProcess;

namespace ALARmService
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var service = new ALARmService())
            {
                ServiceBase.Run(service);
                //service.onDebug();
            }
        }
    }
}
