using System;
using System.Collections.Generic;
using System.Text;

namespace Inspiring.Refs {
    public interface IRef<out T> : IRef {
        TResult Resolve<TResult>(IResolver<T, TResult> r)
#if NETSTANDARD2_1
            => r.Resolve(this);
#else
            ;
#endif
    }
}
