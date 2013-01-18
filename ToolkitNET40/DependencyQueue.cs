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
	public class DependencyQueue<T> : IProducerConsumerCollection<T>
	{
		private readonly ConcurrentQueue<T> il;
		private readonly Action<T> Pushed;
		private readonly Action<T> Popped;

		public DependencyQueue(Action<T> Pushed, Action<T> Popped)
		{
			il = new ConcurrentQueue<T>();
			this.Pushed = Pushed ?? (Item => { });
			this.Popped = Popped ?? ((Item) => { });
		}

		public DependencyQueue(IEnumerable<T> Items, Action<T> Pushed, Action<T> Popped)
		{
			il = new ConcurrentQueue<T>(Items);
			this.Pushed = Pushed ?? (Item => { });
			this.Popped = Popped ?? ((Item) => { });
		}

		public void Enqueue(T Item)
		{
			il.Enqueue(Item);
			CallPushed(Item);
		}

		public bool TryPeek(out T Result)
		{
			return il.TryPeek(out Result);
		}

		public bool TryDequeue(out T Result)
		{
			bool t = il.TryDequeue(out Result);
			CallPopped(Result);
			return t;
		}

		#region - Update Calls -

		private void CallPushed(T item)
		{
			if (Application.Current.Dispatcher == null) { Pushed(item); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Pushed(item); }
			else Application.Current.Dispatcher.Invoke(new Action(() => Pushed(item)), DispatcherPriority.Normal);
		}

		private void CallPopped(T item)
		{
			if (Application.Current.Dispatcher == null) { Popped(item); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { Popped(item); }
			else Application.Current.Dispatcher.Invoke(new Action(() => Popped(item)), DispatcherPriority.Normal);
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
			Enqueue(item);
			return true;
		}

		bool IProducerConsumerCollection<T>.TryTake(out T item)
		{
			return TryDequeue(out item);
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