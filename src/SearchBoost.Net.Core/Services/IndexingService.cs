using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SearchBoost.Net.Core.Services
{
    public class IndexingService : IIndexingService
    {
        public string DoWork()
        {
            return "Done";
        }
    }
}
