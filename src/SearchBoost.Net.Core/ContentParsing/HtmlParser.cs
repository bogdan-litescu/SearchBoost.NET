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

namespace SearchBoost.Net.Core.ContentParsing
{
    public class HtmlParser : IContentParser
    {
        public IList<string> MimeTypes { get; set; }
        public IList<string> FileExtensions { get; set; }

        public IList<ParsedContent> ParseRaw(string rawContent, FollowLinksOptions linkOpts)
        {
            ParsedContent parsed = new ParsedContent();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(rawContent);
            ReadMeta(doc, ref parsed);

            foreach (string invalidNode in new string[] { "script", "style", "link", "object", "embed", "title" }) {
                foreach (HtmlNode script in new List<HtmlNode>(doc.DocumentNode.Descendants(invalidNode)))
                    script.Remove();
            }

            HtmlNode body = doc.DocumentNode.SelectSingleNode("/html/body");
            if (body == null)
                return new ParsedContent[] { parsed };

            // extract links to foolow
            if (true) {
                HtmlNodeCollection links = body.SelectNodes("//a");
                if (links != null) {
                    foreach (HtmlNode a in links) {
                        parsed.Links.Add(new ParsedLink(a));
                    }
                }
            }
            
            // this is plain page, extract and index as HTML
            parsed.PlainContent = body.InnerText.Trim();
            parsed.LinkOpts = linkOpts;
            parsed.LinkOpts.CurrentDepth++;

            return new ParsedContent[] { parsed };
        }

        public IList<ParsedContent> ParseStream(Stream s, FollowLinksOptions linkOpts)
        {
            using (StreamReader sr = new StreamReader(s)) {
                return ParseRaw(sr.ReadToEnd(), linkOpts);
            }
        }

        public IList<ParsedContent> ParseFile(string filePath, FollowLinksOptions linkOpts)
        {
            if (!File.Exists(filePath))
                return new ParsedContent[0];
            return ParseRaw(File.ReadAllText(filePath), linkOpts);
        }

        public IList<ParsedContent> ParseUrl(Uri url, FollowLinksOptions linkOpts)
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
