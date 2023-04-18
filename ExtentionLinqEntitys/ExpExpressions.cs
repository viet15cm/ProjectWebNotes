
using System.Linq.Expressions;

namespace ExtentionLinqEntitys
{
    public static class ExpExpressions
    {
        public static Expression<Func<T, object>>[] Extend<T>(params Expression<Func<T, object>>[] items) where T : class
        {
            Expression<Func<T, object>>[] result = new Expression<Func<T, object>>[items.Length];

            for (int i = 0; i < items.Length; i++)
            {
                result[i] = items[i];
            }

            return result;

        }


        public static Func<IQueryable<T>, IQueryable<T>>[] ExtendInclude<T>(params Func<IQueryable<T>, IQueryable<T>>[] items) where T : class
        {
            Func<IQueryable<T>, IQueryable<T>>[] result = new Func<IQueryable<T>, IQueryable<T>>[items.Length];

            for (int i = 0; i < items.Length; i++)
            {
                result[i] = items[i];
            }

            return result;

        }
    }
}
