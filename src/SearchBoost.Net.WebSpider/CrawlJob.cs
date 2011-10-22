using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.SubSystems.Conversion;

namespace SearchBoost.Net.WebSpider
{
    [Convertible]
    public class CrawlJob
    {
        public CrawlJob(Uri url)
        {
            Url = url;
            TimeoutSec = 30;
        }

        public Uri Url { get; private set; }
        
        public int TimeoutSec { get; set; }
        public string OverrideTitle { get; set; }
        public string OverrideDesc { get; set; }
    }

}
