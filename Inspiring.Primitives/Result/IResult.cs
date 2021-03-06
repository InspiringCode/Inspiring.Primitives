﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Inspiring {
    public interface IResult {
        bool HasValue { get; }

        bool HasErrors { get; }

        IEnumerable<TItem> Get<TItem>() where TItem : IResultItem;
    }
}
