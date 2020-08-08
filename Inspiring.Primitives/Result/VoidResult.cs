using Inspiring.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Inspiring {
    public readonly struct VoidResult : IResult, IResultType<VoidResult> {
        internal readonly ImmutableList<IResultItem>? _items;

        public bool HasValue => false;

        internal ImmutableList<IResultItem> Items
            => _items ?? ImmutableList.Create<IResultItem>();

        internal VoidResult(ImmutableList<IResultItem>? items)
            => _items = items;

        public VoidResult Add(IResultItem item)
            => new VoidResult(Items.Add(item));

        public VoidResult WithoutItems()
            => new VoidResult();

        public IEnumerable<TItem> Get<TItem>() where TItem : IResultItem
            => Items.OfType<TItem>();

        public Result<T> To<T>()
            => new Result<T>(_items);

        public Result<T> SetTo<T>(T value)
            => new Result<T>(value, _items);

        public override bool Equals(object obj)
            => obj is VoidResult r && Items.SequenceEqual(r.Items);

        public override int GetHashCode() {
            HashCode code = Utils.GetHashcodeOfItems(_items);
            code.Add(typeof(VoidResult));
            return code.ToHashCode();
        }

        public override string ToString() {
            string items = Utils.FormatItemsShort(_items);
            return items != "" ? items : "<void>";
        }

        public static implicit operator VoidResult(ResultItem item)
            => Result.Empty.Add(item);

        //public static VoidResult operator +(VoidResult result, IResultItem item)
        //    => result.Add(item);

        public static VoidResult operator +(VoidResult first, VoidResult second)
            => new VoidResult(Utils.Combine(first._items, second._items));
    }
}
