using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SearchBoost.Net.Core.Services
{
    [ServiceContract]
    public interface IIndexingService
    {
        [OperationContract]
        string DoWork();
    }
}
