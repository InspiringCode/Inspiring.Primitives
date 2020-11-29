using Inspiring.Primitives.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inspiring {
    public struct Nothing : IResult, IResultType<Result>, IEquatable<Nothing> {
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

        /****************************** EQUALITY *****************************/

        public bool Equals(Nothing other)
            => true;

        public override bool Equals(object obj)
            => obj is Nothing;

        public override int GetHashCode() {
            HashCode code = new HashCode();
            code.Add(typeof(Nothing));
            return code.ToHashCode();
        }

        /************************** MERGE OPERATORS **************************/

        public static Result operator +(Nothing first, Result second)
            => second;


        /************************** CAST OPERATORS ***************************/

        public static implicit operator Result(Nothing nothing)
            => new Result();
    }
}
