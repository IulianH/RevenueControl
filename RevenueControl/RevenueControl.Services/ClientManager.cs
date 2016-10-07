using System.Collections.Generic;
using System.Linq;
using RevenueControl.DomainObjects;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.DomainObjects.Interfaces;

namespace RevenueControl.Services
{
    public class ClientManager : IClientManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClientManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }

        public Client GetById(string clientName)
        {
            var client = _unitOfWork.ClientRepository.GetById(clientName);
            return client;
        }

        public ActionResponse AddNew(Client client)
        {
            var returnValue = new ActionResponse();
            if (ValidateClient(client))
            {
                client.Name = client.Name.Trim();
                var existing = GetById(client.Name);
                if (existing != null)
                {
                    returnValue.Status = ActionResponseCode.AlreadyExists;
                }
                else
                {
                    _unitOfWork.ClientRepository.Insert(client);
                    _unitOfWork.Save();
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
            _unitOfWork.ClientRepository.Delete(client);
            _unitOfWork.Save();
            return new ActionResponse
            {
                Status = ActionResponseCode.Success
            };
        }

        public bool HasDataSources(Client client)
        {
            var dataSource =
                _unitOfWork.DataSourceRepository.Set.Where(ds => ds.ClientName.ToUpper() == client.Name.ToUpper())
                    .Take(1)
                    .SingleOrDefault();
            return dataSource != null;
        }

        public IList<Client> Get()
        {
            IList<Client> returnValue = null;
            returnValue = _unitOfWork.ClientRepository.Set.ToArray();
            return returnValue;
        }

        public ActionResponse Update(Client client)
        {
            var returnValue = new ActionResponse();
            if (ValidateClient(client))
            {
                client.Name = client.Name.Trim();
                var existing = GetById(client.Name);
                if (existing != null)
                {
                    returnValue.Status = ActionResponseCode.AlreadyExists;
                }
                else
                {
                    _unitOfWork.ClientRepository.Update(client);
                    _unitOfWork.Save();
                    returnValue.Status = ActionResponseCode.Success;
                }
            }
            else
            {
                returnValue.Status = ActionResponseCode.InvalidInput;
            }
            return returnValue;
        }

        private static bool ValidateClient(Client client)
        {
            var returnValue = !string.IsNullOrWhiteSpace(client.Name) &&
                              client.Name.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c));
            return returnValue;
        }
    }
}