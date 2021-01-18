#pragma warning disable CS8633 // Nullability in constraints for type parameter doesn't match the constraints for type parameter in implicitly implemented interface method'.

using System;
using System.Collections.Generic;
using System.Text;

namespace Inspiring.Refs {
    public static class RefExtensions {
        public static Type GetTargetType<T>(this IRef<T> r)
            => r.MustNotBeNull(nameof(r)).Resolve(RefTypeResolver<T>.Instance);

        private class RefTypeResolver<T> : IResolver<T, Type> {
            public static readonly RefTypeResolver<T> Instance = new();

            public Type Resolve<U>(IRef<U> r) where U : T
                => typeof(U);
        }
    }
}
