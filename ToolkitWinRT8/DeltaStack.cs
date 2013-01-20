using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace NETPath.Toolkit.WinRT8
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
			this.Pushed = Pushed ?? (ItemList => { });
			this.Popped = Popped ?? ((ItemList) => { });
			this.Cleared = Cleared ?? (count => { });
		}

		public DeltaStack(IEnumerable<T> Items, Action<IEnumerable<T>> Pushed, Action<IEnumerable<T>> Popped, Action<int> Cleared)
		{
			il = new ConcurrentStack<T>(Items);
			this.Pushed = Pushed ?? (ItemList => { });
			this.Popped = Popped ?? ((ItemList) => { });
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

		#region - Update Calls -

		private async void CallPushed(T item)
		{
			if (Window.Current.Dispatcher == null) { Pushed(new List<T> { item }); return; }
			if (Window.Current.Dispatcher.HasThreadAccess) { Pushed(new List<T> { item }); }
			else await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Pushed(new List<T> { item }));
		}

		private async void CallPushed(IEnumerable<T> items)
		{
			if (Window.Current.Dispatcher == null) { Pushed(items); return; }
			if (Window.Current.Dispatcher.HasThreadAccess) { Pushed(items); }
			else await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Pushed(items));
		}

		private async void CallPopped(T item)
		{
			if (Window.Current.Dispatcher == null) { Popped(new List<T> { item }); return; }
			if (Window.Current.Dispatcher.HasThreadAccess) { Popped(new List<T> { item }); }
			else await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Popped(new List<T> { item }));
		}

		private async void CallPopped(IEnumerable<T> items)
		{
			if (Window.Current.Dispatcher == null) { Popped(items); return; }
			if (Window.Current.Dispatcher.HasThreadAccess) { Popped(items); }
			else await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Popped(items));
		}

		private async void CallCleared(int count, IEnumerable<T> items)
		{
			if (Window.Current.Dispatcher == null) { Cleared(count); return; }
			if (Window.Current.Dispatcher.HasThreadAccess) { Cleared(count); }
			else await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Cleared(count));
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