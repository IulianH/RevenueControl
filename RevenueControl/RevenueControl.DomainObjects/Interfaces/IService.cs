using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RevenueControl.DomainObjects.Interfaces
{
    public interface IService<T>
    {
        ActionResponse Insert(T item);

        ActionResponse Delete(T item);

        ActionResponse Update(T item);

        ActionResponse GetById(params object[] keys);
    }
}
