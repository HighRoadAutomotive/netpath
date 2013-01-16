using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace System.Collections.Generic
{
	public class DependencyQueue<T> : IEnumerable<T>, ICollection
	{
		private readonly Queue<T> il;
		private readonly ReaderWriterLock ocl;
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
			ocl.AcquireWriterLock(0);
			try
			{
				int t = il.Count;
				T[] tl = il.ToArray();
				il.Clear();
				CallCleared(t, tl);
			}
			finally 
			{
				ocl.ReleaseWriterLock();
			}
		}

		public bool Contains(T Item)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.Contains(Item);
			}
			finally 
			{
				ocl.ReleaseReaderLock();
			}
		}

		public T Peek()
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.Peek();
			}
			finally 
			{
				ocl.ReleaseReaderLock();
			}
		}

		public void Enqueue(T Item)
		{
			ocl.AcquireWriterLock(0);
			try
			{
				il.Enqueue(Item);
				CallEnqueued(Item);
			}
			finally
			{
				ocl.ReleaseWriterLock();
			}
		}

		public void EnqueueRange(T[] Items)
		{
			ocl.AcquireWriterLock(0);
			try
			{
				foreach(T t in Items)
					il.Enqueue(t);
				CallEnqueued(Items);
			}
			finally
			{
				ocl.ReleaseWriterLock();
			}
		}

		public void EnqueueRange(T[] Items, int Start, int Count)
		{
			if (Items == null) throw new ArgumentNullException("Items");
			if (Start < 0) throw new ArgumentOutOfRangeException("Start", "Start cannot be negative.");
			if (Count < 0) throw new ArgumentOutOfRangeException("Count", "Count cannot be negative.");
			if (Start > Items.Length) throw new ArgumentOutOfRangeException("Start", "Start cannot be greater than the length of Items.");
			if (Start + Count > Items.Length) throw new ArgumentException("Start + Count cannot be greater than the length of Items.");

			ocl.AcquireWriterLock(0);
			try
			{
				for (int i = Start; i < (Count+i); i++)
					il.Enqueue(Items[i]);
				CallEnqueued(Items);
			}
			finally
			{
				ocl.ReleaseWriterLock();
			}
		}

		public T Dequeue()
		{
			ocl.AcquireReaderLock(0);
			try
			{
				T t = il.Dequeue();
				CallDequeued(t);
				return t;
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public int DequeueRange(T[] Items, int Start, int Count)
		{
			if (Items == null) throw new ArgumentNullException("Items");
			if (Start < 0) throw new ArgumentOutOfRangeException("Start", "Start cannot be negative.");
			if (Count < 0) throw new ArgumentOutOfRangeException("Count", "Count cannot be negative.");
			if (Start > Items.Length) throw new ArgumentOutOfRangeException("Start", "Start cannot be greater than the length of Items.");
			if (Start + Count > Items.Length) throw new ArgumentException("Start + Count cannot be greater than the length of Items.");

			ocl.AcquireReaderLock(0);
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
				ocl.ReleaseReaderLock();
			}
		}

		#region - Update Calls -

		private void CallEnqueued(T item)
		{
			if (Application.Current.Dispatcher == null) { Enqueued(new List<T> { item }); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Enqueued(new List<T> { item }); }
			else Application.Current.Dispatcher.Invoke(new Action<IEnumerable<T>>(c => Enqueued(new List<T> { item })), DispatcherPriority.Normal);
		}

		private void CallEnqueued(IEnumerable<T> items)
		{
			if (Application.Current.Dispatcher == null) { Enqueued(items); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Enqueued(items); }
			else Application.Current.Dispatcher.Invoke(new Action<IEnumerable<T>>(c => Enqueued(items)), DispatcherPriority.Normal);
		}

		private void CallDequeued(T item)
		{
			if (Application.Current.Dispatcher == null) { Dequeued(new List<T> { item }); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Dequeued(new List<T> { item }); }
			else Application.Current.Dispatcher.Invoke(new Action<IEnumerable<T>>(c => Dequeued(new List<T> { item })), DispatcherPriority.Normal);
		}

		private void CallDequeued(IEnumerable<T> items)
		{
			if (Application.Current.Dispatcher == null) { Dequeued(items); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Dequeued(items); }
			else Application.Current.Dispatcher.Invoke(new Action<IEnumerable<T>>(c => Dequeued(items)), DispatcherPriority.Normal);
		}

		private void CallCleared(int count, IEnumerable<T> items)
		{
			if (Application.Current.Dispatcher == null) { Cleared(count); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Cleared(count); }
			else Application.Current.Dispatcher.Invoke(new Action<int>(c => Cleared(count)), DispatcherPriority.Normal);
		}
		#endregion

		#region - Interface Implementations -

		public void CopyTo(T[] array, int index)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				il.CopyTo(array, index);
			}
			finally 
			{
				ocl.ReleaseReaderLock();
			}
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
				return ((ICollection) il).GetEnumerator();
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				((ICollection) il).CopyTo(array, index);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public int Count
		{
			get { ocl.AcquireReaderLock(0); try { return il.Count; } finally { ocl.ReleaseReaderLock(); } }
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