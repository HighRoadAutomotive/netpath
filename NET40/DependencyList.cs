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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace System.Collections.Generic
{
	[Serializable]
	public class DependencyList<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		private List<T> il;
		[NonSerialized] private readonly ReaderWriterLockSlim ocl;
		public delegate void AddRemoveClearedEventHandler(IEnumerable<T> Values);
		public event AddRemoveClearedEventHandler Added;
		public event AddRemoveClearedEventHandler Removed;
		public event AddRemoveClearedEventHandler Cleared;
		public delegate void InsertRemoveAtEventHandler(int Index, IEnumerable<T> Values);
		public event InsertRemoveAtEventHandler RemovedAt;
		public event InsertRemoveAtEventHandler Inserted;
		public delegate void MovedEventHandler(T Value, int NewIndex);
		public event MovedEventHandler Moved;
		public delegate void ReplacedEventHandler(T OldValue, T NewValue);
		public event ReplacedEventHandler Replaced;

		~DependencyList()
		{
			try
			{
				ocl.Dispose();
			}
			catch { }
		}

		public DependencyList()
		{
			il = new List<T>();
			ocl = new ReaderWriterLockSlim();
		}

		public DependencyList(int Capacity)
		{
			il = new List<T>(Capacity);
			ocl = new ReaderWriterLockSlim();
		}

		public DependencyList(IEnumerable<T> Items)
		{
			il = new List<T>(Items);
			ocl = new ReaderWriterLockSlim();
		}

		public void Lock()
		{
			ocl.EnterWriteLock();
		}

		public void Unlock()
		{
			ocl.ExitWriteLock();
		}

		public void ClearEventHandlers()
		{
			Added = null;
			Removed = null;
			Cleared = null;
			RemovedAt = null;
			Inserted = null;
			Moved = null;
			Replaced = null;
		}

		public void Add(T item)
		{
			ocl.EnterWriteLock();
			try
			{
				il.Add(item);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallAdded(item);
		}

		public void AddRange(IEnumerable<T> items)
		{
			ocl.EnterWriteLock();
			try
			{
				il.AddRange(items);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallAdded(items);
		}

		public void AddNoUpdate(T item)
		{
			ocl.EnterWriteLock();
			try
			{
				il.Add(item);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallAdded(item, false);
		}

		public void AddRangeNoUpdate(IEnumerable<T> items)
		{
			ocl.EnterWriteLock();
			try
			{
				il.AddRange(items);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallAdded(items, false);
		}

		public int BinarySearch(T item)
		{
			ocl.EnterReadLock();
			try
			{
				return il.BinarySearch(item);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int BinarySearch(T item, IComparer<T> comparer)
		{
			ocl.EnterReadLock();
			try
			{
				return il.BinarySearch(item, comparer);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int BinarySearch(int start, int count, T item, IComparer<T> comparer)
		{
			ocl.EnterReadLock();
			try
			{
				return il.BinarySearch(start, count, item, comparer);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public void Clear()
		{
			T[] tl = ToArray();
			ocl.EnterWriteLock();
			try
			{
				il.Clear();
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallCleared(tl);
		}

		public void ClearNoUpdate()
		{
			T[] tl;
			ocl.EnterWriteLock();
			try
			{
				tl = ToArray();
				il.Clear();
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallCleared(tl, false);
		}

		public bool Contains(T item)
		{
			ocl.EnterReadLock();
			try
			{
				return il.Contains(item);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public void CopyTo(T[] array)
		{
			ocl.EnterReadLock();
			try
			{
				il.CopyTo(array);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			ocl.EnterReadLock();
			try
			{
				il.CopyTo(array, arrayIndex);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public void CopyTo(int index, T[] array, int arrayIndex, int count)
		{
			ocl.EnterReadLock();
			try
			{
				il.CopyTo(index, array, arrayIndex, count);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int Count
		{
			get
			{
				ocl.EnterReadLock();
				try
				{
					return il.Count;
				}
				finally
				{
					ocl.ExitReadLock();
				}
			}
		}

		public bool Exists(Predicate<T> match)
		{
			ocl.EnterReadLock();
			try
			{
				return il.Exists(match);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public T Find(Predicate<T> match)
		{
			ocl.EnterReadLock();
			try
			{
				return il.Find(match);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public IEnumerable<T> FindAll(Predicate<T> match)
		{
			ocl.EnterReadLock();
			try
			{
				return il.FindAll(match);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public T FindLast(Predicate<T> match)
		{
			ocl.EnterReadLock();
			try
			{
				return il.FindLast(match);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int FindIndex(Predicate<T> match)
		{
			ocl.EnterReadLock();
			try
			{
				return il.FindIndex(match);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int FindIndex(int start, Predicate<T> match)
		{
			ocl.EnterReadLock();
			try
			{
				return il.FindIndex(start, match);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int FindIndex(int start, int count, Predicate<T> match)
		{
			ocl.EnterReadLock();
			try
			{
				return il.FindIndex(start, count, match);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int FindLastIndex(Predicate<T> match)
		{
			ocl.EnterReadLock();
			try
			{
				return il.FindLastIndex(match);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int FindLastIndex(int start, Predicate<T> match)
		{
			ocl.EnterReadLock();
			try
			{
				return il.FindLastIndex(start, match);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int FindLastIndex(int start, int count, Predicate<T> match)
		{
			ocl.EnterReadLock();
			try
			{
				return il.FindLastIndex(start, count, match);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public IEnumerable<T> GetRange(int start, int count)
		{
			ocl.EnterReadLock();
			try
			{
				return il.GetRange(start, count);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int IndexOf(T item)
		{
			ocl.EnterReadLock();
			try
			{
				return il.IndexOf(item);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int IndexOf(T item, int start)
		{
			ocl.EnterReadLock();
			try
			{
				return il.IndexOf(item, start);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int IndexOf(T item, int start, int count)
		{
			ocl.EnterReadLock();
			try
			{
				return il.IndexOf(item, start, count);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public void Insert(int index, T item)
		{
			ocl.EnterWriteLock();
			try
			{
				il.Insert(index, item);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallInserted(index, item);
		}

		public void InsertRange(int index, IEnumerable<T> items)
		{
			ocl.EnterWriteLock();
			try
			{
				il.InsertRange(index, items);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallInserted(index, items);
		}

		public void InsertNoUpdate(int index, T item)
		{
			ocl.EnterWriteLock();
			try
			{
				il.Insert(index, item);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallInserted(index, item, false);
		}

		public void InsertRangeNoUpdate(int index, IEnumerable<T> items)
		{
			ocl.EnterWriteLock();
			try
			{
				il.InsertRange(index, items);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallInserted(index, items, false);
		}

		public int LastIndexOf(T item)
		{
			ocl.EnterReadLock();
			try
			{
				return il.LastIndexOf(item);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int LastIndexOf(T item, int start)
		{
			ocl.EnterReadLock();
			try
			{
				return il.LastIndexOf(item, start);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int LastIndexOf(T item, int start, int count)
		{
			ocl.EnterReadLock();
			try
			{
				return il.LastIndexOf(item, start, count);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public void Move(int oldindex, int newindex)
		{
			T ti = this[oldindex];
			ocl.EnterWriteLock();
			try
			{
				il.Insert(newindex, ti);
				il.Remove(ti);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallMoved(ti, newindex);
		}

		public bool Remove(T item)
		{
			ocl.EnterWriteLock();
			bool rt;
			try
			{
				rt = il.Remove(item);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallRemoved(item);
			return rt;
		}

		public void RemoveAt(int index)
		{
			T ti = this[index];
			ocl.EnterWriteLock();
			try
			{
				il.RemoveAt(index);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallRemovedAt(index, ti);
		}

		public void RemoveRange(int index, int count)
		{
			T[] tl = GetRange(index, count).ToArray();
			ocl.EnterWriteLock();
			try
			{
				il.RemoveRange(index, count);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallRemoved(tl);
		}

		public void MoveNoUpdate(T value, int newindex)
		{
			ocl.EnterWriteLock();
			try
			{
				int oldindex = il.IndexOf(value);
				il.Insert(newindex, value);
				il.RemoveAt(oldindex);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallMoved(value, newindex, false);
		}

		public bool RemoveNoUpdate(T item)
		{
			ocl.EnterWriteLock();
			bool rt;
			try
			{
				rt = il.Remove(item);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			if (rt) CallRemoved(item, false);
			return rt;
		}

		public void RemoveAtNoUpdate(int index)
		{
			T ti = default(T);
			ocl.EnterWriteLock();
			try
			{
				ti = il[index];
				il.RemoveAt(index);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallRemovedAt(index, ti, false);
		}

		public void RemoveRangeNoUpdate(int index, int count)
		{
			T[] tl;
			ocl.EnterWriteLock();
			try
			{
				tl = GetRange(index, count).ToArray();
				il.RemoveRange(index, count);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallRemoved(tl, false);
		}

		public void ReplaceNoUpdate(T NewValue, T OldValue)
		{
			ocl.EnterWriteLock();
			try
			{
				for (int i = 0; i < il.Count; i++)
					if (EqualityComparer<T>.Default.Equals(il[i], OldValue)) il[i] = NewValue;
			}
			finally
			{
				ocl.ExitWriteLock();
			}
			CallReplaced(OldValue, NewValue, false);
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

		public void TrimExcess()
		{
			ocl.EnterWriteLock();
			try
			{
				il.TrimExcess();
			}
			finally
			{
				ocl.ExitWriteLock();
			}
		}

		public bool TrueForAll(Predicate<T> match)
		{
			ocl.EnterReadLock();
			try
			{
				return il.TrueForAll(match);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public void FromList(IEnumerable<T> range)
		{
			ocl.EnterWriteLock();
			try
			{
				System.Threading.Interlocked.Exchange(ref il, new List<T>(range));
			}
			finally
			{
				ocl.ExitWriteLock();
			}
		}

		public List<T> ToList()
		{
			ocl.EnterReadLock();
			try
			{
				return new List<T>(il);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public void ClearEventFunctions()
		{
			Added = null;
			Removed = null;
			Cleared = null;
			RemovedAt = null;
			Inserted = null;
			Moved = null;
			Replaced = null;
		}

		#region - Call Functions -

		private void CallAdded(T item, bool Update = true)
		{
			if (Application.Current.Dispatcher == null) { if (Added != null && Update) Added(new List<T> { item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item)); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { if (Added != null && Update) Added(new List<T> { item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item)); }
			else Application.Current.Dispatcher.Invoke(new Action(() => { if (Added != null && Update) Added(new List<T> { item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item)); }), DispatcherPriority.Normal);
		}

		private void CallAdded(IEnumerable<T> items, bool Update = true)
		{
			if (Application.Current.Dispatcher == null) { if (Added != null && Update) Added(items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(items))); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { if (Added != null && Update) Added(items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(items))); }
			else Application.Current.Dispatcher.Invoke(new Action(() => { if (Added != null && Update) Added(items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(items))); }), DispatcherPriority.Normal);
		}

		private void CallInserted(int index, T item, bool Update = true)
		{
			if (Application.Current.Dispatcher == null) { if (Inserted != null && Update) Inserted(index, new List<T> { item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index)); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { if (Inserted != null && Update) Inserted(index, new List<T> { item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index)); }
			else Application.Current.Dispatcher.Invoke(new Action(() => { if (Inserted != null && Update) Inserted(index, new List<T> { item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index)); }), DispatcherPriority.Normal);
		}

		private void CallInserted(int index, IEnumerable<T> items, bool Update = true)
		{
			if (Application.Current.Dispatcher == null) { if (Inserted != null && Update) Inserted(index, items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(items), index)); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { if (Inserted != null && Update) Inserted(index, items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(items), index)); }
			else Application.Current.Dispatcher.Invoke(new Action(() => { if (Inserted != null && Update) Inserted(index, items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(items), index)); }), DispatcherPriority.Normal);
		}

		private void CallRemoved(T item, bool Update = true)
		{
			if (Application.Current.Dispatcher == null) { if (Removed != null && Update) Removed(new List<T> { item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item)); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { if (Removed != null && Update) Removed(new List<T> { item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item)); }
			else Application.Current.Dispatcher.Invoke(new Action(() => { if (Removed != null && Update) Removed(new List<T> { item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item)); }), DispatcherPriority.Normal);
		}

		private void CallRemoved(IEnumerable<T> items, bool Update = true)
		{
			if (Application.Current.Dispatcher == null) { if (Removed != null && Update) Removed(items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<T>(items))); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { if (Removed != null && Update) Removed(items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<T>(items))); }
			else Application.Current.Dispatcher.Invoke(new Action(() => { if (Removed != null && Update) Removed(items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<T>(items))); }), DispatcherPriority.Normal);
		}

		private void CallRemovedAt(int index, T item, bool Update = true)
		{
			if (Application.Current.Dispatcher == null) { if (RemovedAt != null && Update) RemovedAt(index, new List<T> { item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index)); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { if (RemovedAt != null && Update) RemovedAt(index, new List<T> { item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index)); }
			else Application.Current.Dispatcher.Invoke(new Action(() => { if (RemovedAt != null && Update) RemovedAt(index, new List<T> { item }); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index)); }), DispatcherPriority.Normal);
		}

		private void CallCleared(IEnumerable<T> items, bool Update = true)
		{
			if (Application.Current.Dispatcher == null) { if (Cleared != null && Update) Cleared(items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, new List<T>(items))); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { if (Cleared != null && Update) Cleared(items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, new List<T>(items))); }
			else Application.Current.Dispatcher.Invoke((new Action(() => { if (Cleared != null && Update) Cleared(items); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, new List<T>(items))); })), DispatcherPriority.Normal);
		}

		private void CallMoved(T item, int newindex, bool Update = true)
		{
			if (Application.Current.Dispatcher == null) { if (Moved != null && Update) Moved(item, newindex); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newindex)); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { if (Moved != null && Update) Moved(item, newindex); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newindex)); }
			else Application.Current.Dispatcher.Invoke(new Action(() => { if (Moved != null && Update) Moved(item, newindex); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newindex)); }), DispatcherPriority.Normal);
		}

		private void CallReplaced(T olditem, T newitem, bool Update = true)
		{
			if (Application.Current.Dispatcher == null) { if (Replaced != null && Update) Replaced(olditem, newitem); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newitem, olditem)); return; }
			if (Application.Current.Dispatcher.CheckAccess()) { if (Replaced != null && Update) Replaced(olditem, newitem); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newitem, olditem)); }
			else Application.Current.Dispatcher.Invoke(new Action(() => { if (Replaced != null && Update) Replaced(olditem, newitem); if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newitem, olditem)); }), DispatcherPriority.Normal);
		}

		#endregion

		#region - Interface Implementations -

		public T this[int index]
		{
			get
			{
				ocl.EnterReadLock();
				try
				{
					return il[index];
				}
				finally
				{
					ocl.ExitReadLock();
				}
			}
			set
			{
				T ti = this[index];
				ocl.EnterWriteLock();
				try
				{
					il[index] = value;
				}
				finally
				{
					ocl.ExitWriteLock();
				}
				CallReplaced(ti, value);
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
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
				return ((IEnumerable)il).GetEnumerator();
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string PropertyName = "")
		{
			if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
		}

		#endregion
	}
}