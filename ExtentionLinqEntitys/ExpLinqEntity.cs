
using System.Reflection.Metadata.Ecma335;

namespace ExtentionLinqEntitys
{

    public class ExpLinqEntity<T> : IExpLinqEntity<T> where T : class
    {

        // không nên dùng vì kèn theo package microsoft entity framework core gây vấn đề về hiệu xuất
        //public Func<IQueryable<T>, IIncludableQueryable<T, object>> Include { get; private set; }

        public Func<IQueryable<T>, IQueryable<T>>[] Includes { get; private set; }

        //thay thế Func<IQueryable<T>, IIncludableQueryable<T, object>> Include { get; private set; }
        public Func<IQueryable<T>, IOrderedQueryable<T>> OrderBy { get; private set; }
        public bool DisableTracking { get; private set; }

        private ExpLinqEntity() { }

        public ExpLinqEntity(Func<IQueryable<T>, IQueryable<T>>[] includes,
                            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
                            bool disableTracking)
        {
            Includes = includes;
            DisableTracking = disableTracking;
            OrderBy = orderBy;

        }
        public static ExpLinqEntity<T> ResLinqEntity(Func<IQueryable<T>, IQueryable<T>>[] includes = null,
                                                     Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                     bool disableTracking = false)
        {
           return new ExpLinqEntity<T>(includes, orderBy, disableTracking);
        }

        Func<IQueryable<T>, IQueryable<T>>[] IExpLinqEntity<T>.Includes()
        {
            return Includes;
        }

        Func<IQueryable<T>, IOrderedQueryable<T>> IExpLinqEntity<T>.OrderBy()
        {
            return OrderBy;
        }

        bool IExpLinqEntity<T>.DisableTracking()
        {
            return DisableTracking;
        }

    }
}
