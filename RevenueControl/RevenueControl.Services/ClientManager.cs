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

        public Client GetById(string clientName)
        {
            Client client = unitOfWork.ClientRepository.GetById(clientName);
            return client;
        }

        public ActionResponse AddNew(Client client)
        {
            ActionResponse returnValue = new ActionResponse();
            if (ValidateClient(client))
            {
                client.Name = client.Name.Trim();
                Client existing = GetById(client.Name);
                if (existing != null)
                {
                    returnValue.Status = ActionResponseCode.AlreadyExists;
                }
                else
                {
                    unitOfWork.ClientRepository.Insert(client);
                    unitOfWork.Save();
                    returnValue.Status = ActionResponseCode.Success;
                }
            }
            else
            {
                returnValue.Status = ActionResponseCode.InvalidInput; 
            }
            return returnValue;
        }

       public ActionResponse Delete(Client client)
       {
            unitOfWork.ClientRepository.Delete(client);
            unitOfWork.Save();
            return new ActionResponse
            {
                Status = ActionResponseCode.Success
            };
       }

        public bool HasDataSources(Client client)
        {
            DataSource dataSource = unitOfWork.DataSourceRepository.Set.Where(ds => ds.ClientName.ToUpper() == client.Name.ToUpper()).Take(1).SingleOrDefault();
            return dataSource != null; 
        }

        public IList<Client> Get()
        {
            IList<Client> returnValue = null;
            returnValue = unitOfWork.ClientRepository.Set.ToArray();
            return returnValue;
        }

        public ActionResponse Update(Client client)
        {
            ActionResponse returnValue = new ActionResponse();
            if (ValidateClient(client))
            {
                client.Name = client.Name.Trim();
                Client existing = GetById(client.Name);
                if (existing != null)
                {
                    returnValue.Status = ActionResponseCode.AlreadyExists;
                }
                else
                {
                    unitOfWork.ClientRepository.Update(client);
                    unitOfWork.Save();
                    returnValue.Status = ActionResponseCode.Success;
                }
            }
            else
            {
                returnValue.Status = ActionResponseCode.InvalidInput;
            }
            return returnValue;
        }
    }
}
