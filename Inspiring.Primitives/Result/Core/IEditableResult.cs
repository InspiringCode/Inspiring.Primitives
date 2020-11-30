using System;
using System.Collections.Generic;
using System.Text;

namespace Inspiring.Primitives.Core {
    public interface IEditableResult<TReturnType> : IResult {
        TReturnType Add(IResultItem item);

        TReturnType WithoutItems();

#if NETSTANDARD2_1
        public static TReturnType operator +(IEditableResult<TReturnType> result, IResultItem item)
            => result.Add(item);
#endif
    }
}
