using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace System.Collections.Generic
{
	public class DependencyQueue<T> : IProducerConsumerCollection<T>
	{
		private ConcurrentQueue<T> il;
		public delegate void ChangedEventHandler(T Value);
		public event ChangedEventHandler Enqueued;
		public event ChangedEventHandler Dequeued;

		public DependencyQueue()
		{
			il = new ConcurrentQueue<T>();
		}

		public DependencyQueue(IEnumerable<T> Items)
		{
			il = new ConcurrentQueue<T>(Items);
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

		public void FromRange(IEnumerable<T> range)
		{
			System.Threading.Interlocked.Exchange(ref il, new ConcurrentQueue<T>(range));
		}

		public ConcurrentQueue<T> ToConcurrentQueue()
		{
			return new ConcurrentQueue<T>(il.ToArray());
		}

		public Queue<T> ToQueue()
		{
			return new Queue<T>(il.ToArray());
		}

		public void ClearEventHandlers()
		{
			Enqueued = null;
			Dequeued = null;
		}

		#region - Update Calls -

		private async void CallPushed(T item)
		{
			if (Window.Current.Dispatcher == null) { Enqueued(item); return; }
			if (Window.Current.Dispatcher.HasThreadAccess) { Enqueued(item); }
			else await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Enqueued(item));
		}

		private async void CallPopped(T item)
		{
			if (Window.Current.Dispatcher == null) { Dequeued(item); return; }
			if (Window.Current.Dispatcher.HasThreadAccess) { Dequeued(item); }
			else await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Dequeued(item));
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