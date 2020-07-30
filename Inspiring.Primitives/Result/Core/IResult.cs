using System;
using System.Collections.Generic;
using System.Text;

namespace Inspiring.Core {
    public interface IResult<T> where T : IResult<T> {
        bool HasValue { get; }

        T Add(IResultItem item);

        T WithoutItems();
    }
}
