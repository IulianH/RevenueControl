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
    public class ClientManger : IClientManager
    {
        IClientRepository _clientRepo;
        public ClientManger(IClientRepository clientRepo)
        {
            _clientRepo = clientRepo;
        }
        public ActionResponse<Client> Client
        {
            get
            {
                ActionResponse<Client> toReturn = new ActionResponse<Client>();
                toReturn.Result = new Client();
                Client dbClient = _clientRepo.Clients.Single(c => c.Name == "DefaultClient");
                toReturn.Status = ActionResponseCode.Success;
                toReturn.Result.Name = dbClient.Name;
                return toReturn;
            }
        }

        public void Dispose()
        {
            _clientRepo.Dispose();
        }
    }
}
