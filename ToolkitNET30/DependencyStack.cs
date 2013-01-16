using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace System.Collections.Generic
{
	public class DependencyStack<T> : IEnumerable<T>, ICollection
	{
		private readonly Stack<T> il;
		private readonly ReaderWriterLockSlim ocl;
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

		public void Push(T Item)
		{
			ocl.EnterWriteLock();
			try
			{
				il.Push(Item);
				CallPushed(Item);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
		}

		public void PushRange(T[] Items)
		{
			ocl.EnterWriteLock();
			try
			{
				foreach(T t in Items)
					il.Push(t);
				CallPushed(Items);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
		}

		public void PushRange(T[] Items, int Start, int Count)
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
					il.Push(Items[i]);
				CallPushed(Items);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
		}

		public T Pop()
		{
			ocl.EnterReadLock();
			try
			{
				T t = il.Pop();
				CallPopped(t);
				return t;
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int PopRange(T[] Items, int Start, int Count)
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
					tl[c++] = Items[i] = il.Pop();
				CallPopped(tl);
				return Count;
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		#region - Update Calls -

		private void CallPushed(T item)
		{
			if (Application.Current.Dispatcher == null) { Pushed(new List<T> { item }); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Pushed(new List<T> { item }); }
			else Application.Current.Dispatcher.Invoke(new Action(() => Pushed(new List<T> { item })), DispatcherPriority.Normal);
		}

		private void CallPushed(IEnumerable<T> items)
		{
			if (Application.Current.Dispatcher == null) { Pushed(items); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Pushed(items); }
			else Application.Current.Dispatcher.Invoke(new Action(() => Pushed(items)), DispatcherPriority.Normal);
		}

		private void CallPopped(T item)
		{
			if (Application.Current.Dispatcher == null) { Popped(new List<T> { item }); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Popped(new List<T> { item }); }
			else Application.Current.Dispatcher.Invoke(new Action(() => Popped(new List<T> { item })), DispatcherPriority.Normal);
		}

		private void CallPopped(IEnumerable<T> items)
		{
			if (Application.Current.Dispatcher == null) { Popped(items); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Popped(items); }
			else Application.Current.Dispatcher.Invoke(new Action(() => Popped(items)), DispatcherPriority.Normal);
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