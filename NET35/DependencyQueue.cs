using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace System.Collections.Generic
{
	public class DependencyQueue<T> : IEnumerable<T>, ICollection
	{
		private readonly Queue<T> il;
		private readonly ReaderWriterLockSlim ocl;
		private readonly Action<IEnumerable<T>> Enqueued;
		private readonly Action<IEnumerable<T>> Dequeued;
		private readonly Action<int> Cleared;

		public DependencyQueue(Action<IEnumerable<T>> Enqueued, Action<IEnumerable<T>> Dequeued, Action<int> Cleared)
		{
			il = new Queue<T>();
			this.Enqueued = Enqueued ?? (ItemList => { });
			this.Dequeued = Dequeued ?? ((Index, ItemList) => { });
			this.Cleared = Cleared ?? (count => { });
		}

		public DependencyQueue(IEnumerable<T> Items, Action<IEnumerable<T>> Enqueued, Action<IEnumerable<T>> Dequeued, Action<int> Cleared)
		{
			il = new Queue<T>(Items);
			this.Enqueued = Enqueued ?? (ItemList => { });
			this.Dequeued = Dequeued ?? ((Index, ItemList) => { });
			this.Cleared = Cleared ?? (count => { });
		}

		public void Clear()
		{
			ocl.EnterWriteLock();
			try
			{
				int t = il.Count;
				T[] tl = il.ToArray();
				il.Clear();
				CallCleared(t, tl);
			}
			finally 
			{
				ocl.ExitWriteLock();
			}
		}

		public bool Contains(T Item)
		{
			ocl.EnterReadLock();
			try
			{
				return il.Contains(Item);
			}
			finally 
			{
				ocl.ExitReadLock();
			}
		}

		public T Peek()
		{
			ocl.EnterReadLock();
			try
			{
				return il.Peek();
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public void Enqueue(T Item)
		{
			ocl.EnterWriteLock();
			try
			{
				il.Enqueue(Item);
				CallEnqueued(Item);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
		}

		public void EnqueueRange(T[] Items)
		{
			ocl.EnterWriteLock();
			try
			{
				foreach(T t in Items)
					il.Enqueue(t);
				CallEnqueued(Items);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
		}

		public void EnqueueRange(T[] Items, int Start, int Count)
		{
			if (Items == null) throw new ArgumentNullException("Items");
			if (Start < 0) throw new ArgumentOutOfRangeException("Start", "Start cannot be negative.");
			if (Count < 0) throw new ArgumentOutOfRangeException("Count", "Count cannot be negative.");
			if (Start > Items.Length) throw new ArgumentOutOfRangeException("Start", "Start cannot be greater than the length of Items.");
			if (Start + Count > Items.Length) throw new ArgumentException("Start + Count cannot be greater than the length of Items.");

			ocl.EnterWriteLock();
			try
			{
				for (int i = Start; i < (Count+i); i++)
					il.Enqueue(Items[i]);
				CallEnqueued(Items);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
		}

		public T Dequeue()
		{
			ocl.EnterReadLock();
			try
			{
				T t = il.Dequeue();
				CallDequeued(t);
				return t;
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int DequeueRange(T[] Items, int Start, int Count)
		{
			if (Items == null) throw new ArgumentNullException("Items");
			if (Start < 0) throw new ArgumentOutOfRangeException("Start", "Start cannot be negative.");
			if (Count < 0) throw new ArgumentOutOfRangeException("Count", "Count cannot be negative.");
			if (Start > Items.Length) throw new ArgumentOutOfRangeException("Start", "Start cannot be greater than the length of Items.");
			if (Start + Count > Items.Length) throw new ArgumentException("Start + Count cannot be greater than the length of Items.");

			ocl.EnterReadLock();
			try
			{
				var tl = new T[Count];
				int c = 0;
				for (int i = Start; i < (Count + i); i++)
					tl[c++] = Items[i] = il.Dequeue();
				CallDequeued(tl);
				return Count;
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		#region - Update Calls -

		private void CallEnqueued(T item)
		{
			if (Application.Current.Dispatcher == null) { Enqueued(new List<T> { item }); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Enqueued(new List<T> { item }); }
			else Application.Current.Dispatcher.Invoke(new Action(() => Enqueued(new List<T> { item })), DispatcherPriority.Normal);
		}

		private void CallEnqueued(IEnumerable<T> items)
		{
			if (Application.Current.Dispatcher == null) { Enqueued(items); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Enqueued(items); }
			else Application.Current.Dispatcher.Invoke(new Action(() => Enqueued(items)), DispatcherPriority.Normal);
		}

		private void CallDequeued(T item)
		{
			if (Application.Current.Dispatcher == null) { Dequeued(new List<T> { item }); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Dequeued(new List<T> { item }); }
			else Application.Current.Dispatcher.Invoke(new Action(() => Dequeued(new List<T> { item })), DispatcherPriority.Normal);
		}

		private void CallDequeued(IEnumerable<T> items)
		{
			if (Application.Current.Dispatcher == null) { Dequeued(items); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Dequeued(items); }
			else Application.Current.Dispatcher.Invoke(new Action(() => Dequeued(items)), DispatcherPriority.Normal);
		}

		private void CallCleared(int count, IEnumerable<T> items)
		{
			if (Application.Current.Dispatcher == null) { Cleared(count); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Cleared(count); }
			else Application.Current.Dispatcher.Invoke(new Action(() => Cleared(count)), DispatcherPriority.Normal);
		}
		#endregion

		#region - Interface Implementations -

		public void CopyTo(T[] array, int index)
		{
			ocl.EnterReadLock();
			try
			{
				il.CopyTo(array, index);
			}
			finally 
			{
				ocl.ExitReadLock();
			}
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
				return ((ICollection) il).GetEnumerator();
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			ocl.EnterReadLock();
			try
			{
				((ICollection) il).CopyTo(array, index);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int Count
		{
			get { ocl.EnterReadLock(); try { return il.Count; } finally { ocl.ExitReadLock(); } }
		}

		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		object ICollection.SyncRoot
		{
			get { return null; }
		}

		#endregion
	}
}