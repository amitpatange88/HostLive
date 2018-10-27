using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HostLive
{
    public class HostLiveSecrets
    {
        public string project { get; set; }
        public string Description { get; set; }
        public Gmail Gmail { get; set; }
        public Yahoo Yahoo { get; set; }
    }

    public class Gmail
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class Yahoo
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class RabbitMQ
    {
        public string queue { get; set; }
    }

    public class Designpattern
    {
        public string name { get; set; }
    }

    public class Secrets
    {
        public HostLiveSecrets HostLiveSecrets { get; set; }
        public IList<RabbitMQ> RabbitMQ { get; set; }
        public Designpattern designpattern { get; set; }
    }

}