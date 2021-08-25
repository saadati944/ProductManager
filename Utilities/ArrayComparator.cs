using System.Collections.Generic;

namespace Utilities
{
    public static class ArrayComparator
    {
        public static bool AreEqual<T>(T[] a, T[] b)
        {
            if (a.Length != b.Length)
                return false;
            for (int i = 0; i < a.Length; i++)
                if (!EqualityComparer<T>.Default.Equals(a[i], b[i]))
                    return false;
            return true;
        }
    }
}
