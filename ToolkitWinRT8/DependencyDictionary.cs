using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace System.Collections.Generic
{
	public class DependencyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private ConcurrentDictionary<TKey, TValue> il;
		public delegate void AddRemoveEventHandler(TKey Key, TValue Value);
		public event AddRemoveEventHandler Added;
		public event AddRemoveEventHandler Removed;
		public delegate void ClearedEventHandler(KeyValuePair<TKey, TValue>[] Values);
		public event ClearedEventHandler Cleared;
		public delegate void UpdatedRemoveEventHandler(TKey Key, TValue NewValue, TValue OldValue);
		public event UpdatedRemoveEventHandler Updated;

		public DependencyDictionary()
		{
			il = new ConcurrentDictionary<TKey, TValue>();
		}

		public DependencyDictionary(int ConcurrencyLevel, int Capacity)
		{
			il = new ConcurrentDictionary<TKey, TValue>(ConcurrencyLevel, Capacity);
		}

		public DependencyDictionary(IEnumerable<KeyValuePair<TKey, TValue>> Items)
		{
			il = new ConcurrentDictionary<TKey, TValue>(Items);
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
			KeyValuePair<TKey, TValue>[] c = il.ToArray();
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

		private async void CallAdded(TKey key, TValue value)
		{
			if (Window.Current.Dispatcher == null) { Added(key, value); return; }
			if (Window.Current.Dispatcher.HasThreadAccess) Added(key, value);
			else await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Added(key, value));
		}

		private async void CallRemoved(TKey key, TValue value)
		{
			if (Window.Current.Dispatcher == null) { Removed(key, value); return; }
			if (Window.Current.Dispatcher.HasThreadAccess) Removed(key, value);
			else await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Removed(key, value));
		}

		private async void CallCleared(KeyValuePair<TKey, TValue>[] values)
		{
			if (Window.Current.Dispatcher == null) { Cleared(values); return; }
			if (Window.Current.Dispatcher.HasThreadAccess) Cleared(values);
			else await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Cleared(values));
		}

		private async void CallUpdated(TKey index, TValue olditem, TValue newitem)
		{
			if (Window.Current.Dispatcher == null) { Updated(index, olditem, newitem); return; }
			if (Window.Current.Dispatcher.HasThreadAccess) Updated(index, olditem, newitem);
			else await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Updated(index, olditem, newitem));
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