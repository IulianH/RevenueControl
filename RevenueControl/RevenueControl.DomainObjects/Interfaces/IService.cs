namespace RevenueControl.DomainObjects.Interfaces
{
    public interface IService<in T>
    {
        ActionResponse Insert(T item);

        ActionResponse Delete(T item);

        ActionResponse Update(T item);

        ActionResponse GetById(params object[] keys);
    }
}