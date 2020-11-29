using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inspiring {
    public static class EnumerableExtensions {
        public static Result Combine(this IEnumerable<Result> results) => results
            .MustNotBeNull(nameof(results))
            .Aggregate((agg, next) => agg + next);

        public static Result<T> Combine<T>(this IEnumerable<Result<T>> results) => results
            .MustNotBeNull(nameof(results))
            .Aggregate((agg, next) => agg + next);
    }
}
