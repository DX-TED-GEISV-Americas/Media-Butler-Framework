using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaButler.BaseProcess.AMSSupport
{
    public enum PublishStatus
    {
        NotPublished = 0,
        PublishedActive = 1,
        PublishedFuture = 2,
        PublishedExpired = 3,
    }
}
