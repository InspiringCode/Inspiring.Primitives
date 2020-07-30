using System;
using System.Collections.Generic;
using System.Text;

namespace Inspiring.Core {
    public interface IResult<TResultType> where TResultType : IResult<TResultType> {
        bool HasValue { get; }

        Result<T> SetTo<T>(T value);

        Result<T> To<T>();

        VoidResult ToVoid();

        TResultType Add(IResultItem item);

        IEnumerable<TItem> Get<TItem>() where TItem : IResultItem;

        TResultType WithoutItems();
    }
}
