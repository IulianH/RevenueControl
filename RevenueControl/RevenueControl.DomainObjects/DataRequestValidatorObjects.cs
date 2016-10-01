namespace RevenueControl.DomainObjects
{
    public enum DataActionType
    {
        None = 0,
        Read,
        Insert,
        Update,
        Delete
    }

    public enum DataActionAuthorization
    {
        None = 0,
        Authorized,
        NotAuthorized,
        Invalid
    }
}