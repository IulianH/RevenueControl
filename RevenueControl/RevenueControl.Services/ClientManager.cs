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
        IUnitOfWork unitOfWork;
        public ClientManager(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public void Dispose()
        {
            unitOfWork.Dispose();
        }

        bool ValidateClient(Client client)
        {
            bool returnValue;
            if(!string.IsNullOrWhiteSpace(client.Name))
            {
                returnValue = client.Name.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c));

            }
            else
            {
                returnValue = false;
            }
            return returnValue;
        }

        public ActionResponse<Client> SearchForClient(string clientName)
        {
            ActionResponse<Client> toReturn = new ActionResponse<Client>();
            toReturn.Result = new Client();

            Client dbClient = unitOfWork.ClientRepository.Get(client => client.Name.ToUpper() == clientName.ToUpper()).SingleOrDefault();
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

        public ActionResponse<Client> AddNewClient(Client client)
        {
            ActionResponse<Client> returnValue = new ActionResponse<Client>();
            if (ValidateClient(client))
            {
                client.Name = client.Name.Trim();
                ActionResponse<Client> result = SearchForClient(client.Name);
                if (result.Status == ActionResponseCode.Success)
                {
                    returnValue.Status = ActionResponseCode.AlreadyExists;
                }
                else
                {
                    unitOfWork.ClientRepository.Insert(client);
                    unitOfWork.Save();
                    returnValue.Status = ActionResponseCode.Success;
                    returnValue.Result = client;
                }
            }
            else
            {
                returnValue.Status = ActionResponseCode.InvalidInput; 
            }
            return returnValue;
        }

       public void DeleteClient(Client client)
       {
            unitOfWork.ClientRepository.Delete(client);
            unitOfWork.Save();
       }

        public bool HasDataSources(Client client)
        {
            IList<DataSource> dataSources = unitOfWork.DataSourceRepository.Get(ds => ds.ClientName.ToUpper() == client.Name.ToUpper(), null, null, 1).ToList();
            return dataSources.Count > 0;
        }
    }
}
