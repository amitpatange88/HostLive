using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace HostingByConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(HostLive.HostLive)))
            {
                host.Open();
                Console.WriteLine("Service has started : " + DateTime.Now);
                Console.ReadLine();
            }
        }
    }
}
