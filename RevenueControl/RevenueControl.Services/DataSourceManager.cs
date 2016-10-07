using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RevenueControl.DomainObjects;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.DomainObjects.Interfaces;

namespace RevenueControl.Services
{
    public class DataSourceManager : IDataSourceManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public DataSourceManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResponse Insert(DataSource dataSource)
        {
            var returnValue = new ActionResponse();
            if (ValidateDataSource(dataSource))
            {
                dataSource.BankAccount = dataSource.BankAccount.Trim();
                dataSource.Culture = dataSource.Culture.Trim();
                if (!string.IsNullOrWhiteSpace(dataSource.Name))
                    dataSource.Name = dataSource.Name.Trim();
                if (
                    _unitOfWork.DataSourceRepository.Set.SingleOrDefault(
                        ds => (ds.ClientName == dataSource.ClientName) &&
                              (ds.BankAccount.ToUpper() == dataSource.BankAccount.ToUpper())) == null)
                {
                    _unitOfWork.DataSourceRepository.Insert(dataSource);
                    _unitOfWork.Save();
                    returnValue.Status = ActionResponseCode.Success;
                }
                else
                {
                    returnValue.Status = ActionResponseCode.AlreadyExists;
                }
            }
            else
            {
                returnValue.Status = ActionResponseCode.InvalidInput;
            }
            return returnValue;
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }

        public IList<DataSource> Get(string clientName, string searchTerm = null)
        {
            IList<DataSource> toReturn;
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                toReturn = _unitOfWork.DataSourceRepository.Set.Where(ds => ds.ClientName == clientName).ToArray();
            }
            else
            {
                var toSearch = searchTerm.Trim();
                toReturn = _unitOfWork.DataSourceRepository.Set
                    .Where(
                        ds =>
                            (ds.ClientName == clientName) &&
                            (ds.BankAccount.Contains(toSearch) || ((ds.Name != null) && ds.Name.Contains(toSearch))))
                    .ToArray();
            }
            return toReturn;
        }

        public bool HasTransactions(DataSource dataSource)
        {
            var transaction =
                _unitOfWork.TransactionRepository.Set.Where(tr => tr.DataSourceId == dataSource.Id)
                    .Take(1)
                    .SingleOrDefault();
            return transaction != null;
        }

        public DataSource GetById(int id, string clientName)
        {
            return _unitOfWork.DataSourceRepository.GetById(id);
        }

        public ActionResponse Delete(DataSource dataSource)
        {
            _unitOfWork.DataSourceRepository.Delete(dataSource);
            _unitOfWork.Save();
            return new ActionResponse
            {
                Status = ActionResponseCode.Success
            };
        }

        public ActionResponse Update(DataSource dataSource)
        {
            _unitOfWork.DataSourceRepository.Update(dataSource);
            _unitOfWork.Save();
            return new ActionResponse
            {
                Status = ActionResponseCode.Success
            };
        }

        private static bool ValidateDataSource(DataSource dataSource)
        {
            bool returnValue;

            if (!string.IsNullOrWhiteSpace(dataSource.BankAccount) && !string.IsNullOrWhiteSpace(dataSource.Culture) &&
                !string.IsNullOrWhiteSpace(dataSource.ClientName))
                try
                {
                    var culture = CultureInfo.CreateSpecificCulture(dataSource.Culture);
                    returnValue = true;
                }
                catch (Exception)
                {
                    returnValue = false;
                }
            else
                returnValue = false;
            return returnValue;
        }
    }
}