using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace System.Collections.Generic
{
	public class DependencyList<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		public class DependencyListInsertedArgs
		{
			public int Index { get; private set; }
			public IEnumerable<T> NewValues { get; private set; }

			private DependencyListInsertedArgs() { }

			public DependencyListInsertedArgs(int Index, IEnumerable<T> NewValues)
			{
				this.Index = Index;
				this.NewValues = NewValues;
			}
		}

		public class DependencyListRemovedArgs
		{
			public int Index { get; private set; }
			public IEnumerable<T> OldValues { get; private set; }

			private DependencyListRemovedArgs() { }

			public DependencyListRemovedArgs(int Index, IEnumerable<T> OldValues)
			{
				this.Index = Index;
				this.OldValues = OldValues;
			}
		}

		public class DependencyListMovedArgs
		{
			public int OldIndex { get; private set; }
			public int NewIndex { get; private set; }
			public T Value { get; private set; }

			private DependencyListMovedArgs() { }

			public DependencyListMovedArgs(int NewIndex, int OldIndex, T Value)
			{
				this.OldIndex = OldIndex;
				this.NewIndex = NewIndex;
				this.Value = Value;
			}
		}

		public class DependencyListReplacedArgs
		{
			public T OldValue { get; private set; }
			public T NewValue { get; private set; }
			public int Index { get; private set; }

			private DependencyListReplacedArgs() { }

			public DependencyListReplacedArgs(int Index, T NewValue, T OldValue)
			{
				this.OldValue = OldValue;
				this.NewValue = NewValue;
				this.Index = Index;
			}
		}

		private readonly List<T> il;
		private readonly ReaderWriterLock ocl;
		private readonly Action<IEnumerable<T>> Added;
		private readonly Action<DependencyListInsertedArgs> Inserted;
		private readonly Action<DependencyListMovedArgs> Moved;
		private readonly Action<IEnumerable<T>> Removed;
		private readonly Action<DependencyListRemovedArgs> RemovedAt;
		private readonly Action<int> Cleared;
		private readonly Action<DependencyListReplacedArgs> Replaced;

		public DependencyList()
		{
			il = new List<T>();
			ocl = new ReaderWriterLock();
			Added = (items => { });
			Inserted = (InsertedArgs => { });
			Moved = (MovedArgs => { });
			Removed = (items => { });
			RemovedAt = (RemovedArgs => { });
			Cleared = (count => { });
			Replaced = (ReplacedArgs => { });
		}

		public DependencyList(int Capacity)
		{
			il = new List<T>(Capacity);
			ocl = new ReaderWriterLock();
			Added = (items => { });
			Inserted = (InsertedArgs => { });
			Moved = (MovedArgs => { });
			Removed = (items => { });
			RemovedAt = (RemovedArgs => { });
			Cleared = (count => { });
			Replaced = (ReplacedArgs => { });
		}

		public DependencyList(IEnumerable<T> Items)
		{
			il = new List<T>(Items);
			ocl = new ReaderWriterLock();
			Added = (items => { });
			Inserted = (InsertedArgs => { });
			Moved = (MovedArgs => { });
			Removed = (items => { });
			RemovedAt = (RemovedArgs => { });
			Cleared = (count => { });
			Replaced = (ReplacedArgs => { });
		}

		public DependencyList(Action<IEnumerable<T>> Added, Action<DependencyListInsertedArgs> Inserted, Action<DependencyListMovedArgs> Moved, Action<IEnumerable<T>> Removed, Action<DependencyListRemovedArgs> RemovedAt, Action<int> Cleared, Action<DependencyListReplacedArgs> Replaced)
		{
			il = new List<T>();
			ocl = new ReaderWriterLock();
			this.Added = Added;
			this.Inserted = Inserted;
			this.Moved = Moved;
			this.Removed = Removed;
			this.RemovedAt = RemovedAt;
			this.Cleared = Cleared;
			this.Replaced = Replaced;
		}

		public DependencyList(int Capacity, Action<IEnumerable<T>> Added, Action<DependencyListInsertedArgs> Inserted, Action<DependencyListMovedArgs> Moved, Action<IEnumerable<T>> Removed, Action<DependencyListRemovedArgs> RemovedAt, Action<int> Cleared, Action<DependencyListReplacedArgs> Replaced)
		{
			il = new List<T>(Capacity);
			ocl = new ReaderWriterLock();
			this.Added = Added;
			this.Inserted = Inserted;
			this.Moved = Moved;
			this.Removed = Removed;
			this.RemovedAt = RemovedAt;
			this.Cleared = Cleared;
			this.Replaced = Replaced;
		}

		public DependencyList(IEnumerable<T> Items, Action<IEnumerable<T>> Added, Action<DependencyListInsertedArgs> Inserted, Action<DependencyListMovedArgs> Moved, Action<IEnumerable<T>> Removed, Action<DependencyListRemovedArgs> RemovedAt, Action<int> Cleared, Action<DependencyListReplacedArgs> Replaced)
		{
			il = new List<T>(Items);
			ocl = new ReaderWriterLock();
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
			ocl.AcquireWriterLock(0);
			try
			{
				il.Add(item);
			}
			finally
			{
				ocl.ReleaseWriterLock();
			}
			CallAdded(item);
		}

		public void AddRange(IEnumerable<T> items)
		{
			ocl.AcquireWriterLock(0);
			try
			{
				il.AddRange(items);
			}
			finally
			{
				ocl.ReleaseWriterLock();
			}
			CallAdded(items);
		}

		public int BinarySearch(T item)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.BinarySearch(item);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public int BinarySearch(T item, IComparer<T> comparer)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.BinarySearch(item, comparer);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public int BinarySearch(int start, int count, T item, IComparer<T> comparer)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.BinarySearch(start, count, item, comparer);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public void Clear()
		{
			T[] tl = ToArray();
			ocl.AcquireWriterLock(0);
			try
			{
				il.Clear();
			}
			finally
			{
				ocl.ReleaseWriterLock();
			}
			CallCleared(tl.GetLength(0), tl);
		}

		public bool Contains(T item)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.Contains(item);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public void CopyTo(T[] array)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				il.CopyTo(array);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				il.CopyTo(array, arrayIndex);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public void CopyTo(int index, T[] array, int arrayIndex, int count)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				il.CopyTo(index, array, arrayIndex, count);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public int Count
		{
			get
			{
				ocl.AcquireReaderLock(0);
				try
				{
					return il.Count;
				}
				finally
				{
					ocl.ReleaseReaderLock();
				}
			}
		}

		public bool Exists(Predicate<T> match)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.Exists(match);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public T Find(Predicate<T> match)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.Find(match);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public IEnumerable<T> FindAll(Predicate<T> match)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.FindAll(match);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public T FindLast(Predicate<T> match)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.FindLast(match);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public int FindIndex(Predicate<T> match)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.FindIndex(match);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public int FindIndex(int start, Predicate<T> match)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.FindIndex(start, match);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public int FindIndex(int start, int count, Predicate<T> match)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.FindIndex(start, count, match);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public int FindLastIndex(Predicate<T> match)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.FindLastIndex(match);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public int FindLastIndex(int start, Predicate<T> match)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.FindLastIndex(start, match);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public int FindLastIndex(int start, int count, Predicate<T> match)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.FindLastIndex(start, count, match);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public IEnumerable<T> GetRange(int start, int count)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.GetRange(start, count);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public int IndexOf(T item)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.IndexOf(item);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public int IndexOf(T item, int start)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.IndexOf(item, start);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public int IndexOf(T item, int start, int count)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.IndexOf(item, start, count);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public void Insert(int index, T item)
		{
			ocl.AcquireWriterLock(0);
			try
			{
				il.Insert(index, item);
			}
			finally
			{
				ocl.ReleaseWriterLock();
			}
			CallInserted(index, item);
		}

		public void InsertRange(int index, IEnumerable<T> items)
		{
			ocl.AcquireWriterLock(0);
			try
			{
				il.InsertRange(index, items);
			}
			finally
			{
				ocl.ReleaseWriterLock();
			}
			CallInserted(index, items);
		}

		public int LastIndexOf(T item)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.LastIndexOf(item);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public int LastIndexOf(T item, int start)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.LastIndexOf(item, start);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public int LastIndexOf(T item, int start, int count)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.LastIndexOf(item, start, count);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public void Move(int oldindex, int newindex)
		{
			T ti = this[oldindex];
			ocl.AcquireWriterLock(0);
			try
			{
				il.Insert(newindex, ti);
				il.Remove(ti);
			}
			finally
			{
				ocl.ReleaseWriterLock();
			}
			CallMoved(oldindex, newindex, ti);
		}

		public bool Remove(T item)
		{
			ocl.AcquireWriterLock(0);
			bool rt;
			try
			{
				rt = il.Remove(item);
			}
			finally
			{
				ocl.ReleaseWriterLock();
			}
			CallRemoved(item);
			return rt;
		}

		public void RemoveAt(int index)
		{
			T ti = this[index];
			ocl.AcquireWriterLock(0);
			try
			{
				il.RemoveAt(index);
			}
			finally
			{
				ocl.ReleaseWriterLock();
			}
			CallRemovedAt(index, ti);
		}

		public void RemoveRange(int index, int count)
		{
			T[] tl = new List<T>(GetRange(index, count)).ToArray();
			ocl.AcquireWriterLock(0);
			try
			{
				il.RemoveRange(index, count);
			}
			finally
			{
				ocl.ReleaseWriterLock();
			}
			CallRemoved(tl);
		}

		public T[] ToArray()
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.ToArray();
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public void TrimExcess()
		{
			ocl.AcquireWriterLock(0);
			try
			{
				il.TrimExcess();
			}
			finally
			{
				ocl.ReleaseWriterLock();
			}
		}

		public bool TrueForAll(Predicate<T> match)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.TrueForAll(match);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		private void CallAdded(T item)
		{
			if (Application.Current.Dispatcher == null) { Added(new List<T> { item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item)); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Added(new List<T> { item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item)); }
			else Application.Current.Dispatcher.Invoke(new Action<int>(c => { Added(new List<T> { item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item)); }), DispatcherPriority.Normal);
		}

		private void CallAdded(IEnumerable<T> items)
		{
			if (Application.Current.Dispatcher == null) { Added(items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(items))); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Added(items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(items))); }
			else Application.Current.Dispatcher.Invoke(new Action<int>(c => { Added(items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(items))); }), DispatcherPriority.Normal);
		}

		private void CallInserted(int index, T item)
		{
			if (Application.Current.Dispatcher == null) { Inserted(new DependencyListInsertedArgs(index, new List<T>{ item })); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index)); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Inserted(new DependencyListInsertedArgs(index, new List<T> { item })); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index)); }
			else Application.Current.Dispatcher.Invoke(new Action<int>(c => { Inserted(new DependencyListInsertedArgs(index, new List<T> { item })); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index)); }), DispatcherPriority.Normal);
		}

		private void CallInserted(int index, IEnumerable<T> items)
		{
			if (Application.Current.Dispatcher == null) { Inserted(new DependencyListInsertedArgs(index, items)); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(items), index)); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Inserted(new DependencyListInsertedArgs(index, items)); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(items), index)); }
			else Application.Current.Dispatcher.Invoke(new Action<int>(c => { Inserted(new DependencyListInsertedArgs(index, items)); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(items), index)); }), DispatcherPriority.Normal);
		}

		private void CallRemoved(T item)
		{
			if (Application.Current.Dispatcher == null) { Removed(new List<T>{ item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item)); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Removed(new List<T>{ item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item)); }
			else Application.Current.Dispatcher.Invoke(new Action<int>(c => { Removed(new List<T> { item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item)); }), DispatcherPriority.Normal);
		}

		private void CallRemoved(IEnumerable<T> items)
		{
			if (Application.Current.Dispatcher == null) { Removed(items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<T>(items))); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Removed(items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<T>(items))); }
			else Application.Current.Dispatcher.Invoke(new Action<int>(c => { Removed(items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<T>(items))); }), DispatcherPriority.Normal);
		}

		private void CallRemovedAt(int index, T item)
		{
			if (Application.Current.Dispatcher == null) { RemovedAt(new DependencyListRemovedArgs(index, new List<T>{ item })); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index)); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { RemovedAt(new DependencyListRemovedArgs(index, new List<T> { item })); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index)); }
			else Application.Current.Dispatcher.Invoke(new Action<int>(c => { RemovedAt(new DependencyListRemovedArgs(index, new List<T> { item })); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index)); }), DispatcherPriority.Normal);
		}

		private void CallCleared(int count, IEnumerable<T> items)
		{
			if (Application.Current.Dispatcher == null) { Cleared(count); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, new List<T>(items))); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Cleared(count); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, new List<T>(items))); }
			else Application.Current.Dispatcher.Invoke(new Action<int>(c => { Cleared(count); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, new List<T>(items))); }), DispatcherPriority.Normal);
		}

		private void CallMoved(int oldindex, int newindex, T item)
		{
			if (Application.Current.Dispatcher == null) { Moved(new DependencyListMovedArgs(newindex, oldindex, item)); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newindex, oldindex)); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Moved(new DependencyListMovedArgs(newindex, oldindex, item)); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newindex, oldindex)); }
			else Application.Current.Dispatcher.Invoke(new Action<int>(c => { Moved(new DependencyListMovedArgs(newindex, oldindex, item)); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newindex, oldindex)); }), DispatcherPriority.Normal);
		}

		private void CallReplaced(int index, T olditem, T newitem)
		{
			if (Application.Current.Dispatcher == null) { Replaced(new DependencyListReplacedArgs(index, olditem, newitem)); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newitem, olditem, index)); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Replaced(new DependencyListReplacedArgs(index, olditem, newitem)); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newitem, olditem, index)); }
			else Application.Current.Dispatcher.Invoke(new Action<int>(c => { Replaced(new DependencyListReplacedArgs(index, olditem, newitem)); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newitem, olditem, index)); }), DispatcherPriority.Normal);
		}

		public T this[int index]
		{
			get
			{
				ocl.AcquireReaderLock(0);
				try
				{
					return il[index];
				}
				finally
				{
					ocl.ReleaseReaderLock();
				}
			}
			set
			{
				T ti = this[index];
				ocl.AcquireWriterLock(0);
				try
				{
					il[index] = value;
				}
				finally
				{
					ocl.ReleaseWriterLock();
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
			ocl.AcquireReaderLock(0);
			try
			{
				return il.GetEnumerator();
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return ((IEnumerable)il).GetEnumerator();
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string PropertyName)
		{
			if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
		}
	}
}