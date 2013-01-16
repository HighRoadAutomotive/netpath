using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace System.Collections.Generic
{
	public class DependencyStack<T> : IEnumerable<T>, ICollection
	{
		private readonly Stack<T> il;
		private readonly ReaderWriterLock ocl;
		private readonly Action<IEnumerable<T>> Pushed;
		private readonly Action<IEnumerable<T>> Popped;
		private readonly Action<int> Cleared;

		public DependencyStack(Action<IEnumerable<T>> Pushed, Action<IEnumerable<T>> Popped, Action<int> Cleared)
		{
			il = new Stack<T>();
			this.Pushed = Pushed ?? (ItemList => { });
			this.Popped = Popped ?? ((Index, ItemList) => { });
			this.Cleared = Cleared ?? (count => { });
		}

		public DependencyStack(IEnumerable<T> Items, Action<IEnumerable<T>> Pushed, Action<IEnumerable<T>> Popped, Action<int> Cleared)
		{
			il = new Stack<T>(Items);
			this.Pushed = Pushed ?? (ItemList => { });
			this.Popped = Popped ?? ((Index, ItemList) => { });
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

		public void Push(T Item)
		{
			ocl.AcquireWriterLock(0);
			try
			{
				il.Push(Item);
				CallPushed(Item);
			}
			finally
			{
				ocl.ReleaseWriterLock();
			}
		}

		public void PushRange(T[] Items)
		{
			ocl.AcquireWriterLock(0);
			try
			{
				foreach(T t in Items)
					il.Push(t);
				CallPushed(Items);
			}
			finally
			{
				ocl.ReleaseWriterLock();
			}
		}

		public void PushRange(T[] Items, int Start, int Count)
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
					il.Push(Items[i]);
				CallPushed(Items);
			}
			finally
			{
				ocl.ReleaseWriterLock();
			}
		}

		public T Pop()
		{
			ocl.AcquireReaderLock(0);
			try
			{
				T t = il.Pop();
				CallPopped(t);
				return t;
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public int PopRange(T[] Items, int Start, int Count)
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
					tl[c++] = Items[i] = il.Pop();
				CallPopped(tl);
				return Count;
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		#region - Update Calls -

		private void CallPushed(T item)
		{
			if (Application.Current.Dispatcher == null) { Pushed(new List<T> { item }); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Pushed(new List<T> { item }); }
			else Application.Current.Dispatcher.Invoke(new Action<IEnumerable<T>>(c => Pushed(new List<T> { item })), DispatcherPriority.Normal);
		}

		private void CallPushed(IEnumerable<T> items)
		{
			if (Application.Current.Dispatcher == null) { Pushed(items); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Pushed(items); }
			else Application.Current.Dispatcher.Invoke(new Action<IEnumerable<T>>(c => Pushed(items)), DispatcherPriority.Normal);
		}

		private void CallPopped(T item)
		{
			if (Application.Current.Dispatcher == null) { Popped(new List<T> { item }); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Popped(new List<T> { item }); }
			else Application.Current.Dispatcher.Invoke(new Action<IEnumerable<T>>(c => Popped(new List<T> { item })), DispatcherPriority.Normal);
		}

		private void CallPopped(IEnumerable<T> items)
		{
			if (Application.Current.Dispatcher == null) { Popped(items); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Popped(items); }
			else Application.Current.Dispatcher.Invoke(new Action<IEnumerable<T>>(c => Popped(items)), DispatcherPriority.Normal);
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