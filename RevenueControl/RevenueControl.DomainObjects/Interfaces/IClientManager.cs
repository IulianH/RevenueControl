using System;
using System.Collections.Generic;
using RevenueControl.DomainObjects.Entities;

namespace RevenueControl.DomainObjects.Interfaces
{
    public interface IClientManager : IDisposable
    {
        IList<Client> Get();

        Client GetById(string clientName);

        ActionResponse AddNew(Client client);

        ActionResponse Delete(Client client);

        bool HasDataSources(Client client);

        ActionResponse Update(Client client);
    }
}