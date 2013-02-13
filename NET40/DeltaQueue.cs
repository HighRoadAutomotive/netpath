/******************************************************************************
*	Copyright 2013 Prospective Software Inc.
*	Licensed under the Apache License, Version 2.0 (the "License");
*	you may not use this file except in compliance with the License.
*	You may obtain a copy of the License at
*
*		http://www.apache.org/licenses/LICENSE-2.0
*
*	Unless required by applicable law or agreed to in writing, software
*	distributed under the License is distributed on an "AS IS" BASIS,
*	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*	See the License for the specific language governing permissions and
*	limitations under the License.
******************************************************************************/

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
	public class DeltaQueue<T> : IProducerConsumerCollection<T>
	{
		private ConcurrentQueue<T> il;
		[NonSerialized] private ConcurrentQueue<ChangeListItem<T>> dl;

		public delegate void ChangedEventHandler(T Value);
		public event ChangedEventHandler Enqueued;
		public event ChangedEventHandler Dequeued;

		public DeltaQueue()
		{
			il = new ConcurrentQueue<T>();
			dl = new ConcurrentQueue<ChangeListItem<T>>();
		}

		public DeltaQueue(IEnumerable<T> Items)
		{
			il = new ConcurrentQueue<T>(Items);
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

		public void Enqueue(T Item)
		{
			EnqueueNoUpdate(Item);
			Enqueued(Item);
		}

		public void EnqueueNoUpdate(T Item)
		{
			il.Enqueue(Item);
			dl.Enqueue(new ChangeListItem<T>(ListItemChangeMode.Add, Item));
		}

		public bool TryPeek(out T Result)
		{
			return il.TryPeek(out Result);
		}

		public bool TryDequeue(out T Result)
		{
			bool t = il.TryDequeue(out Result);
			dl.Enqueue(new ChangeListItem<T>(ListItemChangeMode.Remove, Result));
			Dequeued(Result);
			return t;
		}

		public bool TryDequeueNoUpdate(out T Result)
		{
			bool t = il.TryDequeue(out Result);
			dl.Enqueue(new ChangeListItem<T>(ListItemChangeMode.Remove, Result));
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