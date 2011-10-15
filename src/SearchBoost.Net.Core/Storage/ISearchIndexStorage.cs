using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchBoost.Net.Core.Storage
{
    public interface ISearchIndexStorage
    {
        void Index(string content);
        IList<string> Search(string terms);
        void ClearIndex();
    }
}
