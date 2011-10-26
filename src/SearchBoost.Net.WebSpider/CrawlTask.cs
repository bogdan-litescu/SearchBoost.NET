using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.SubSystems.Conversion;
using SearchBoost.Net.Core.ContentParsing;

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

        public CrawlJob CreateJob(ParsedLink forLink)
        {
            CrawlJob job = new CrawlJob(new Uri(forLink.Url));
            // TODO: rest of the params plus maybe some checkings
            return job;
        }
    }

}
