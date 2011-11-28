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
using SearchBoost.Net.Core.Extensions;

namespace SearchBoost.Net.Core.Engine
{
    public enum eMaiContentType
    {
        Unknown,
        WebPage,
        Doc
    }

    public class SbSearchDoc
    {
        public SbSearchDoc()
        {
            ContentTypes = new List<string>();
            Sources = new List<string>();
            Categories = new List<string>();
            LastModified = DateTime.Now;
            Location = "";

            Title = "";
            Description = "";
            Keywords = "";
            Author = "";
            PlainContent = "";
            Boost = 1.0f;
        }

        /// <summary>
        /// Load data directly from lucene document
        /// </summary>
        /// <param name="doc"></param>
        public SbSearchDoc(Lucene.Net.Documents.Document doc)
        {
            ContentTypes = doc.GetValues("type");
            Sources = doc.GetValues("source");
            Categories = doc.GetValues("category");

            try {
                LastModified = DateTimeEx.FromUnixTimestamp(Convert.ToInt32(doc.Get("lastmod")));
            } catch { LastModified = DateTime.MinValue; }

            Title = doc.Get("title");
            Keywords = doc.Get("keywords");
            Description = doc.Get("desc");
            Author = doc.Get("author");
            Location = doc.Get("loc");
            // PlainContent = doc.Get("content");
        }

        /// <summary>
        /// List of content types. 
        /// </summary>
        public IList<string> ContentTypes { get; set; }

        /// <summary>
        /// Where did the content came from; multiple sources are allowed
        /// </summary>
        public IList<string> Sources { get; set; }

        /// <summary>
        /// List of categories the content falls under
        /// </summary>
        public IList<string> Categories { get; set; }

        /// <summary>
        /// Tells when the content was last modified
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Let's make this generic string since we'll support both web links but local links too
        /// So we'll handle differently if a link is to a loca file for example and the search client is web based or desktop based
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// The title is displayed in the search results
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Keywords are also searchable with a higher boost.
        /// So if the search terms are found in keywords the result is considered more important.
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// Unless the content highlighter is enabed, this description will be shown in the search results
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// This is the processed content that will get indexed
        /// </summary>
        public string PlainContent { get; set; }

        /// <summary>
        /// Author of the content
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The boost factor determines if this content is more important than others.
        /// 1.0 means no boost
        /// </summary>
        public float Boost { get; set; }

    }
}
