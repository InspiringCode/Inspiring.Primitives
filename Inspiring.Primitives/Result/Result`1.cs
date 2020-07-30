using Inspiring.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Inspiring {
    public class Result<T> : Result, IResult<Result<T>> {
        internal static readonly new Result<T> Empty = new Result<T>();
        private readonly T _value;

        public T Value {
            get {
                if (!HasValue)
                    ThrowValueException();

                return _value;
            }
        }

        internal Result(ImmutableList<IResultItem> items)
            : this(false, default, items) { }

        internal Result(T value, ImmutableList<IResultItem> items)
            : this(true, value, items) { }

        private Result(bool hasValue = false, T value = default, ImmutableList<IResultItem> items = null) 
            : base(hasValue, items) => _value = value;

        Result<T> IResult<Result<T>>.Add(IResultItem item)
            => this + item;

        protected override Result CreateCopy(ImmutableList<IResultItem> items)
            => new Result<T>(HasValue, _value, items);

        private void ThrowValueException() {
            throw new InvalidOperationException();
        }

        public static implicit operator Result<T>(T value)
            => new Result<T6;

        public static implicit operator Result<T>(VoidResult result) 
            => (result ?? throw new ArgumentNullException(nameof(result))).To<T>();

        public static implicit operator VoidResult(Result<T> result)
            => result.ToVoid();

        public static Result<T> operator +(Result<T> result, IResultItem item)
            => (Result<T>)(result ?? throw new ArgumentNullException(nameof(result))).Add(item);
    }
}
