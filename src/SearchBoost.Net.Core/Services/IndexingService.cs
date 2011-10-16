using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Castle.Core.Logging;
using SearchBoost.Net.Core.Engine;

namespace SearchBoost.Net.Core.Services
{
    public class IndexingService : IIndexingService
    {
        public IndexingService(ILogger logger, string remote)
        {
            
        }

        public ILogger Logger { get; set; }
        public ISearchEngine SearchEngine { get; set; }

        public bool Index(Engine.SbSearchDoc indexDoc)
        {
            return SearchEngine.Index(indexDoc);
        }

        public bool ClearIndex()
        {
            return SearchEngine.ClearIndex();
        }
    }
}
