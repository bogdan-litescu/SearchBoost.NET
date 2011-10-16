using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SearchBoost.Net.Core.Engine;

namespace SearchBoost.Net.Core.Storage
{
    public interface ISearchIndexStorage
    {
        void Index(SbSearchDoc indexDoc);
        IList<SbSearchDoc> Search(string terms);
        void ClearIndex();
    }
}
