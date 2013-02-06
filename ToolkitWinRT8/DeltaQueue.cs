using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace System.Collections.Generic
{
	public class DeltaQueue<T> : IProducerConsumerCollection<T>
	{
		private ConcurrentQueue<T> il;

		public delegate void ChangedEventHandler(T Value);
		public event ChangedEventHandler Enqueued;
		public event ChangedEventHandler Dequeued;

		public DeltaQueue()
		{
			il = new ConcurrentQueue<T>();
		}

		public DeltaQueue(IEnumerable<T> Items)
		{
			il = new ConcurrentQueue<T>(Items);
		}

		public void Enqueue(T Item)
		{
			il.Enqueue(Item);
			Enqueued(Item);
		}

		public bool TryPeek(out T Result)
		{
			return il.TryPeek(out Result);
		}

		public bool TryDequeue(out T Result)
		{
			bool t = il.TryDequeue(out Result);
			Dequeued(Result);
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