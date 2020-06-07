using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Riff
{
    static class AsyncEnumerableExtensions
    {
        public const int DefaultConcurrencyDegree = 4;
        public static async Task ForEachAsync<T1, T2>(this IEnumerable<T1> enumerable, T2 item2, Func<T1, T2, Task> fn, int concurrencyDegree = DefaultConcurrencyDegree)
        {
            using (SemaphoreSlim throttler = new SemaphoreSlim(concurrencyDegree, concurrencyDegree))
            {
                IList<Task> tasks = new List<Task>();
                foreach (var item in enumerable)
                {
                    await throttler.WaitAsync();

                    var task = fn.Invoke(item, item2).ContinueWith(t => throttler.Release());
                    tasks.Add(task);
                }

                await Task.WhenAll(tasks);
            }
        }
    }
}
