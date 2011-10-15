using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SearchBoost.Net.Core.Storage;
using Castle.Core.Logging;

namespace SearchBoost.Net.Core.Engine
{
    public class SearchEngine : ISearchEngine
    {
        public SearchEngine()
        {
        }

        public ILogger Logger { get; set; }
        public ISearchIndexStorage Storage { get; set; }

    }
}
