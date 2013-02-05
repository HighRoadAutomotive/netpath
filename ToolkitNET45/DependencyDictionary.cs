using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace System.Collections.Generic
{
	[Serializable]
	[HostProtectionAttribute(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
	public class DependencyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private ConcurrentDictionary<TKey, TValue> il;
		[NonSerialized] private readonly Action<KeyValuePair<TKey, TValue>> Added;
		[NonSerialized] private readonly Action<KeyValuePair<TKey, TValue>> Removed;
		[NonSerialized] private readonly Action<int> Cleared;
		[NonSerialized] private readonly Action<TKey, TValue, TValue> Updated;

		public DependencyDictionary(Action<KeyValuePair<TKey, TValue>> Added = null, Action<KeyValuePair<TKey, TValue>> Removed = null, Action<int> Cleared = null, Action<TKey, TValue, TValue> Updated = null)
		{
			il = new ConcurrentDictionary<TKey, TValue>();
			this.Added = Added ?? (items => { });
			this.Removed = Removed ?? (items => { });
			this.Cleared = Cleared ?? (count => { });
			this.Updated = Updated ?? ((key, ov, nv) => { });
		}

		public DependencyDictionary(int ConcurrencyLevel, int Capacity, Action<KeyValuePair<TKey, TValue>> Added = null, Action<KeyValuePair<TKey, TValue>> Removed = null, Action<int> Cleared = null, Action<TKey, TValue, TValue> Updated = null)
		{
			il = new ConcurrentDictionary<TKey, TValue>(ConcurrencyLevel, Capacity);
			this.Added = Added ?? (items => { });
			this.Removed = Removed ?? (items => { });
			this.Cleared = Cleared ?? (count => { });
			this.Updated = Updated ?? ((key, ov, nv) => { });
		}

		public DependencyDictionary(IEnumerable<KeyValuePair<TKey, TValue>> Items, Action<KeyValuePair<TKey, TValue>> Added = null, Action<KeyValuePair<TKey, TValue>> Removed = null, Action<int> Cleared = null, Action<TKey, TValue, TValue> Updated = null)
		{
			il = new ConcurrentDictionary<TKey, TValue>(Items);
			this.Added = Added ?? (items => { });
			this.Removed = Removed ?? (items => { });
			this.Cleared = Cleared ?? (count => { });
			this.Updated = Updated ?? ((key, ov, nv) => { });
		}

		public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
		{
			bool update = ContainsKey(key);
			TValue ov = default(TValue);
			if (update) ov = il[key];
			TValue rt = il.AddOrUpdate(key, addValueFactory, updateValueFactory);
			if (update) CallUpdated(key, ov, rt);
			else CallAdded(key, rt);
			return rt;
		}

		public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
		{
			bool update = ContainsKey(key);
			TValue ov = default(TValue);
			if (update) ov = il[key];
			TValue rt = il.AddOrUpdate(key, addValue, updateValueFactory);
			if (update) CallUpdated(key, ov, rt);
			else CallAdded(key, rt);
			return rt;
		}

		public void Clear()
		{
			int c = Count;
			il.Clear();
			CallCleared(c);
		}

		public bool ContainsKey(TKey key)
		{
			return il.ContainsKey(key);
		}

		public int Count
		{
			get { return il.Count; }
		}

		public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
		{
			bool add = !ContainsKey(key);
			TValue rt = il.GetOrAdd(key, valueFactory);
			if (add) CallAdded(key, rt);
			return rt;
		}

		public TValue GetOrAdd(TKey key, TValue value)
		{
			bool add = !ContainsKey(key);
			TValue rt = il.GetOrAdd(key, value);
			if (add) CallAdded(key, rt);
			return rt;
		}

		public bool IsEmpty
		{
			get { return il.IsEmpty; }
		}

		public ICollection<TKey> Keys
		{
			get { return il.Keys; }
		}

		public bool TryAdd(TKey Key, TValue Value)
		{
			bool rt = il.TryAdd(Key, Value);
			if (rt) CallAdded(Key, Value);
			return rt;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return il.TryGetValue(key, out value);
		}

		public bool TryRemove(TKey Key, out TValue Value)
		{
			bool rt = il.TryRemove(Key, out Value);
			if (rt) CallRemoved(Key, Value);
			return rt;
		}

		public bool TryUpdate(TKey Key, TValue Value, TValue Comparison)
		{
			TValue ov = il[Key];
			bool rt = il.TryUpdate(Key, Value, Comparison);
			if (rt) CallUpdated(Key, ov, Value);
			return rt;
		}

		public TValue this[TKey key]
		{
			get { return il[key]; }
			set { TValue ov = il[key]; il[key] = value; CallUpdated(key, ov, value); }
		}

		public KeyValuePair<TKey, TValue>[] ToArray()
		{
			return il.ToArray();
		}

		public ICollection<TValue> Values
		{
			get { return il.Values; }
		}

		public void FromDictionary(IDictionary<TKey, TValue> dict)
		{
			System.Threading.Interlocked.Exchange(ref il, new ConcurrentDictionary<TKey, TValue>(dict));
		}

		public ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary()
		{
			return new ConcurrentDictionary<TKey, TValue>(il.ToArray());
		}

		public Dictionary<TKey, TValue> ToDictionary()
		{
			KeyValuePair<TKey, TValue>[] td = il.ToArray();
			return td.ToDictionary(k => k.Key, k => k.Value);
		}

		private void CallAdded(TKey key, TValue value)
		{
			if (Application.Current.Dispatcher == null) { Added(new KeyValuePair<TKey, TValue>(key, value)); return; }
			if (Application.Current.Dispatcher.CheckAccess()) Added(new KeyValuePair<TKey, TValue>(key, value));
			else Application.Current.Dispatcher.Invoke(() => Added(new KeyValuePair<TKey, TValue>(key, value)), DispatcherPriority.Normal);
		}

		private void CallRemoved(TKey key, TValue value)
		{
			if (Application.Current.Dispatcher == null) { Removed(new KeyValuePair<TKey, TValue>(key, value)); return; }
			if (Application.Current.Dispatcher.CheckAccess()) Removed(new KeyValuePair<TKey, TValue>(key, value));
			else Application.Current.Dispatcher.Invoke(() => Removed(new KeyValuePair<TKey, TValue>(key, value)), DispatcherPriority.Normal);
		}

		private void CallCleared(int count)
		{
			if (Application.Current.Dispatcher == null) { Cleared(count); return; }
			if (Application.Current.Dispatcher.CheckAccess()) Cleared(count);
			else Application.Current.Dispatcher.Invoke(() => Cleared(count), DispatcherPriority.Normal);
		}

		private void CallUpdated(TKey index, TValue olditem, TValue newitem)
		{
			if (Application.Current.Dispatcher == null) { Updated(index, olditem, newitem); return; }
			if (Application.Current.Dispatcher.CheckAccess()) Updated(index, olditem, newitem);
			else Application.Current.Dispatcher.Invoke(() => Updated(index, olditem, newitem), DispatcherPriority.Normal);
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return il.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((IEnumerable) il).GetEnumerator();
		}

		void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
		{
			((IDictionary<TKey, TValue>)il).Add(key, value);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			((IDictionary<TKey, TValue>)il).Add(item.Key, item.Value);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			return ContainsKey(item.Key);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<TKey, TValue>>) il).CopyTo(array, arrayIndex);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
		{
			get { return false; }
		}

		bool IDictionary<TKey, TValue>.Remove(TKey key)
		{
			return ((IDictionary<TKey, TValue>)il).Remove(key);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			return ((IDictionary<TKey, TValue>)il).Remove(item.Key);
		}
	}
}