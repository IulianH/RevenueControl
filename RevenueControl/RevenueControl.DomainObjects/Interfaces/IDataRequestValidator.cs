namespace RevenueControl.DomainObjects.Interfaces
{
    public interface IDataRequestValidator
    {
        DataActionAuthorization IsClientActionAuthorized(string userId, string clientId, DataActionType actionType);

        DataActionAuthorization IsDataSourceActionAuthorized(string userId, string dataSourceId,
            DataActionType actionType);

        DataActionAuthorization IsTransactionActionAuthorized(string userId, string dataSourceId, string transactionId,
            DataActionType actionType);
    }
}