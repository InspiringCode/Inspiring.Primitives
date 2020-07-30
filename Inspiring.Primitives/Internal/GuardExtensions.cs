using System;
using System.Runtime.CompilerServices;

namespace Inspiring {
    internal static class GuardExtensions {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T MustNotBeNull<T>(this T? parameter, string? parameterName = null, string? message = null) where T : class {
            if (parameter == null)
                ThrowArgumentNull(parameterName, message);
            return parameter!;
        }

        public static void ThrowArgumentNull(string? parameterName = null, string? message = null) =>
            throw new ArgumentNullException(parameterName, message ?? $"{parameterName ?? "The value"} must not be null.");
    }
}
