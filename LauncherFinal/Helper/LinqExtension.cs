using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LauncherFinal.Helper
{
    public static class LinqExtension
    {
        public static bool IsTermwiseEquals<T>(this IEnumerable<T> orignal, IEnumerable<T> sequence, IEqualityComparer<T> comparer = null)
        {
            var enumerable = orignal.ToList();
            var second = sequence.ToList();

            if (enumerable.IsNullOrEmpty() && second.IsNullOrEmpty())
                return true;

            if (enumerable.IsNullOrEmpty() || second.IsNullOrEmpty())
                return true;

            if (enumerable.Count != second.Count)
            {
                return false;
            }

            if (comparer == null)
                comparer = EqualityComparer<T>.Default;

            var any = enumerable.Except(second, comparer);
            return !any.Any();
        }

        public static IList<T> SelectRecursive<T>(T source, Func<T, IEnumerable<T>> search)
        {
            if (source == null || search == null)
                return new List<T>();

            var result = new List<T>();

            // копируем, так как результат вычисления может быть отложенным
            var children = search(source).ToList();

            if (!children.IsNullOrEmpty())
            {
                result.AddRange(children);

                foreach (var child in children)
                {
                    var childrenOfChildren = SelectRecursive(child, search);

                    if (!childrenOfChildren.IsNullOrEmpty())
                        result.AddRange(childrenOfChildren);
                }
            }

            return result;
        }

        public static bool IsNullOrEmpty(this IEnumerable sequence)
        {
            return sequence == null || !sequence.GetEnumerator().MoveNext();
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source.IsNullOrEmpty() || action == null)
                return;

            foreach (var item in source)
            {
                action(item);
            }
        }
    }
}
