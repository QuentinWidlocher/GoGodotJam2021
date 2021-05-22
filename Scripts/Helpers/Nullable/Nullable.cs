using System;
using System.Diagnostics.CodeAnalysis;

namespace Helpers
{
    public static class Nullable
    {
        public static void AssertNotNull<T>([NotNull] T? x) where T : class
        {
            if (x == null) throw new Exception();
        }
        
        public static void AssertNotNull<T>([NotNull] T? x) where T : struct
        {
            if (x == null) throw new Exception();
        }

        public static T ReturnIfNotNull<T>([NotNull] T? x) where T : class
        {
            AssertNotNull(x);
            return x;
        }

        public static bool IsNullOrEmpty(this string instance)
        {
            return string.IsNullOrEmpty(instance);
        }
    }
}