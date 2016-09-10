using RevenueControl.DomainObjects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.DomainObjects;

namespace RevenueControl.Services
{
    public class ClientManager : IClientManager
    {
        IClientRepository _clientRepo;
        public ClientManager(IClientRepository clientRepo)
        {
            _clientRepo = clientRepo;
        }

        public void Dispose()
        {
            if (_clientRepo != null)
            {
                _clientRepo.Dispose();
            }
        }

        public ActionResponse<Client> SearchForClient(string clientName)
        {
            ActionResponse<Client> toReturn = new ActionResponse<Client>();
            toReturn.Result = new Client();
            Client dbClient = _clientRepo.Clients.SingleOrDefault(c => c.Name == clientName);
            if (dbClient != null)
            {
                toReturn.Status = ActionResponseCode.Success;
                toReturn.Result.Name = dbClient.Name;
            }
            else
            {
                toReturn.Status = ActionResponseCode.NotFound;
            }
            return toReturn;
        }
    }
}
