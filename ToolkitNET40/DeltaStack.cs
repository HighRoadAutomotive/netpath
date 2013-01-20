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
	public class DeltaStack<T> : IProducerConsumerCollection<T>
	{
		private readonly ConcurrentStack<T> il;
		private readonly Action<IEnumerable<T>> Pushed;
		private readonly Action<IEnumerable<T>> Popped;
		private readonly Action<int> Cleared;

		public DeltaStack(Action<IEnumerable<T>> Pushed, Action<IEnumerable<T>> Popped, Action<int> Cleared)
		{
			il = new ConcurrentStack<T>();
			this.Pushed = Pushed ?? (Item => { });
			this.Popped = Popped ?? ((Item) => { });
			this.Cleared = Cleared ?? (count => { });
		}

		public DeltaStack(IEnumerable<T> Items, Action<IEnumerable<T>> Pushed, Action<IEnumerable<T>> Popped, Action<int> Cleared)
		{
			il = new ConcurrentStack<T>(Items);
			this.Pushed = Pushed ?? (Item => { });
			this.Popped = Popped ?? ((Item) => { });
			this.Cleared = Cleared ?? (count => { });
		}

		public void Clear()
		{
			int t = il.Count;
			T[] tl = il.ToArray();
			il.Clear();
			CallCleared(t, tl);
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