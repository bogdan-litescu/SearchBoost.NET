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
using System.IO;
using SearchBoost.Net.Core.Engine;
using HtmlAgilityPack;
using System.Xml;
using Castle.Core.Logging;

namespace SearchBoost.Net.Core.ContentParsing
{
    public class XmlSitemapAndRssFeedsParser : IContentParser
    {
        public IList<string> MimeTypes { get; set; }
        public IList<string> FileExtensions { get; set; }
        public ILogger Logger { get; set; }

        public IList<ParsedContent> ParseRaw(string rawContent)
        {
            // This is RSS or Sitemap
            XmlDocument xmlDoc = new XmlDocument();
            try {
                xmlDoc.LoadXml(rawContent);
            } catch (Exception ex) {
                Logger.Error("Invalid XML!", ex);
                Logger.Debug(rawContent);
                return new List<ParsedContent>();
            }

            // check type
            var parsed = new ParsedContent();

            if (xmlDoc.DocumentElement.Name == "urlset") {
                // this is a sitemap
                XmlNamespaceManager mgr = new XmlNamespaceManager(xmlDoc.NameTable);
                mgr.AddNamespace("ns", "http://www.sitemaps.org/schemas/sitemap/0.9");

                foreach (XmlElement xmlUrl in xmlDoc.DocumentElement.SelectNodes("//ns:url", mgr)) {
                    parsed.Links.Add(new ParsedLink() {
                        Url = xmlUrl["loc"].InnerText.Trim()
                    });
                }

            } else if (xmlDoc.DocumentElement.Name == "rss") {

                try { parsed.Title = xmlDoc.DocumentElement["channel"]["title"].InnerText; } catch { }
                try { parsed.Description = xmlDoc.DocumentElement["channel"]["description"].InnerText; } catch { }
                try { parsed.Author = xmlDoc.DocumentElement["channel"]["managingEditor"].InnerText; } catch { }
                try { parsed.Metadata["link"] = xmlDoc.DocumentElement["channel"]["link"].InnerText; } catch { }

                foreach (XmlElement xmlUrl in xmlDoc.DocumentElement["channel"].SelectNodes("item")) {
                    var link = new ParsedLink();
                    link.Url = xmlUrl["link"].InnerText.Trim();
                    try { link.Title = xmlUrl["title"].InnerText.Trim(); } catch { }
                    try { link.Description = xmlUrl["description"].InnerText.Trim(); } catch { }
                    parsed.Links.Add(link);
                }
            }

            return new ParsedContent[] { parsed };
        }

        public IList<ParsedContent> ParseStream(Stream s)
        {
            using (StreamReader sr = new StreamReader(s)) {
                return ParseRaw(sr.ReadToEnd());
            }
        }

        public IList<ParsedContent> ParseFile(string filePath)
        {
            if (!File.Exists(filePath))
                return new ParsedContent[0];
            return ParseRaw(File.ReadAllText(filePath));
        }

        public IList<ParsedContent> ParseUrl(Uri url)
        {
            throw new NotImplementedException();
        }


        void ReadMeta(HtmlDocument doc, ref ParsedContent parsed)
        {
            // first, normalize values for meta name attribute
            if (doc.DocumentNode.SelectNodes("/html/head/meta") != null) {
                foreach (HtmlNode xmlMeta in doc.DocumentNode.SelectNodes("/html/head/meta")) {
                    xmlMeta.Attributes["name"].Value = xmlMeta.Attributes["name"].Value.ToLower();
                }
            }

            if (string.IsNullOrEmpty(parsed.Title)) {
                try { parsed.Title = doc.DocumentNode.SelectSingleNode("/html/head/title").InnerText.Trim(); } catch { }
            }

            if (string.IsNullOrEmpty(parsed.Description)) {
                try { parsed.Description = doc.DocumentNode.SelectSingleNode("/html/head/meta[@name='description']").Attributes["content"].Value.Trim(); } catch { }
            }
        }

    }
}
