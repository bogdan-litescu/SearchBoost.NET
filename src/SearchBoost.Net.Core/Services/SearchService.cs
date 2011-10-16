using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using SearchBoost.Net.Core.Engine;

namespace SearchBoost.Net.Core.Services
{
    public class SearchService : ISearchService
    {
        public IList<string> Search(string terms)
        {
            return SbApp.Instance.SearchEngine.Storage.Search(terms);
        }

    }
}
