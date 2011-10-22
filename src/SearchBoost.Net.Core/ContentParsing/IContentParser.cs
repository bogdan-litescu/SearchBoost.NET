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

namespace SearchBoost.Net.Core.ContentParsing
{
    public interface IContentParser
    {
        /// <summary>
        /// List of file extensions this content parser can handle. 
        /// This is populated from coniguration file, there's no way to do it programatically right now.
        /// </summary>
        IList<string> FileExtensions { get; }

        /// <summary>
        /// List of MIME Content Types this content parser can handle. 
        /// This is populated from coniguration file, there's no way to do it programatically right now.
        /// </summary>
        IList<string> MimeTypes { get; }

        /// <summary>
        /// Parse content from raw string source.This is a common method to call for plain text formats.
        /// </summary>
        /// <param name="rawContent"></param>
        /// <returns>List of parsed content, most of the time with only one element.</returns>
        IList<ParsedContent> ParseRaw(string rawContent);

        /// <summary>
        /// Parse content from a stream. This can be from a file, a web response and so on.
        /// </summary>
        /// <param name="s"></param>
        /// <returns>List of parsed content, most of the time with only one element.</returns>
        IList<ParsedContent> ParseStream(Stream s);

        /// <summary>
        /// Parse content from file on disk.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>List of parsed content, most of the time with only one element.</returns>
        IList<ParsedContent> ParseFile(string filePath);

        /// <summary>
        /// Parse content from give URL.
        /// </summary>
        /// <param name="url"></param>
        /// <returns>List of parsed content, most of the time with only one element.</returns>
        IList<ParsedContent> ParseUrl(Uri url);
    }
}
