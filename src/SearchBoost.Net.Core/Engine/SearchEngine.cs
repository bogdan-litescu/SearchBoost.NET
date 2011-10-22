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
using SearchBoost.Net.Core.Storage;
using Castle.Core.Logging;
using Castle.Facilities.WcfIntegration;
using SearchBoost.Net.Core.Services;

namespace SearchBoost.Net.Core.Engine
{
    public class SearchEngine : ISearchEngine
    {
        public SearchEngine()
        {
        }

        public ILogger Logger { get; set; }
        public ISearchIndexStorage Storage { get; set; }

        public void IndexAsync(SbSearchDoc indexDoc, Action<bool> onComplete)
        {
            if (Storage == null) {
                try {
                    var client = SbApp.Instance.Container.Resolve<IIndexingService>();
                    client.BeginWcfCall(
                        c => c.Index(indexDoc),
                        asyncCall => onComplete(asyncCall.End()),
                        null);
                } catch (Castle.MicroKernel.ComponentNotFoundException) {
                    Logger.Error("Storage not set; setup lucene storage or a remote storage!");
                    throw new Exception("Storage not set; setup lucene storage or a remote storage!");
                }
            } else {
                Storage.Index(indexDoc);
            }
        }

        public bool Index(SbSearchDoc indexDoc)
        {
            if (indexDoc == null)
                return true;

            if (Storage == null) {
                try {
                    var client = SbApp.Instance.Container.Resolve<IIndexingService>();
                    client.BeginWcfCall(c => c.Index(indexDoc)).End();
                } catch (Castle.MicroKernel.ComponentNotFoundException) {
                    Logger.Error("Storage not set; setup lucene storage or a remote storage!");
                    throw new Exception("Storage not set; setup lucene storage or a remote storage!");
                }
            } else {
                Storage.Index(indexDoc);
            }
            return true;
        }

        public void SearchAsync(string searchTerms, Action<IList<SbSearchDoc>> onReceiveResults)
        {
            if (Storage == null) {
                // use the search service
                try {
                    var client = SbApp.Instance.Container.Resolve<ISearchService>();
                    client.BeginWcfCall(
                        c => c.Search("container"),
                        asyncCall => onReceiveResults(asyncCall.End()),
                        null);
                } catch (Castle.MicroKernel.ComponentNotFoundException) {
                    Logger.Error("Storage not set; setup lucene storage or a remote storage!");
                    throw new Exception("Storage not set; setup lucene storage or a remote storage!");
                }
            } else {
                onReceiveResults(Storage.Search(searchTerms));
            }
        }

        public IList<SbSearchDoc> Search(string searchTerms)
        {
            if (Storage == null) {
                // use the search service
                try {
                    var client = SbApp.Instance.Container.Resolve<ISearchService>();
                    return client.BeginWcfCall(c => c.Search("container")).End();
                } catch (Castle.MicroKernel.ComponentNotFoundException) {
                    Logger.Error("Storage not set; setup lucene storage or a remote storage!");
                    throw new Exception("Storage not set; setup lucene storage or a remote storage!");
                }
            } else {
                return Storage.Search(searchTerms);
            }
        }

        public void ClearIndexAsync(Action<bool> onComplete)
        {
            if (Storage == null) {
                try {
                    var client = SbApp.Instance.Container.Resolve<IIndexingService>();
                    client.BeginWcfCall(
                        c => c.ClearIndex(),
                        asyncCall => onComplete(asyncCall.End()),
                        null);
                } catch (Castle.MicroKernel.ComponentNotFoundException) {
                    Logger.Error("Storage not set; setup lucene storage or a remote storage!");
                    throw new Exception("Storage not set; setup lucene storage or a remote storage!");
                }
            } else {
                Storage.ClearIndex();
            }
        }

        public bool ClearIndex()
        {
            if (Storage == null) {
                try {
                    var client = SbApp.Instance.Container.Resolve<IIndexingService>();
                    client.BeginWcfCall(c => c.ClearIndex()).End();
                } catch (Castle.MicroKernel.ComponentNotFoundException) {
                    Logger.Error("Storage not set; setup lucene storage or a remote storage!");
                    throw new Exception("Storage not set; setup lucene storage or a remote storage!");
                }
            } else {
                Storage.ClearIndex();
            }
            return true;
        }


    }
}
