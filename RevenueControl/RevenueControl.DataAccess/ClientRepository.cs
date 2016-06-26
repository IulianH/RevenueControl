using RevenueControl.DomainObjects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevenueControl.DomainObjects.Entities;

namespace RevenueControl.DataAccess
{
    public class ClientRepository : IClientRepository
    {
        RevenueControlDb _db = new RevenueControlDb();

        public IEnumerable<Client> Clients
        {
            get
            {
                return _db.Clients;
            }
        }

        public void Dispose()
        {
            if(_db != null)
            {
                _db.Dispose();
            }
        }
    }
}
