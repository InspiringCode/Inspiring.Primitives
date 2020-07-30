using Inspiring.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Inspiring {
    public class VoidResult : Result, IResult<VoidResult> {
        internal VoidResult(ImmutableList<IResultItem> items = null) : base(false, items) { }

        public Result<T> To<T>() => new Result<T>(_items);

        protected override Result CreateCopy(ImmutableList<IResultItem> items)
            => new VoidResult(items);

        VoidResult IResult<VoidResult>.Add(IResultItem item)
            => this + item;

        public static VoidResult operator +(VoidResult result, IResultItem item)
            => (VoidResult)(result ?? throw new ArgumentNullException(nameof(result))).Add(item);
    }
}
