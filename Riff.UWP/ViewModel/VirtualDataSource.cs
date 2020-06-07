using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Riff.UWP.ViewModel
{
    public sealed class VirtualDataSource<T> : IList, INotifyCollectionChanged, IItemsRangeInfo
    {
        private ItemCache<T> itemCache;
        private readonly ItemCache<T>.fetchDataCallbackHandler handler;

        public VirtualDataSource(ItemCache<T>.fetchDataCallbackHandler handler)
        {
            this.handler = handler;
        }

        public async Task UpdateCountAsync(Func<int> countFn)
        {
            int count = 0;
            await Task.Run(() => count = countFn());
            if (this.Count != count)
            {
                this.Count = count;
                Reset();
            }
        }

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        public int Count { get; private set; } = 1;

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public object this[int index] { get => itemCache[index]; set => throw new NotImplementedException(); }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void RangesChanged(ItemIndexRange visibleRange, IReadOnlyList<ItemIndexRange> trackedItems)
        {
            Debug.WriteLine($"({visibleRange.FirstIndex},{visibleRange.LastIndex})");
            Debug.WriteLine($"Number of trackedItems = {trackedItems.Count}");
            foreach (var index in trackedItems)
            {
                Debug.WriteLine($"({index.FirstIndex},{index.LastIndex}), Length - {index.Length}");
            }

            //itemCache.UpdateRanges(trackedItems.ToArray());
            
            itemCache.UpdateRanges(new ItemIndexRange[] { new ItemIndexRange(visibleRange.FirstIndex, Math.Min((uint)(Count - visibleRange.FirstIndex), (uint)(visibleRange.Length + 30))) });
        }

        public void Reset()
        {
            // Unhook the old change notification
            if (itemCache != null)
            {
                this.itemCache.CacheChanged -= ItemCache_CacheChanged;
            }

            // Create a new instance of the cache manager
            this.itemCache = new ItemCache<T>(this.handler, 50);
            this.itemCache.CacheChanged += ItemCache_CacheChanged;
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        }

        private void ItemCache_CacheChanged(object sender, CacheChangedEventArgs<T> args)
        {
            CollectionChanged?.Invoke(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, args.oldItem, args.newItem, args.itemIndex));
        }

        public void Dispose()
        {
            itemCache = null;
        }

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            return IndexOf(value) != -1;
        }

        public int IndexOf(object value)
        {
            return (value != null) ? itemCache.IndexOf((T)value) : -1;
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
