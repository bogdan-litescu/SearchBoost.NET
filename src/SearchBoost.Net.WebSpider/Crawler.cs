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
using SearchBoost.Net.Core.Publishers;
using Castle.Core.Logging;
using System.Net;
using HtmlAgilityPack;
using System.Xml;
using SearchBoost.Net.Core.Engine;
using System.IO;
using SearchBoost.Net.Core;
using SearchBoost.Net.Core.ContentParsing;

namespace SearchBoost.Net.WebSpider
{
    public class Crawler : IContentPublisher
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

        public void Publish()
        {
            Logger.Info("Running web crawler...");
            foreach (CrawlJob opts in Urls) {

                Logger.Debug(string.Format("Parsing URL {0}", opts.Url));

                // download content
                IDictionary<string, string> httpHeaders;
                string rawContent = Download(opts, out httpHeaders);
                
                // TODO: treat exceptions

                //// check content type
                //// TODO: handle content types external to this component, for example to PDFs, Word Docx, etc
                //SbSearchDoc sbDoc = null;
                //if (httpHeaders["Content-Type"].StartsWith("text/plain")) {
                //    sbDoc = new SbSearchDoc() {
                //        Title = Path.GetFileName(opts.Url.ToString()),
                //        Content = rawContent
                //    };
                //} else if (httpHeaders["Content-Type"].StartsWith("text/html")) {
                //    sbDoc = ParseHtml(opts, rawContent);
                //} 
                ////else if (httpHeaders["Content-Type"].StartsWith("text/xml")) {
                ////    // TODO: This is RSS or Sitemap
                ////}

                string mimeContentType = httpHeaders["Content-Type"];
                if (mimeContentType.IndexOf(';') > 0)
                    mimeContentType = mimeContentType.Substring(0, mimeContentType.IndexOf(';'));

                Logger.Debug(string.Format("  > MIME Content Type: ", mimeContentType));

                IList<IContentParser> parsersByMimeType = FindParser.ByMimeContentType(mimeContentType);
                foreach (IContentParser parser in parsersByMimeType) {
                    foreach (ParsedContent parsed in parser.ParseRaw(rawContent)) {
                        SbApp.Instance.SearchEngine.Index(parsed.ToSearchDoc());
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

        //SbSearchDoc ParseHtml(CrawlOptions opts, string rawContent)
        //{
        //    HtmlDocument doc = new HtmlDocument();
        //    doc.LoadHtml(rawContent);

        //    foreach (string invalidNode in new string[] { "script", "style", "link", "object", "embed", "title" }) {
        //        foreach (HtmlNode script in new List<HtmlNode>(doc.DocumentNode.Descendants(invalidNode)))
        //            script.Remove();
        //    }

        //    // this is plain page, extract and index as HTML
        //    SbSearchDoc sbDoc = new SbSearchDoc() {
        //        Title = opts.OverrideTitle,
        //        Description = opts.OverrideDesc,
        //        Content = doc.DocumentNode.SelectSingleNode("/html/body").InnerText
        //    };
            
        //    ReadMeta(doc, ref sbDoc);
        //    return sbDoc;
        //}


        //void ReadMeta(HtmlDocument doc, ref SbSearchDoc sbDoc)
        //{
        //    // first, normalize values for meta name attribute
        //    foreach (HtmlNode xmlMeta in doc.DocumentNode.SelectNodes("/html/head/meta")) {
        //        xmlMeta.Attributes["name"].Value = xmlMeta.Attributes["name"].Value.ToLower();
        //    }

        //    if (string.IsNullOrEmpty(sbDoc.Title)) {
        //        try { sbDoc.Title = doc.DocumentNode.SelectSingleNode("/html/head/title").InnerText.Trim(); } catch { }
        //    }

        //    if (string.IsNullOrEmpty(sbDoc.Description)) {
        //        try { sbDoc.Description = doc.DocumentNode.SelectSingleNode("/html/head/meta[@name='description']").Attributes["content"].Value.Trim(); } catch { }
        //    }
        //}
    }
}
