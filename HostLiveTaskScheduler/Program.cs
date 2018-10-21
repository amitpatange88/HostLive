using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostLiveTaskScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            HostLiveService.HostLiveClient h = new HostLiveService.HostLiveClient("WSHttpBinding_IHostLive");
            bool IsDone = h.Start();
        }
    }
}
