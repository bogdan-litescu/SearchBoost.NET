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

namespace SearchBoost.Net.Core.ContentParsing
{
    public static class FindParser
    {
        static Dictionary<string, List<IContentParser>> _ContentParsersByMimeType;
        static Dictionary<string, List<IContentParser>> _ContentParsersByFileExtension;

        static FindParser()
        {
            _ContentParsersByMimeType = new Dictionary<string, List<IContentParser>>();
            _ContentParsersByFileExtension = new Dictionary<string, List<IContentParser>>();

            foreach (IContentParser parser in SbApp.Instance.Container.ResolveAll<IContentParser>()) {
                
                foreach (string mimeType in parser.MimeTypes) {
                    if (!_ContentParsersByMimeType.ContainsKey(mimeType))
                        _ContentParsersByMimeType[mimeType] = new List<IContentParser>();
                    _ContentParsersByMimeType[mimeType].Add(parser);
                }

                foreach (string ext in parser.FileExtensions) {
                    if (!_ContentParsersByFileExtension.ContainsKey(ext))
                        _ContentParsersByFileExtension[ext] = new List<IContentParser>();
                    _ContentParsersByFileExtension[ext].Add(parser);
                }
            }
        }

        public static IList<IContentParser> ByMimeContentType(string contentType)
        {
            contentType = contentType.Trim().ToLower();
            if (_ContentParsersByMimeType.ContainsKey(contentType))
                return _ContentParsersByMimeType[contentType];
            return new IContentParser[0];
        }

        public static IList<IContentParser> ByFileExtension(string fileExt)
        {
            fileExt = fileExt.Trim('.').ToLower();
            if (_ContentParsersByFileExtension.ContainsKey(fileExt))
                return _ContentParsersByFileExtension[fileExt];
            return new IContentParser[0];
        }
    }
}
