﻿using RevenueControl.DomainObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueControl.DomainObjects.Interfaces
{
    public interface IDataSourceRepository : IDisposable
    {
        DataSource GetDataSource(DataSource dataSource);
        IEnumerable<DataSource> GetClientDataSources(Client client);
    }

}
