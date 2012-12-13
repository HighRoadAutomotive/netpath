using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace System.Collections.Generic
{
	public class DependencyList<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		private readonly List<T> il;
		private readonly ReaderWriterLockSlim ocl;
		private readonly Action<IEnumerable<T>> Added;
		private readonly Action<int, IEnumerable<T>> Inserted;
		private readonly Action<int, int, T> Moved;
		private readonly Action<IEnumerable<T>> Removed;
		private readonly Action<int, IEnumerable<T>> RemovedAt;
		private readonly Action<int> Cleared;
		private readonly Action<int, T, T> Replaced;

		~DependencyList()
		{
			try
			{
				ocl.Dispose();
			}
			catch { }
		}

		public DependencyList()
		{
			il = new List<T>();
			ocl = new ReaderWriterLockSlim();
			Added = (items => { });
			Inserted = ((index, items) => { });
			Moved = ((oldindex, newindex, item) => { });
			Removed = (items => { });
			RemovedAt = ((index, items) => { });
			Cleared = (count => { });
			Replaced = ((index, olditem, newitem) => { });
		}

		public DependencyList(int Capacity)
		{
			il = new List<T>(Capacity);
			ocl = new ReaderWriterLockSlim();
			Added = (items => { });
			Inserted = ((index, items) => { });
			Moved = ((oldindex, newindex, item) => { });
			Removed = (items => { });
			RemovedAt = ((index, items) => { });
			Cleared = (count => { });
			Replaced = ((index, olditem, newitem) => { });
		}

		public DependencyList(IEnumerable<T> Items)
		{
			il = new List<T>(Items);
			ocl = new ReaderWriterLockSlim();
			Added = (items => { });
			Inserted = ((index, items) => { });
			Moved = ((oldindex, newindex, item) => { });
			Removed = (items => { });
			RemovedAt = ((index, items) => { });
			Cleared = (count => { });
			Replaced = ((index, olditem, newitem) => { });
		}

		public DependencyList(Action<IEnumerable<T>> Added, Action<int, IEnumerable<T>> Inserted, Action<int, int, T> Moved, Action<IEnumerable<T>> Removed, Action<int, IEnumerable<T>> RemovedAt, Action<int> Cleared, Action<int, T, T> Replaced)
		{
			il = new List<T>();
			ocl = new ReaderWriterLockSlim();
			this.Added = Added;
			this.Inserted = Inserted;
			this.Moved = Moved;
			this.Removed = Removed;
			this.RemovedAt = RemovedAt;
			this.Cleared = Cleared;
			this.Replaced = Replaced;
		}

		public DependencyList(int Capacity, Action<IEnumerable<T>> Added, Action<int, IEnumerable<T>> Inserted, Action<int, int, T> Moved, Action<IEnumerable<T>> Removed, Action<int, IEnumerable<T>> RemovedAt, Action<int> Cleared, Action<int, T, T> Replaced)
		{
			il = new List<T>(Capacity);
			ocl = new ReaderWriterLockSlim();
			this.Added = Added;
			this.Inserted = Inserted;
			this.Moved = Moved;
			this.Removed = Removed;
			this.RemovedAt = RemovedAt;
			this.Cleared = Cleared;
			this.Replaced = Replaced;
		}

		public DependencyList(IEnumerable<T> Items, Action<IEnumerable<T>> Added, Action<int, IEnumerable<T>> Inserted, Action<int, int, T> Moved, Action<IEnumerable<T>> Removed, Action<int, IEnumerable<T>> RemovedAt, Action<int> Cleared, Action<int, T, T> Replaced)
		{
			il = new List<T>(Items);
			ocl = new ReaderWriterLockSlim();
			this.Added = Added;
			this.Inserted = Inserted;
			this.Moved = Moved;
			this.Removed = Removed;
			this.RemovedAt = RemovedAt;
			this.Cleared = Cleared;
			this.Replaced = Replaced;
		}

		public void Add(T item)
		{
			ocl.EnterWriteLock();
			try
			{
				il.Add(item);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallAdded(item);
		}

		public void AddRange(IEnumerable<T> items)
		{
			ocl.EnterWriteLock();
			try
			{
				il.AddRange(items);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallAdded(items);
		}

		public int BinarySearch(T item)
		{
			ocl.EnterReadLock();
			try
			{
				return il.BinarySearch(item);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int BinarySearch(T item, IComparer<T> comparer)
		{
			ocl.EnterReadLock();
			try
			{
				return il.BinarySearch(item, comparer);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int BinarySearch(int start, int count, T item, IComparer<T> comparer)
		{
			ocl.EnterReadLock();
			try
			{
				return il.BinarySearch(start, count, item, comparer);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public void Clear()
		{
			T[] tl = ToArray();
			ocl.EnterWriteLock();
			try
			{
				il.Clear();
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallCleared(tl.GetLength(0), tl);
		}

		public bool Contains(T item)
		{
			ocl.EnterReadLock();
			try
			{
				return il.Contains(item);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public void CopyTo(T[] array)
		{
			ocl.EnterReadLock();
			try
			{
				il.CopyTo(array);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			ocl.EnterReadLock();
			try
			{
				il.CopyTo(array, arrayIndex);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public void CopyTo(int index, T[] array, int arrayIndex, int count)
		{
			ocl.EnterReadLock();
			try
			{
				il.CopyTo(index, array, arrayIndex, count);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int Count
		{
			get
			{
				ocl.EnterReadLock();
				try
				{
					return il.Count;
				}
				finally
				{
					ocl.ExitReadLock();
				}
			}
		}

		public bool Exists(Predicate<T> match)
		{
			ocl.EnterReadLock();
			try
			{
				return il.Exists(match);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public T Find(Predicate<T> match)
		{
			ocl.EnterReadLock();
			try
			{
				return il.Find(match);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public IEnumerable<T> FindAll(Predicate<T> match)
		{
			ocl.EnterReadLock();
			try
			{
				return il.FindAll(match);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public T FindLast(Predicate<T> match)
		{
			ocl.EnterReadLock();
			try
			{
				return il.FindLast(match);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int FindIndex(Predicate<T> match)
		{
			ocl.EnterReadLock();
			try
			{
				return il.FindIndex(match);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int FindIndex(int start, Predicate<T> match)
		{
			ocl.EnterReadLock();
			try
			{
				return il.FindIndex(start, match);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int FindIndex(int start, int count, Predicate<T> match)
		{
			ocl.EnterReadLock();
			try
			{
				return il.FindIndex(start, count, match);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int FindLastIndex(Predicate<T> match)
		{
			ocl.EnterReadLock();
			try
			{
				return il.FindLastIndex(match);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int FindLastIndex(int start, Predicate<T> match)
		{
			ocl.EnterReadLock();
			try
			{
				return il.FindLastIndex(start, match);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int FindLastIndex(int start, int count, Predicate<T> match)
		{
			ocl.EnterReadLock();
			try
			{
				return il.FindLastIndex(start, count, match);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public IEnumerable<T> GetRange(int start, int count)
		{
			ocl.EnterReadLock();
			try
			{
				return il.GetRange(start, count);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int IndexOf(T item)
		{
			ocl.EnterReadLock();
			try
			{
				return il.IndexOf(item);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int IndexOf(T item, int start)
		{
			ocl.EnterReadLock();
			try
			{
				return il.IndexOf(item, start);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int IndexOf(T item, int start, int count)
		{
			ocl.EnterReadLock();
			try
			{
				return il.IndexOf(item, start, count);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public void Insert(int index, T item)
		{
			ocl.EnterWriteLock();
			try
			{
				il.Insert(index, item);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallInserted(index, item);
		}

		public void InsertRange(int index, IEnumerable<T> items)
		{
			ocl.EnterWriteLock();
			try
			{
				il.InsertRange(index, items);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallInserted(index, items);
		}

		public int LastIndexOf(T item)
		{
			ocl.EnterReadLock();
			try
			{
				return il.LastIndexOf(item);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int LastIndexOf(T item, int start)
		{
			ocl.EnterReadLock();
			try
			{
				return il.LastIndexOf(item, start);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int LastIndexOf(T item, int start, int count)
		{
			ocl.EnterReadLock();
			try
			{
				return il.LastIndexOf(item, start, count);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public void Move(int oldindex, int newindex)
		{
			T ti = this[oldindex];
			ocl.EnterWriteLock();
			try
			{
				il.Insert(newindex, ti);
				il.Remove(ti);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallMoved(oldindex, newindex, ti);
		}

		public bool Remove(T item)
		{
			ocl.EnterWriteLock();
			bool rt;
			try
			{
				rt = il.Remove(item);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallRemoved(item);
			return rt;
		}

		public void RemoveAt(int index)
		{
			T ti = this[index];
			ocl.EnterWriteLock();
			try
			{
				il.RemoveAt(index);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallRemovedAt(index, ti);
		}

		public void RemoveRange(int index, int count)
		{
			T[] tl = GetRange(index, count).ToArray();
			ocl.EnterWriteLock();
			try
			{
				il.RemoveRange(index, count);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallRemoved(tl);
		}

		public T[] ToArray()
		{
			ocl.EnterReadLock();
			try
			{
				return il.ToArray();
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public IEnumerable<T> ToList()
		{
			ocl.EnterReadLock();
			try
			{
				return il.ToList();
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public void TrimExcess()
		{
			ocl.EnterWriteLock();
			try
			{
				il.TrimExcess();
			}
			finally
			{
				ocl.ExitWriteLock();
			}
		}

		public bool TrueForAll(Predicate<T> match)
		{
			ocl.EnterReadLock();
			try
			{
				return il.TrueForAll(match);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		private async void CallAdded(T item)
		{
			if (Window.Current.Dispatcher == null) { Added(new List<T> { item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item)); return; }
			if (Window.Current.Dispatcher.HasThreadAccess) { Added(new List<T> { item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item)); }
			else await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { Added(new List<T> { item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item)); });
		}

		private async void CallAdded(IEnumerable<T> items)
		{
			if (Window.Current.Dispatcher == null) { Added(items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(items))); return; }
			if (Window.Current.Dispatcher.HasThreadAccess) { Added(items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(items))); }
			else await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { Added(items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(items))); });
		}

		private async void CallInserted(int index, T item)
		{
			if (Window.Current.Dispatcher == null) { Inserted(index, new List<T>{ item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index)); return; }
			if (Window.Current.Dispatcher.HasThreadAccess) { Inserted(index, new List<T>{ item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index)); }
			else await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { Inserted(index, new List<T>{ item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index)); });
		}

		private async void CallInserted(int index, IEnumerable<T> items)
		{
			if (Window.Current.Dispatcher == null) { Inserted(index, items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(items), index)); return; }
			if (Window.Current.Dispatcher.HasThreadAccess) { Inserted(index, items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(items), index)); }
			else await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { Inserted(index, items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(items), index)); });
		}

		private async void CallRemoved(T item)
		{
			if (Window.Current.Dispatcher == null) { Removed(new List<T>{ item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item)); return; }
			if (Window.Current.Dispatcher.HasThreadAccess) { Removed(new List<T>{ item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item)); }
			else await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { Removed(new List<T>{ item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item)); });
		}

		private async void CallRemoved(IEnumerable<T> items)
		{
			if (Window.Current.Dispatcher == null) { Removed(items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<T>(items))); return; }
			if (Window.Current.Dispatcher.HasThreadAccess) { Removed(items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<T>(items))); }
			else await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { Removed(items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<T>(items))); });
		}

		private async void CallRemovedAt(int index, T item)
		{
			if (Window.Current.Dispatcher == null) { RemovedAt(index, new List<T>{ item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index)); return; }
			if (Window.Current.Dispatcher.HasThreadAccess) { RemovedAt(index, new List<T>{ item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index)); }
			else await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { RemovedAt(index, new List<T>{ item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index)); });
		}

		private async void CallCleared(int count, IEnumerable<T> items)
		{
			if (Window.Current.Dispatcher == null) { Cleared(count); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, new List<T>(items))); return; }
			if (Window.Current.Dispatcher.HasThreadAccess) { Cleared(count); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, new List<T>(items))); }
			else await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { Cleared(count); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, new List<T>(items))); });
		}

		private async void CallMoved(int oldindex, int newindex, T item)
		{
			if (Window.Current.Dispatcher == null) { Moved(newindex, oldindex, item); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newindex, oldindex)); return; }
			if (Window.Current.Dispatcher.HasThreadAccess) { Moved(newindex, oldindex, item); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newindex, oldindex)); }
			else await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { Moved(newindex, oldindex, item); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newindex, oldindex)); });
		}

		private async void CallReplaced(int index, T olditem, T newitem)
		{
			if (Window.Current.Dispatcher == null) { Replaced(index, olditem, newitem); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newitem, olditem, index)); return; }
			if (Window.Current.Dispatcher.HasThreadAccess) { Replaced(index, olditem, newitem); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newitem, olditem, index)); }
			else await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { Replaced(index, olditem, newitem); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newitem, olditem, index)); });
		}

		public T this[int index]
		{
			get
			{
				ocl.EnterReadLock();
				try
				{
					return il[index];
				}
				finally
				{
					ocl.ExitReadLock();
				}
			}
			set
			{
				T ti = this[index];
				ocl.EnterWriteLock();
				try
				{
					il[index] = value;
				}
				finally
				{
					ocl.ExitWriteLock();
				}
				CallReplaced(index, ti, value);
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public IEnumerator<T> GetEnumerator()
		{
			ocl.EnterReadLock();
			try
			{
				return il.GetEnumerator();
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			ocl.EnterReadLock();
			try
			{
				return ((IEnumerable)il).GetEnumerator();
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = "")
		{
			if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
		}
	}
}