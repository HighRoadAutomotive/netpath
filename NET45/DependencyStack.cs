using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace System.Collections.Generic
{
	[Serializable]
	public class DependencyStack<T> : IProducerConsumerCollection<T>
	{
		private ConcurrentStack<T> il;
		public delegate void ChangedEventHandler(IEnumerable<T> Value);
		public event ChangedEventHandler Pushed;
		public event ChangedEventHandler Popped;
		public event ChangedEventHandler Cleared;

		public DependencyStack()
		{
			il = new ConcurrentStack<T>();
		}

		public DependencyStack(IEnumerable<T> Items)
		{
			il = new ConcurrentStack<T>(Items);
		}

		public void Clear()
		{
			int t = il.Count;
			T[] tl = il.ToArray();
			il.Clear();
			CallCleared(tl);
		}

		public void Push(T Item)
		{
			il.Push(Item);
			CallPushed(Item);
		}

		public void PushRange(T[] Items)
		{
			il.PushRange(Items);
			CallPushed(Items);
		}

		public void PushRange(T[] Items, int Start, int Count)
		{
			il.PushRange(Items, Start, Count);
			CallPushed(Items);
		}

		public bool TryPeek(out T Result)
		{
			return il.TryPeek(out Result);
		}

		public bool TryPop(out T Result)
		{
			bool t = il.TryPop(out Result);
			CallPopped(Result);
			return t;
		}

		public int TryPopRange(T[] Items)
		{
			int t = il.TryPopRange(Items);
			CallPopped(Items);
			return t;
		}

		public int TryPopRange(T[] Items, int Start, int Count)
		{
			int t = il.TryPopRange(Items, Start, Count);
			CallPopped(Items);
			return t;
		}

		public void FromRange(IEnumerable<T> range)
		{
			System.Threading.Interlocked.Exchange(ref il, new ConcurrentStack<T>(range));
		}

		public ConcurrentStack<T> ToConcurrentStack()
		{
			return new ConcurrentStack<T>(il.ToArray());
		}

		public Stack<T> ToStack()
		{
			return new Stack<T>(il.ToArray());
		}

		public void ClearEventHandlers()
		{
			Cleared = null;
			Pushed = null;
			Popped = null;
		}

		#region - Update Calls -

		private void CallPushed(T item)
		{
			if (Application.Current.Dispatcher == null) { Pushed(new List<T> { item }); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Pushed(new List<T> { item }); }
			else Application.Current.Dispatcher.Invoke(() => Pushed(new List<T> { item }), DispatcherPriority.Normal);
		}

		private void CallPushed(IEnumerable<T> items)
		{
			if (Application.Current.Dispatcher == null) { Pushed(items); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Pushed(items); }
			else Application.Current.Dispatcher.Invoke(() => Pushed(items), DispatcherPriority.Normal);
		}

		private void CallPopped(T item)
		{
			if (Application.Current.Dispatcher == null) { Popped(new List<T> { item }); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Popped(new List<T> { item }); }
			else Application.Current.Dispatcher.Invoke(() => Popped(new List<T> { item }), DispatcherPriority.Normal);
		}

		private void CallPopped(IEnumerable<T> items)
		{
			if (Application.Current.Dispatcher == null) { Popped(items); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Popped(items); }
			else Application.Current.Dispatcher.Invoke(() => Popped(items), DispatcherPriority.Normal);
		}

		private void CallCleared(IEnumerable<T> items)
		{
			if (Application.Current.Dispatcher == null) { Cleared(items); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Cleared(items); }
			else Application.Current.Dispatcher.Invoke(() => Cleared(items), DispatcherPriority.Normal);
		}

		#endregion

		#region - Interface Implementations -

		public void CopyTo(T[] array, int index)
		{
			il.CopyTo(array, index);
		}

		public T[] ToArray()
		{
			return il.ToArray();
		}

		bool IProducerConsumerCollection<T>.TryAdd(T item)
		{
			Push(item);
			return true;
		}

		bool IProducerConsumerCollection<T>.TryTake(out T item)
		{
			return TryPop(out item);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return il.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((System.Collections.ICollection)il).GetEnumerator();
		}

		void System.Collections.ICollection.CopyTo(Array array, int index)
		{
			((System.Collections.ICollection)il).CopyTo(array, index);
		}

		public int Count
		{
			get { return il.Count; }
		}

		bool System.Collections.ICollection.IsSynchronized
		{
			get { return false; }
		}

		object System.Collections.ICollection.SyncRoot
		{
			get { return null; }
		}

		#endregion
	}
}