﻿using Inspiring.Core;
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

        protected Result(bool hasValue, ImmutableList<IResultItem>? items = null) {
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

        public Result WithoutItems()
            => InvokeWithoutItems();

        Result IResult<Result>.Add(IResultItem item)
            => this + item;

        protected bool ItemsEqualToItemsOf(Result other)
            => _items.SequenceEqual(other._items);

        protected HashCode GetHashcodeOfItems() {
            HashCode code = new HashCode();
            foreach (IResultItem item in _items)
                code.Add(item);
            return code;
        }

        protected Result Add(IResultItem item)
            => CreateCopy(_items.Add(item ?? throw new ArgumentNullException(nameof(item))));

        protected abstract Result CreateCopy(ImmutableList<IResultItem> items);

        protected abstract Result InvokeWithoutItems();

        public static Result<T> Of<T>() => Result<T>.Empty;

        public static Result<T> From<T>(T value) => value;

        public static Result operator +(Result result, IResultItem item)
            => result.Add(item);

        public static bool operator ==(Result x, object y)
            => Equals(x, y);

        public static bool operator !=(Result x, object y)
            => !Equals(x, y);
    }
}
