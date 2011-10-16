using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using SearchBoost.Net.Core.Engine;

namespace SearchBoost.Net.Core.Services
{
    [ServiceContract]
    public interface IIndexingService
    {
        [OperationContract]
        bool Index(SbSearchDoc indexDoc);

        [OperationContract]
        bool ClearIndex();
    }
}
