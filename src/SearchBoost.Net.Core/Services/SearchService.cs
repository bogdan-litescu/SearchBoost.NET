using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using SearchBoost.Net.Core.Engine;
using Castle.Core.Logging;

namespace SearchBoost.Net.Core.Services
{
    public class SearchService : ISearchService
    {
        public SearchService(ILogger logger, string remote)
        {
            
        }

        public ILogger Logger { get; set; }
        public ISearchEngine SearchEngine { get; set; }
        
        public IList<SbSearchDoc> Search(string terms)
        {
            return SearchEngine.Search(terms);
        }

    }
}
