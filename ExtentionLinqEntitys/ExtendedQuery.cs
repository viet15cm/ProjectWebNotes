
using System.Reflection.Metadata.Ecma335;

namespace ExtentionLinqEntitys
{

    public class ExtendedQuery<T> : IExtendedQuery<T> where T : class
    {

        // không nên dùng vì kèn theo package microsoft entity framework core gây vấn đề về hiệu xuất
        //public Func<IQueryable<T>, IIncludableQueryable<T, object>> Include { get; private set; }

        public Func<IQueryable<T>, IQueryable<T>>[] Includes { get; private set; }

        //thay thế Func<IQueryable<T>, IIncludableQueryable<T, object>> Include { get; private set; }
        public Func<IQueryable<T>, IOrderedQueryable<T>> OrderBy { get; private set; }
        public bool DisableTracking { get; private set; }

        private ExtendedQuery() { }

        public ExtendedQuery(Func<IQueryable<T>, IQueryable<T>>[] includes,
                            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
                            bool disableTracking)
        {
            Includes = includes;
            DisableTracking = disableTracking;
            OrderBy = orderBy;

        }
        public static ExtendedQuery<T> Set(Func<IQueryable<T>, IQueryable<T>>[] includes = null,
                                                     Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                     bool disableTracking = false)
        {
           return new ExtendedQuery<T>(includes, orderBy, disableTracking);
        }

        Func<IQueryable<T>, IQueryable<T>>[] IExtendedQuery<T>.Includes()
        {
            return Includes;
        }

        Func<IQueryable<T>, IOrderedQueryable<T>> IExtendedQuery<T>.OrderBy()
        {
            return OrderBy;
        }

        bool IExtendedQuery<T>.DisableTracking()
        {
            return DisableTracking;
        }

    }
}
