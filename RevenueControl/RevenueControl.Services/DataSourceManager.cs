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
        IRepository<DataSource> _dsRepo;

        public DataSourceManager(IRepository<DataSource> dsRepo)
        {
            _dsRepo = dsRepo;
        }

        private bool ValidateDataSource(DataSource dataSource)
        {
            bool returnValue;
            if(!string.IsNullOrWhiteSpace(dataSource.BankAccount) && !string.IsNullOrWhiteSpace(dataSource.Culture))
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

        public ActionResponse<DataSource> CreateDataSource(DataSource dataSource)
        {
            ActionResponse<DataSource> returnValue = new ActionResponse<DataSource>();
            if(ValidateDataSource(dataSource))
            {
                dataSource.BankAccount = dataSource.BankAccount.Trim();
                dataSource.Culture = dataSource.Culture.Trim();
                if(!string.IsNullOrWhiteSpace(dataSource.Name))
                {
                    dataSource.Name = dataSource.Name.Trim();
                }
                if(_dsRepo.SearchFor(ds => ds.BankAccount.ToUpper() == dataSource.BankAccount.ToUpper()).SingleOrDefault() == null)
                {
                    _dsRepo.Insert(dataSource);
                    returnValue.Status = ActionResponseCode.Success;
                    returnValue.Result = dataSource;
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
            if (_dsRepo != null)
            {
                _dsRepo.Dispose();
            }
        }

        public ActionResponse<DataSource> GetClientDataSources(Client client, string searchTerm = null)
        {
            IList<DataSource> toReturn;
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                toReturn = _dsRepo.SearchFor(ds => ds.ClientName == client.Name).ToArray();
            }
            else
            {
                string toSearch = searchTerm.Trim();
                toReturn = _dsRepo.SearchFor(ds => ds.ClientName == client.Name && (ds.BankAccount.Contains(toSearch) || (ds.Name != null && ds.Name.Contains(toSearch)))).ToArray();
            }
            return new ActionResponse<DataSource>
            {
                ResultList = toReturn,
                Status = ActionResponseCode.Success
            };

        }
    }
}
