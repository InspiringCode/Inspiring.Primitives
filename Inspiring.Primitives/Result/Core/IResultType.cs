using System;
using System.Collections.Generic;
using System.Text;

namespace Inspiring.Core {
    public interface IResultType<TResultType> : IResult {
        TResultType Add(IResultItem item);

        TResultType WithoutItems();

#if NETSTANDARD2_1
        public static TResultType operator +(IResultType<TResultType> result, IResultItem item)
            => result.Add(item);
#endif
    }
}
