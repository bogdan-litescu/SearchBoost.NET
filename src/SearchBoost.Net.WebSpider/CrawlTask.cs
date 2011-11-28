using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.SubSystems.Conversion;
using SearchBoost.Net.Core.ContentParsing;
using System.IO;

namespace SearchBoost.Net.WebSpider
{
    [Convertible]
    public class CrawlJob
    {
        public CrawlJob(Uri url)
        {
            Url = url;
            TimeoutSec = 30;

            LinkOpts = new FollowLinksOptions();
        }

        public Uri Url { get; private set; }
        public FollowLinksOptions LinkOpts { get; set; }
         
        public int TimeoutSec { get; set; }
        public string OverrideTitle { get; set; }
        public string OverrideDesc { get; set; }

        public CrawlJob CreateJob(ParsedLink forLink)
        {
            Uri url;
            try {
                url = new Uri(forLink.Url);
            } catch (UriFormatException ex) {
                // relative URL, build it from current URL
                url = new Uri(Url.ToString().Substring(0, Url.ToString().LastIndexOf('/')) + "/" + forLink.Url);
            }

            CrawlJob job = new CrawlJob(url);
            job.LinkOpts = LinkOpts;

            // TODO: rest of the params plus maybe some checkings
            return job;
        }
    }

}
