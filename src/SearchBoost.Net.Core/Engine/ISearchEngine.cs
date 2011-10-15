using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SearchBoost.Net.Core.Storage;

namespace SearchBoost.Net.Core.Engine
{
    public interface ISearchEngine
    {
        ISearchIndexStorage Storage { get; }
    }
}
