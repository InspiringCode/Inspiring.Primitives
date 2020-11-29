using Inspiring.Primitives.Core;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Inspiring {
    public struct Nothing : IResult, IResultType<Result> {
        public bool HasValue => false;

        public bool HasErrors => false;

        public IEnumerable<TItem> Get<TItem>() where TItem : IResultItem
            => Enumerable.Empty<TItem>();

        public Result Add(IResultItem item)
            => new Result().Add(item);

        public Result WithoutItems()
            => new Result();

        public override string ToString()
            => "Nothing";

        /************************** MERGE OPERATORS **************************/

        public static Result operator +(Nothing first, Result second)
            => second;

        
        /************************** CAST OPERATORS ***************************/

        public static implicit operator Result(Nothing nothing)
            => new Result();
    }
}
