using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OnePlayer
{
    static class AsyncEnumerableExtensions
    {
        public const int DefaultConcurrencyDegree = 4;
        public static async Task ForEachAsync<T>(this IEnumerable<T> enumerable, Func<T, Task> fn, int concurrencyDegree = DefaultConcurrencyDegree)
        {
            using (SemaphoreSlim throttler = new SemaphoreSlim(concurrencyDegree, concurrencyDegree))
            {
                IList<Task> tasks = new List<Task>();
                foreach (var item in enumerable)
                {
                    await throttler.WaitAsync();

                    var task = fn.Invoke(item).ContinueWith(t => throttler.Release());
                    tasks.Add(task);
                }

                await Task.WhenAll(tasks);
            }
        }
    }
}
