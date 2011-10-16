using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SearchBoost.Net.Core.Storage;

namespace SearchBoost.Net.Core.Engine
{
    public interface ISearchEngine
    {
        void IndexAsync(SbSearchDoc indexDoc, Action<bool> onComplete);
        bool Index(SbSearchDoc indexDoc);
        
        void SearchAsync(string searchTerms, Action<IList<SbSearchDoc>> onReceiveResults);
        IList<SbSearchDoc> Search(string searchTerms);

        void ClearIndexAsync(Action<bool> onComplete);
        bool ClearIndex();
    }
}
