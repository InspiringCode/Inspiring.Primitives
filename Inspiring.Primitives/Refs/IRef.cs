using System;
using System.Collections.Generic;
using System.Text;

namespace Inspiring.Refs {
    public interface IRef {
#if NETSTANDARD2_1
        IRef<object> AsObjectRef() =>
            this as IRef<object> ??
            throw new InvalidOperationException(
                "Cannot cast the ref '{this}' because it does not implement 'IRef<T>' for any type 'T'. You should " +
                "never implement 'IRef' directly but implement 'IRef<T>' instead and use 'object' for 'T' if it is " +
                "an untyped ref implementation.");
#endif
    }
}
