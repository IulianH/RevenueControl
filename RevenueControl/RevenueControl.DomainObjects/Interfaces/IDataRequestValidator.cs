using RevenueControl.DomainObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueControl.DomainObjects.Interfaces
{
    public interface IDataRequestValidator
    {
        DataActionAuthorization IsClientActionAuthorized(string userId, string clientId, DataActionType actionType);

        DataActionAuthorization IsDataSourceActionAuthorized(string userId, string dataSourceId, DataActionType actionType);

        DataActionAuthorization IsTransactionActionAuthorized(string userId, string dataSourceId, string transactionId, DataActionType actionType);
    }
}
