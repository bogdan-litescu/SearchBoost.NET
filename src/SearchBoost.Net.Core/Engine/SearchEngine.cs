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
