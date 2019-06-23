using System;
using System.Collections.Generic;
using CoreFra.Domain;

namespace CoreFra.Logging
{
    public interface IAuditorProvider
    {
        List<Audit> Get(DateTime fromDate, DateTime toDate, int pageSize, NetTcpStyleUriParser pageNumber);
        void Save(Domain.Audit entity);
    }
}