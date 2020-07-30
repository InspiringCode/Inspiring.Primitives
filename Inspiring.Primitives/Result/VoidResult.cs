﻿using Inspiring.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Inspiring {
    public class VoidResult : Result, IResult<VoidResult> {
        internal VoidResult(ImmutableList<IResultItem>? items = null) : base(false, items) { }

        public new VoidResult WithoutItems()
            => new VoidResult();

        public override bool Equals(object obj)
            => obj is VoidResult r && ItemsEqualToItemsOf(r);

        public override int GetHashCode() {
            HashCode code = GetHashcodeOfItems();
            code.Add(typeof(VoidResult));
            return code.ToHashCode();
        }

        public override string ToString() {
            string items = base.ToString();
            return items != "" ? items : "<void>";
        }

        protected override Result CreateCopy(ImmutableList<IResultItem> items)
            => new VoidResult(items);

        protected override Result InvokeWithoutItems()
            => Result.Empty;

        VoidResult IResult<VoidResult>.Add(IResultItem item)
            => this + item;

        public static VoidResult operator +(VoidResult result, IResultItem item)
            => (VoidResult)(result ?? throw new ArgumentNullException(nameof(result))).Add(item);
    }
}