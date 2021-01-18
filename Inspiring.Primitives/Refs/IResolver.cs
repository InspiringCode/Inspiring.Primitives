using System;
using System.Collections.Generic;
using System.Text;

namespace Inspiring.Refs {
    public interface IResolver<in T, out TResult> {
        TResult Resolve<U>(IRef<U> r) where U : T;
    }
}
