
namespace ExtentionLinqEntitys
{
    public interface IExtendedQuery<T> where T : class
    {
        Func<IQueryable<T>, IQueryable<T>>[] Includes();

        Func<IQueryable<T>, IOrderedQueryable<T>> OrderBy();
        bool DisableTracking();
    }
}
