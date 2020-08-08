using Inspiring.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspiring {
    public interface IResult {
        bool HasValue { get; }

        IEnumerable<TItem> Get<TItem>() where TItem : IResultItem;
    }
}
