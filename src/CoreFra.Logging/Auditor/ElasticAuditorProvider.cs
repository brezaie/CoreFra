using System;
using System.Collections.Generic;
using CoreFra.Domain;

namespace CoreFra.Logging
{
    public class ElasticAuditorProvider : IAuditorProvider
    {
        public List<Audit> Get(DateTime fromDate, DateTime toDate, int pageSize, NetTcpStyleUriParser pageNumber)
        {
            throw new NotImplementedException();
        }

        public void Save(Audit entity)
        {
            throw new NotImplementedException();
        }
    }
}