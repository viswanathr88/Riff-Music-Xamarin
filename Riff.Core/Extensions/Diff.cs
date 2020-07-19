using System;
using System.Collections.Generic;

namespace Riff.UWP.UI.Extensions
{
    public enum DiffAction
    {
        Add,
        Remove,
        Equal
    };

    public sealed class DiffListItem<T>
    {
        public DiffAction Action { get; set; }

        public T SourceItem { get; set; }

        public T TargetItem { get; set; }
    }

    public sealed class DiffList<T> : List<DiffListItem<T>>
    {

    }

    public static class Diff
    {
        public static DiffList<T> Compare<T>(IList<T> source, IList<T> target)
        {
            return Compare(source, target, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Compare two lists using LCS algorithm
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">Source list</param>
        /// <param name="target">Target list</param>
        /// <param name="comparer">Equality comparer</param>
        /// <returns></returns>
        public static DiffList<T> Compare<T>(IList<T> source, IList<T> target, IEqualityComparer<T> comparer)
        {
            var diffList = new DiffList<T>();

            if (source == target)
            {
                return diffList;
            }

            if (source == null || target == null)
            {
                throw new ArgumentNullException();
            }

            var sourceCount = source.Count;
            var targetCount = target.Count;

            // Allocate the LCS matrix
            // They are all automatically initialized to zero
            var matrix = new int[sourceCount + 1, targetCount + 1];

            // Build the LCS matrix
            int i,j = 0;
            for (i = 1; i < sourceCount + 1; i++)
            {
                for (j = 1; j < targetCount + 1; j++)
                {
                    if (comparer.Equals(source[i - 1], target[j - 1]))
                    {
                        matrix[i, j] = matrix[i - 1, j - 1] + 1;
                    }
                    else
                    {
                        matrix[i, j] = Math.Max(matrix[i - 1, j], matrix[i, j - 1]);
                    }
                }
            }

            // Generate actions
            i = sourceCount; 
            j = targetCount;

            while (i > 0 || j > 0)
            {
                if (i > 0 && j > 0 && comparer.Equals(source[i - 1], target[j - 1]))
                {
                    diffList.Add(new DiffListItem<T>()
                    {
                        Action = DiffAction.Equal,
                        SourceItem = source[i - 1],
                        TargetItem = target[j - 1]
                    });
                    i--;
                    j--;
                }
                else
                {
                    if (j > 0 && (i == 0 || matrix[i, j - 1] >= matrix[i, j - 1]))
                    {
                        diffList.Add(new DiffListItem<T>()
                        {
                            Action = DiffAction.Add,
                            SourceItem = default,
                            TargetItem = target[j - 1]
                        });
                        j--;
                    }
                    else if (i > 0 && (j == 0 || matrix[i,j - 1] < matrix[i - 1, j]))
                    {
                        diffList.Add(new DiffListItem<T>()
                        {
                            Action = DiffAction.Remove,
                            SourceItem = source[i - 1],
                            TargetItem = default
                        });
                        i--;
                    }
                }
            }

            diffList.Reverse();
            return diffList;
        }

        public static void ApplyDiff<T>(this IList<T> list, DiffList<T> diffs)
        {
            int index = 0;
            foreach (var diff in diffs)
            {
                if (diff.Action == DiffAction.Add)
                {
                    list.Insert(index, diff.TargetItem);
                    index++;
                }
                else if (diff.Action == DiffAction.Remove)
                {
                    list.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
        }

        public static void ApplyDiff<TDest, TSource>(this IList<TDest> list, DiffList<TSource> diffs, Func<TSource, TDest> createDestFn)
        {
            int index = 0;
            foreach (var diff in diffs)
            {
                if (diff.Action == DiffAction.Add)
                {
                    list.Insert(index, createDestFn(diff.TargetItem));
                    index++;
                }
                else if (diff.Action == DiffAction.Remove)
                {
                    list.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
        }

        public static void TransformInfo<T>(this IList<T> source, IList<T> target)
        {
            TransformInfo(source, target, EqualityComparer<T>.Default);
        }

        public static void TransformInfo<T>(this IList<T> source, IList<T> target, IEqualityComparer<T> comparer)
        {
            var diffList = Compare<T>(source, target, comparer);
            ApplyDiff(source, diffList);
        }
    }
}
