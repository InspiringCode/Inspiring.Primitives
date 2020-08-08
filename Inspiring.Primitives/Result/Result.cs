#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()

using Inspiring.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Inspiring {
    public static class Result {
        public static readonly VoidResult Empty = new VoidResult();


        public static Result<T> Of<T>() => default;

        public static Result<T> From<T>(T value) => value;
    }

    //public abstract class ResultBase : IResultType<ResultBase>, IResult {
    //    public static readonly VoidResult Empty = new VoidResult();

    //    protected readonly ImmutableList<IResultItem> _items;

    //    public bool HasValue { get; }

    //    protected ResultBase(bool hasValue, ImmutableList<IResultItem>? items = null) {
    //        _items = items ?? ImmutableList.Create<IResultItem>();
    //        HasValue = hasValue;
    //    }

    //    public VoidResult ToVoid()
    //        => new VoidResult(_items);

    //    public Result<T> To<T>()
    //        => new Result<T>(_items);

    //    public Result<T> SetTo<T>(T value)
    //        => new Result<T>(value, _items);

    //    public IResult Add(IResultItem item)
    //        => CreateCopy(_items.Add(item.MustNotBeNull(nameof(item))));

    //    public IEnumerable<TItem> Get<TItem>() where TItem : IResultItem
    //        => _items.OfType<TItem>();

    //    public IResult WithoutItems()
    //        => InvokeWithoutItems();

    //    public override string ToString() => _items.Count switch
    //    {
    //        0 => "",
    //        1 => $"{_items.First()}",
    //        _ => $"{_items.Last()} (and {_items.Count - 1} more items)"
    //    };

    //    protected bool ItemsEqualToItemsOf(ResultBase other)
    //        => _items.SequenceEqual(other._items);

    //    protected HashCode GetHashcodeOfItems() {
    //        HashCode code = new HashCode();
    //        foreach (IResultItem item in _items)
    //            code.Add(item);
    //        return code;
    //    }

    //    protected abstract ResultBase CreateCopy(ImmutableList<IResultItem> items);

    //    protected abstract ResultBase InvokeWithoutItems();



    //    protected static ResultBase Merge(ResultBase first, ResultBase second) {
    //        first.MustNotBeNull(nameof(first));
    //        second.MustNotBeNull(nameof(second));

    //        bool preferFirstAsTemplate =
    //            second is VoidResult ||
    //            (!second.HasValue && first.HasValue);

    //        ResultBase template = preferFirstAsTemplate ? first : second;
    //        return template.CreateCopy(first._items.AddRange(second._items));
    //    }

    //    ResultBase IResultType<ResultBase>.Add(IResultItem item) {
    //        throw new NotImplementedException();
    //    }

    //    ResultBase IResultType<ResultBase>.WithoutItems() {
    //        throw new NotImplementedException();
    //    }
    //}
}
