using RevenueControl.DomainObjects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevenueControl.DomainObjects;
using RevenueControl.DomainObjects.Entities;
using System.Globalization;

namespace RevenueControl.Services
{
    public class DataSourceManager : IDataSourceManager
    {
        IUnitOfWork unitOfWork;

        public DataSourceManager(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        private bool ValidateDataSource(DataSource dataSource)
        {
            bool returnValue;

            if(!string.IsNullOrWhiteSpace(dataSource.BankAccount) && !string.IsNullOrWhiteSpace(dataSource.Culture) && !string.IsNullOrWhiteSpace(dataSource.ClientName))
            {
                try
                {
                    CultureInfo culture = CultureInfo.CreateSpecificCulture(dataSource.Culture);
                    returnValue = true;
                }
                catch (Exception)
                {
                    returnValue = false;
                }
            }
            else
            {
                returnValue = false;
            }
            return returnValue;
        }

        public ActionResponse Insert(DataSource dataSource)
        {
            ActionResponse returnValue = new ActionResponse();
            if(ValidateDataSource(dataSource))
            {
                dataSource.BankAccount = dataSource.BankAccount.Trim();
                dataSource.Culture = dataSource.Culture.Trim();
                if(!string.IsNullOrWhiteSpace(dataSource.Name))
                {
                    dataSource.Name = dataSource.Name.Trim();
                }
                if(unitOfWork.DataSourceRepository.Set.Where(ds => ds.ClientName == dataSource.ClientName && ds.BankAccount.ToUpper() == dataSource.BankAccount.ToUpper()).SingleOrDefault() == null)
                {
                    unitOfWork.DataSourceRepository.Insert(dataSource);
                    unitOfWork.Save();
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
           unitOfWork.Dispose();
        }

        public IList<DataSource> Get(string clientName, string searchTerm = null)
        {
            IList<DataSource> toReturn;
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                toReturn = unitOfWork.DataSourceRepository.Set.Where(ds => ds.ClientName == clientName).ToArray();
            }
            else
            {
                string toSearch = searchTerm.Trim();
                toReturn = unitOfWork.DataSourceRepository.Set
                    .Where(ds => ds.ClientName == clientName && (ds.BankAccount.Contains(toSearch) || (ds.Name != null && ds.Name.Contains(toSearch)))).ToArray();
            }
            return toReturn;
        }

        public bool HasTransactions(DataSource dataSource)
        {
            Transaction transaction = unitOfWork.TransactionRepository.Set.Where(tr => tr.DataSourceId == dataSource.Id).Take(1).SingleOrDefault();
            return transaction != null;
        }

        public DataSource GetById(int id, string clientName)
        {
            return unitOfWork.DataSourceRepository.GetById(id);
        }

        public ActionResponse Delete(DataSource dataSource)
        {
            unitOfWork.DataSourceRepository.Delete(dataSource);
            unitOfWork.Save();
            return new ActionResponse
            {
                Status = ActionResponseCode.Success
            };

        }

        public ActionResponse Update(DataSource dataSource)
        {
            unitOfWork.DataSourceRepository.Update(dataSource);
            unitOfWork.Save();
            return new ActionResponse
            {
                Status = ActionResponseCode.Success
            };
        }
    }
}
