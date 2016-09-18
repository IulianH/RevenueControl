using RevenueControl.DomainObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueControl.DomainObjects.Interfaces
{
    public interface IClientManager : IDisposable
    {
        ActionResponse<Client> SearchForClient(string clientName);

        ActionResponse<Client> AddNewClient(Client client);

        void DeleteClient(Client client);

        bool HasDataSources(Client client);
    }
}
