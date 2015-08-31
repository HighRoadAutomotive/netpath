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
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace System.Collections.Generic
{
	class DependencyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private readonly Dictionary<TKey, TValue> il;
		private readonly ReaderWriterLock ocl;
		private readonly Action<KeyValuePair<TKey, TValue>> Added;
		private readonly Action<KeyValuePair<TKey, TValue>> Removed;
		private readonly Action<int> Cleared;
		private readonly Action<DependencyDictionaryUpdatedArgs> Updated;

		public class DependencyDictionaryUpdatedArgs
		{
			public TValue OldValue { get; private set; }
			public TValue NewValue { get; private set; }
			public TKey Key { get; private set; }

			private DependencyDictionaryUpdatedArgs() { }

			public DependencyDictionaryUpdatedArgs(TKey Key, TValue NewValue, TValue OldValue)
			{
				this.OldValue = OldValue;
				this.NewValue = NewValue;
				this.Key = Key;
			}
		}

		public DependencyDictionary()
		{
			il = new Dictionary<TKey, TValue>();
			ocl = new ReaderWriterLock();
			Added = (items => { });
			Removed = (items => { });
			Cleared = (count => { });
			Updated = (ReplacedArgs => { });
		}

		public DependencyDictionary(int Capacity)
		{
			il = new Dictionary<TKey, TValue>(Capacity);
			ocl = new ReaderWriterLock();
			Added = (items => { });
			Removed = (items => { });
			Cleared = (count => { });
			Updated = (ReplacedArgs => { });
		}

		public DependencyDictionary(IDictionary<TKey, TValue> Items)
		{
			il = new Dictionary<TKey, TValue>(Items);
			ocl = new ReaderWriterLock();
			Added = (items => { });
			Removed = (items => { });
			Cleared = (count => { });
			Updated = (ReplacedArgs => { });
		}

		public DependencyDictionary(Action<KeyValuePair<TKey, TValue>> Added, Action<KeyValuePair<TKey, TValue>> Removed, Action<int> Cleared, Action<DependencyDictionaryUpdatedArgs> Updated)
		{
			il = new Dictionary<TKey, TValue>();
			ocl = new ReaderWriterLock();
			this.Added = Added ?? (items => { });
			this.Removed = Removed ?? (items => { });
			this.Cleared = Cleared ?? (count => { });
			this.Updated = Updated ?? (ReplacedArgs => { });
		}

		public DependencyDictionary(int Capacity, Action<KeyValuePair<TKey, TValue>> Added, Action<KeyValuePair<TKey, TValue>> Removed, Action<int> Cleared, Action<DependencyDictionaryUpdatedArgs> Updated)
		{
			il = new Dictionary<TKey, TValue>(Capacity);
			ocl = new ReaderWriterLock();
			this.Added = Added ?? (items => { });
			this.Removed = Removed ?? (items => { });
			this.Cleared = Cleared ?? (count => { });
			this.Updated = Updated ?? (ReplacedArgs => { });
		}

		public DependencyDictionary(IDictionary<TKey, TValue> Items, Action<KeyValuePair<TKey, TValue>> Added, Action<KeyValuePair<TKey, TValue>> Removed, Action<int> Cleared, Action<DependencyDictionaryUpdatedArgs> Updated)
		{
			il = new Dictionary<TKey, TValue>(Items);
			ocl = new ReaderWriterLock();
			this.Added = Added ?? (items => { });
			this.Removed = Removed ?? (items => { });
			this.Cleared = Cleared ?? (count => { });
			this.Updated = Updated ?? (ReplacedArgs => { });
		}

		public void Add(TKey key, TValue value)
		{
			ocl.AcquireWriterLock(0);
			try
			{
				il.Add(key, value);
			}
			finally
			{
				ocl.ReleaseWriterLock();
			}
			CallAdded(key, value);
		}

		public bool ContainsKey(TKey key)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.ContainsKey(key);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public bool ContainsValue(TValue value)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.ContainsValue(value);
			}
			finally 
			{
				ocl.ReleaseReaderLock();
			}
		}

		public ICollection<TKey> Keys
		{
			get
			{
				ocl.AcquireReaderLock(0);
				try
				{
					return il.Keys;
				}
				finally
				{
					ocl.ReleaseReaderLock();
				}
			}
		}

		public bool Remove(TKey key)
		{
			TKey k = key;
			TValue v = il[key];
			ocl.AcquireWriterLock(0);
			bool rt;
			try
			{
				rt = il.Remove(key);
			}
			finally
			{
				ocl.ReleaseWriterLock();
			}
			CallRemoved(k, v);
			return rt;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.TryGetValue(key, out value);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
				ocl.AcquireReaderLock(0);
				try
				{
					return il.Values;
				}
				finally
				{
					ocl.ReleaseReaderLock();
				}
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				ocl.AcquireReaderLock(0);
				try
				{
					return il[key];
				}
				finally
				{
					ocl.ReleaseReaderLock();
				}
			}
			set
			{
				TValue ov = il[key];
				ocl.AcquireWriterLock(0);
				try
				{
					il[key] = value;
				}
				finally
				{
					ocl.ReleaseWriterLock();
				}
				CallUpdated(key, ov, value);
			}
		}

		public void Clear()
		{
			int c = Count;
			ocl.AcquireWriterLock(0);
			try
			{
				il.Clear();
			}
			finally
			{
				ocl.ReleaseWriterLock();
			}
			CallCleared(c);
		}

		public int Count
		{
			get
			{
				ocl.AcquireReaderLock(0);
				try
				{
					return il.Count;
				}
				finally 
				{
					ocl.ReleaseReaderLock();
				}
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		private void CallAdded(TKey key, TValue value)
		{
			if (Application.Current.Dispatcher == null) { Added(new KeyValuePair<TKey, TValue>(key, value)); return; }
			if (Application.Current.Dispatcher.CheckAccess()) Added(new KeyValuePair<TKey, TValue>(key, value));
			else Application.Current.Dispatcher.Invoke(new Action<KeyValuePair<TKey, TValue>>(c => Added(new KeyValuePair<TKey, TValue>(key, value))), DispatcherPriority.Normal);
		}

		private void CallRemoved(TKey key, TValue value)
		{
			if (Application.Current.Dispatcher == null) { Removed(new KeyValuePair<TKey, TValue>(key, value)); return; }
			if (Application.Current.Dispatcher.CheckAccess()) Removed(new KeyValuePair<TKey, TValue>(key, value));
			else Application.Current.Dispatcher.Invoke(new Action<KeyValuePair<TKey, TValue>>(c => Removed(new KeyValuePair<TKey, TValue>(key, value))), DispatcherPriority.Normal);
		}

		private void CallCleared(int count)
		{
			if (Application.Current.Dispatcher == null) { Cleared(count); return; }
			if (Application.Current.Dispatcher.CheckAccess()) Cleared(count);
			else Application.Current.Dispatcher.Invoke(new Action<int>(c => Cleared(count)), DispatcherPriority.Normal);
		}

		private void CallUpdated(TKey index, TValue olditem, TValue newitem)
		{
			if (Application.Current.Dispatcher == null) { Updated(new DependencyDictionaryUpdatedArgs(index, olditem, newitem)); return; }
			if (Application.Current.Dispatcher.CheckAccess()) Updated(new DependencyDictionaryUpdatedArgs(index, olditem, newitem));
			else Application.Current.Dispatcher.Invoke(new Action<DependencyDictionaryUpdatedArgs>(c => Updated(new DependencyDictionaryUpdatedArgs(index, olditem, newitem))), DispatcherPriority.Normal);
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return il.GetEnumerator();
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			ocl.AcquireReaderLock(0);
			try
			{
				return ((IEnumerable)il).GetEnumerator();
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			return ContainsKey(item.Key);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			ocl.AcquireReaderLock(0);
			try
			{
				((ICollection<KeyValuePair<TKey, TValue>>)il).CopyTo(array, arrayIndex);
			}
			finally
			{
				ocl.ReleaseReaderLock();
			}
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			return Remove(item.Key);
		}
	}
}