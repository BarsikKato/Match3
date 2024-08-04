using System.Collections;

namespace Match3.Extensions
{
    public static class IEnumeratorExtension
    {
        public static void FullyIterate(this IEnumerator enumerator)
        {
            while (enumerator.MoveNext()) { }
        }
    }
}

