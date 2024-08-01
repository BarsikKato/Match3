using System.Collections;

namespace Extensions
{
    public static class IEnumeratorExtension
    {
        public static void FullyIterate(this IEnumerator enumerator)
        {
            while (enumerator.MoveNext()) { }
        }
    }
}

