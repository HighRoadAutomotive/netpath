using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace System.Collections.Generic
{
	[Serializable]
	public class DeltaStack<T> : IProducerConsumerCollection<T>
	{
		private ConcurrentStack<T> il;
		[NonSerialized] private ConcurrentQueue<ChangeListItem<T>> dl;

		public delegate void ChangedEventHandler(IEnumerable<T> Value);
		public event ChangedEventHandler Pushed;
		public event ChangedEventHandler Popped;
		public event ChangedEventHandler Cleared;

		public DeltaStack()
		{
			il = new ConcurrentStack<T>();
			dl = new ConcurrentQueue<ChangeListItem<T>>();
		}

		public DeltaStack(IEnumerable<T> Items)
		{
			il = new ConcurrentStack<T>(Items);
			dl = new ConcurrentQueue<ChangeListItem<T>>();
		}

		public IEnumerable<ChangeListItem<T>> PeekDelta()
		{
			return dl.ToArray();
		}

		public IEnumerable<ChangeListItem<T>> GetDelta()
		{
			var tdl = new List<ChangeListItem<T>>();
			ChangeListItem<T> td;
			while (dl.TryDequeue(out td))
				tdl.Add(td);
			return tdl;
		}

		public void ClearDelta()
		{
			Threading.Interlocked.Exchange(ref dl, new ConcurrentQueue<ChangeListItem<T>>());
		}

		public void Clear()
		{
			T[] tl = il.ToArray();
			il.Clear();
			foreach (var t in tl) dl.Enqueue(new ChangeListItem<T>(ListItemChangeMode.Remove, t));
			Cleared(tl);
		}

		public void ClearNoUpdate()
		{
			T[] tl = il.ToArray();
			il.Clear();
			foreach (var t in tl) dl.Enqueue(new ChangeListItem<T>(ListItemChangeMode.Remove, t));
			Cleared(tl);
		}

		public void Push(T Item)
		{
			PushNoUpdate(Item);
			Pushed(new[] { Item });
		}
		
		public void PushNoUpdate(T Item)
		{
			il.Push(Item);
			dl.Enqueue(new ChangeListItem<T>(ListItemChangeMode.Add, Item));
		}

		public void PushRange(T[] Items)
		{
			PushRangeNoUpdate(Items);
			Pushed(Items);
		}

		public void PushRangeNoUpdate(T[] Items)
		{
			il.PushRange(Items);
			foreach (var t in Items) dl.Enqueue(new ChangeListItem<T>(ListItemChangeMode.Add, t));
		}

		public void PushRange(T[] Items, int Start, int Count)
		{
			PushRangeNoUpdate(Items, Start, Count);
			Pushed(Items);
		}

		public void PushRangeNoUpdate(T[] Items, int Start, int Count)
		{
			il.PushRange(Items, Start, Count);
			foreach (var t in Items) dl.Enqueue(new ChangeListItem<T>(ListItemChangeMode.Add, t));
		}

		public bool TryPeek(out T Result)
		{
			return il.TryPeek(out Result);
		}

		public bool TryPop(out T Result)
		{
			bool t = il.TryPop(out Result);
			dl.Enqueue(new ChangeListItem<T>(ListItemChangeMode.Remove, Result));
			Popped(new[] { Result });
			return t;
		}

		public bool TryPopNoUpdate(out T Result)
		{
			bool t = il.TryPop(out Result);
			dl.Enqueue(new ChangeListItem<T>(ListItemChangeMode.Remove, Result));
			return t;
		}

		public int TryPopRange(T[] Items)
		{
			int t = il.TryPopRange(Items);
			foreach (var x in Items) dl.Enqueue(new ChangeListItem<T>(ListItemChangeMode.Remove, x));
			Popped(Items);
			return t;
		}

		public int TryPopRangeNoUpdate(T[] Items)
		{
			int t = il.TryPopRange(Items);
			foreach (var x in Items) dl.Enqueue(new ChangeListItem<T>(ListItemChangeMode.Remove, x));
			return t;
		}

		public int TryPopRange(T[] Items, int Start, int Count)
		{
			int t = il.TryPopRange(Items, Start, Count);
			foreach (var x in Items) dl.Enqueue(new ChangeListItem<T>(ListItemChangeMode.Remove, x));
			Popped(Items);
			return t;
		}

		public int TryPopRangeNoUpdate(T[] Items, int Start, int Count)
		{
			int t = il.TryPopRange(Items, Start, Count);
			foreach (var x in Items) dl.Enqueue(new ChangeListItem<T>(ListItemChangeMode.Remove, x));
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