/*
 * This file is part of the SearchBoost.NET project.
 * Copyright (c) 2011 Bogdan Litescu
 * Authors: Bogdan Litescu
 *
 * SearchBoost.NET is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SearchBoost.NET is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with SearchBoost.NET. If not, see <http://www.gnu.org/licenses/>.
 * 
 * You can be released from the requirements of the license by purchasing
 * a commercial license. Buying such a license is mandatory as soon as you
 * develop commercial activities involving the software without
 * disclosing the source code of your own applications.
 *
 * For more information, please contact us at support@dnnsharp.com
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using System.Net;
using HtmlAgilityPack;
using System.Xml;
using SearchBoost.Net.Core.Engine;
using System.IO;
using SearchBoost.Net.Core;
using SearchBoost.Net.Core.ContentParsing;
using SearchBoost.Net.Core.Indexers;

namespace SearchBoost.Net.WebSpider
{
    public class Crawler : IIndexer
    {
        public Crawler(ILogger logger, IEnumerable<CrawlJob> urls)
        {
            Logger = logger;
            Urls = urls;

            logger.Info("Web Spider instantiated...");
            if (logger.IsDebugEnabled) {
                logger.Debug("List of URLs to index:");
                foreach (CrawlJob u in urls)
                    logger.Debug(string.Format("  > {0}", u.Url.ToString()));
            }
        }

        public ILogger Logger { get; set; }
        public IEnumerable<CrawlJob> Urls { get; set; }

        public void Index()
        {
            Logger.Info("Running web crawler...");
            foreach (CrawlJob job in Urls) {
                Index(job);
            }
        }

        void Index(CrawlJob job)
        {
            Logger.Debug(string.Format("Parsing URL {0}", job.Url));

            // download content
            IDictionary<string, string> httpHeaders;
            string rawContent;
            try {
                rawContent = Download(job, out httpHeaders);
            } catch (WebException ex) {
                Logger.Error("Error downloading " + job.Url, ex);
                return;
            }

            // TODO: treat exceptions

            string mimeContentType = httpHeaders["Content-Type"];
            if (mimeContentType.IndexOf(';') > 0)
                mimeContentType = mimeContentType.Substring(0, mimeContentType.IndexOf(';'));

            Logger.Debug(string.Format("  > MIME Content Type: ", mimeContentType));

            IList<IContentParser> parsersByMimeType = FindParser.ByMimeContentType(mimeContentType);
            foreach (IContentParser parser in parsersByMimeType) {
                foreach (ParsedContent parsed in parser.ParseRaw(rawContent)) {
                    SbApp.Instance.SearchEngine.Index(parsed.ToSearchDoc());

                    // if it has links, index them too
                    foreach (var link in parsed.Links) {
                        Index(job.CreateJob(link));
                    }
                }
            }
        }

        string Download(CrawlJob opts, out IDictionary<string, string> httpHeaders)
        {
            System.Net.ServicePointManager.Expect100Continue = false;

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(opts.Url);
            httpRequest.Timeout = opts.TimeoutSec * 1000;

            HttpWebResponse response = (HttpWebResponse)httpRequest.GetResponse();
            System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream());
            string strResponse = reader.ReadToEnd();
            response.Close();

            // read HTTP headers
            httpHeaders = new Dictionary<string, string>();
            foreach (string header in response.Headers.AllKeys) {
                httpHeaders[header] = response.Headers[header];
            }

            return strResponse.Trim();
        }

    }
}
