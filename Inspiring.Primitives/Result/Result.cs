using Inspiring.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Inspiring {
    public abstract class Result : IResult<Result> {
        public static readonly VoidResult Empty = new VoidResult();

        protected readonly ImmutableList<IResultItem> _items;

        public bool HasValue { get; }

        protected Result(bool hasValue, ImmutableList<IResultItem> items = null) {
            _items = items ?? ImmutableList.Create<IResultItem>();
            HasValue = hasValue;
        }

        public VoidResult ToVoid()
            => new VoidResult(_items);

        public Result<T> To<T>()
            => new Result<T>(_items);

        public Result<T> SetTo<T>(T value)
            => new Result<T>(value, _items);

        public IEnumerable<TItem> Get<TItem>() where TItem : IResultItem
            => _items.OfType<TItem>();

        Result IResult<Result>.Add(IResultItem item)
            => this + item;

        protected Result Add(IResultItem item)
            => CreateCopy(_items.Add(item ?? throw new ArgumentNullException(nameof(item))));

        protected abstract Result CreateCopy(ImmutableList<IResultItem> items);

        public static Result<T> Of<T>() => Result<T>.Empty;

        public static Result<T> From<T>(T value) => value;

        public static Result operator +(Result result, IResultItem item)
            => result.Add(item);
    }
}
