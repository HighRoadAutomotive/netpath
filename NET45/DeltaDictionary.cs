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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace System.Collections.Generic
{
	[Serializable]
	[HostProtectionAttribute(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
	public class DeltaDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		public delegate void AddRemoveEventHandler(TKey Key, TValue Value);
		public event AddRemoveEventHandler Added;
		public event AddRemoveEventHandler Removed;
		public delegate void ClearedEventHandler(KeyValuePair<TKey, TValue>[] Values);
		public event ClearedEventHandler Cleared;
		public delegate void UpdatedRemoveEventHandler(TKey Key, TValue NewValue, TValue OldValue);
		public event UpdatedRemoveEventHandler Updated;
		public delegate void SendChangesHandler(IEnumerable<ChangeDictionaryItem<TKey, TValue>> delta);
		public event SendChangesHandler SendChanges;

		[NonSerialized, IgnoreDataMember, XmlIgnore] private ConcurrentDictionary<TKey, TValue> il;
		[NonSerialized, IgnoreDataMember, XmlIgnore] private ConcurrentQueue<ChangeDictionaryItem<TKey, TValue>> dl;
		[NonSerialized, IgnoreDataMember, XmlIgnore] private long changeCount;
		[NonSerialized, IgnoreDataMember, XmlIgnore] private long batchInterval;
		[IgnoreDataMember, XmlIgnore] public long BatchInterval { get { return batchInterval; } protected set { Interlocked.Exchange(ref batchInterval, value); } }

		public DeltaDictionary(long BatchInterval = 0)
		{
			il = new ConcurrentDictionary<TKey, TValue>();
			dl = new ConcurrentQueue<ChangeDictionaryItem<TKey, TValue>>();
			this.BatchInterval = BatchInterval;
		}

		private DeltaDictionary(long BatchInterval, int ConcurrencyLevel, int Capacity)
		{
			il = new ConcurrentDictionary<TKey, TValue>(ConcurrencyLevel, Capacity);
			dl = new ConcurrentQueue<ChangeDictionaryItem<TKey, TValue>>();
			this.BatchInterval = BatchInterval;
		}

		private DeltaDictionary(long BatchInterval, IEnumerable<KeyValuePair<TKey, TValue>> Items)
		{
			il = new ConcurrentDictionary<TKey, TValue>(Items);
			dl = new ConcurrentQueue<ChangeDictionaryItem<TKey, TValue>>();
			this.BatchInterval = BatchInterval;
		}

		public void ApplyDelta(ChangeDictionaryItem<TKey, TValue> delta)
		{
			switch (delta.Mode)
			{
				case ListItemChangeMode.Add: il.AddOrUpdate(delta.Key, delta.Value, (p,v) => delta.Value); break;
				case ListItemChangeMode.Remove: TValue t; il.TryRemove(delta.Key, out t); break;
				case ListItemChangeMode.Replace: il.AddOrUpdate(delta.Key, delta.Value, (p, v) => delta.Value); break;
			}
		}

		public void ApplyDelta(IEnumerable<ChangeDictionaryItem<TKey, TValue>> delta)
		{
			foreach (var item in delta)
			{
				switch (item.Mode)
				{
					case ListItemChangeMode.Add: il.AddOrUpdate(item.Key, item.Value, (p, v) => item.Value); break;
					case ListItemChangeMode.Remove: TValue t; il.TryRemove(item.Key, out t); break;
					case ListItemChangeMode.Replace: il.AddOrUpdate(item.Key, item.Value, (p, v) => item.Value); break;
				}
			}
		}

		public IEnumerable<ChangeDictionaryItem<TKey, TValue>> PeekDelta()
		{
			return dl.ToArray();
		}

		public IEnumerable<ChangeDictionaryItem<TKey, TValue>> GetDelta()
		{
			var tdl = new List<ChangeDictionaryItem<TKey, TValue>>();
			ChangeDictionaryItem<TKey, TValue> td;
			while(dl.TryDequeue(out td))
				tdl.Add(td);
			return tdl.Count > 0 ? tdl : null;
		}

		public void ClearDelta()
		{
			Threading.Interlocked.Exchange(ref dl, new ConcurrentQueue<ChangeDictionaryItem<TKey, TValue>>());
		}

		private void EnqueueChange(ChangeDictionaryItem<TKey, TValue> change)
		{
			if (BatchInterval < 1) return;
			if (BatchInterval == 1) SendChanges(new[] {change});
			else
			{
				dl.Enqueue(change);
				Interlocked.Increment(ref changeCount);

				if (changeCount < BatchInterval) return;

				Task.Factory.StartNew(() =>
				{
					var tdl = new List<ChangeDictionaryItem<TKey, TValue>>();
					ChangeDictionaryItem<TKey, TValue> td;
					while (dl.TryDequeue(out td))
						tdl.Add(td);
					Interlocked.Exchange(ref changeCount, 0);

					try { SendChanges(tdl); }
					catch { }
				}, TaskCreationOptions.None);
			}
		}

		public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
		{
			TValue ov = default(TValue);
			bool update = TryGetValue(key, out ov);
			TValue rt = il.AddOrUpdate(key, addValueFactory, updateValueFactory);
			if (update) { EnqueueChange(new ChangeDictionaryItem<TKey, TValue>(ListItemChangeMode.Replace, key, rt)); if (Updated != null) Updated(key, ov, rt); }
			else { EnqueueChange(new ChangeDictionaryItem<TKey, TValue>(ListItemChangeMode.Add, key, rt)); if (Added != null) Added(key, rt); }
			return rt;
		}

		public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
		{
			TValue ov = default(TValue);
			bool update = TryGetValue(key, out ov);
			TValue rt = il.AddOrUpdate(key, addValue, updateValueFactory);
			if (update) { EnqueueChange(new ChangeDictionaryItem<TKey, TValue>(ListItemChangeMode.Replace, key, rt)); if (Updated != null) Updated(key, ov, rt); }
			else { EnqueueChange(new ChangeDictionaryItem<TKey, TValue>(ListItemChangeMode.Add, key, rt)); if (Added != null) Added(key, rt); }
			return rt;
		}

		public TValue AddOrUpdateNoUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
		{
			TValue ov = default(TValue);
			bool update = TryGetValue(key, out ov);
			TValue rt = il.AddOrUpdate(key, addValueFactory, updateValueFactory);
			if (update) { EnqueueChange(new ChangeDictionaryItem<TKey, TValue>(ListItemChangeMode.Replace, key, rt)); }
			else { EnqueueChange(new ChangeDictionaryItem<TKey, TValue>(ListItemChangeMode.Add, key, rt)); }
			return rt;
		}

		public TValue AddOrUpdateNoUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
		{
			TValue ov = default(TValue);
			bool update = TryGetValue(key, out ov);
			TValue rt = il.AddOrUpdate(key, addValue, updateValueFactory);
			if (update) { EnqueueChange(new ChangeDictionaryItem<TKey, TValue>(ListItemChangeMode.Replace, key, rt)); }
			else { EnqueueChange(new ChangeDictionaryItem<TKey, TValue>(ListItemChangeMode.Add, key, rt)); }
			return rt;
		}

		public void Clear()
		{
			KeyValuePair<TKey, TValue>[] tl = il.ToArray();
			il.Clear();
			foreach (var t in tl)
				EnqueueChange(new ChangeDictionaryItem<TKey, TValue>(ListItemChangeMode.Remove, t.Key, t.Value));
			Cleared(tl);
		}

		public void ClearNoUpdate()
		{
			KeyValuePair<TKey, TValue>[] tl = il.ToArray();
			il.Clear();
			foreach (var t in tl)
				EnqueueChange(new ChangeDictionaryItem<TKey, TValue>(ListItemChangeMode.Remove, t.Key, t.Value));
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
			if (add) { EnqueueChange(new ChangeDictionaryItem<TKey, TValue>(ListItemChangeMode.Add, key, rt)); if (Added != null) Added(key, rt); }
			return rt;
		}

		public TValue GetOrAdd(TKey key, TValue value)
		{
			bool add = !ContainsKey(key);
			TValue rt = il.GetOrAdd(key, value);
			if (add) { EnqueueChange(new ChangeDictionaryItem<TKey, TValue>(ListItemChangeMode.Add, key, rt)); if (Added != null) Added(key, rt); }
			return rt;
		}

		public TValue GetOrAddNoUpdate(TKey key, Func<TKey, TValue> valueFactory)
		{
			bool add = !ContainsKey(key);
			TValue rt = il.GetOrAdd(key, valueFactory);
			if (add) { EnqueueChange(new ChangeDictionaryItem<TKey, TValue>(ListItemChangeMode.Add, key, rt)); }
			return rt;
		}

		public TValue GetOrAddNoUpdate(TKey key, TValue value)
		{
			bool add = !ContainsKey(key);
			TValue rt = il.GetOrAdd(key, value);
			if (add) { EnqueueChange(new ChangeDictionaryItem<TKey, TValue>(ListItemChangeMode.Add, key, rt)); }
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
			if (rt) { EnqueueChange(new ChangeDictionaryItem<TKey, TValue>(ListItemChangeMode.Add, Key, Value)); if (Added != null) Added(Key, Value); }
			return rt;
		}

		public bool TryAddNoUpdate(TKey Key, TValue Value)
		{
			bool rt = il.TryAdd(Key, Value);
			if (rt) { EnqueueChange(new ChangeDictionaryItem<TKey, TValue>(ListItemChangeMode.Add, Key, Value)); }
			return rt;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return il.TryGetValue(key, out value);
		}

		public bool TryRemove(TKey Key, out TValue Value)
		{
			bool rt = il.TryRemove(Key, out Value);
			if (rt) { EnqueueChange(new ChangeDictionaryItem<TKey, TValue>(ListItemChangeMode.Remove, Key, Value)); if (Removed != null) Removed(Key, Value); }
			return rt;
		}

		public bool TryRemoveNoUpdate(TKey Key, out TValue Value)
		{
			bool rt = il.TryRemove(Key, out Value);
			if (rt) { EnqueueChange(new ChangeDictionaryItem<TKey, TValue>(ListItemChangeMode.Remove, Key, Value)); }
			return rt;
		}

		public bool TryUpdate(TKey Key, TValue Value, TValue Comparison)
		{
			TValue ov;
			if (!il.TryGetValue(Key, out ov)) return false;
			bool rt = il.TryUpdate(Key, Value, Comparison);
			if (rt) { EnqueueChange(new ChangeDictionaryItem<TKey, TValue>(ListItemChangeMode.Replace, Key, Value)); if (Updated != null) Updated(Key, ov, Value); }
			return rt;
		}

		public bool TryUpdateNoUpdate(TKey Key, TValue Value, TValue Comparison)
		{
			TValue ov;
			if (!il.TryGetValue(Key, out ov)) return false;
			bool rt = il.TryUpdate(Key, Value, Comparison);
			if (rt) { EnqueueChange(new ChangeDictionaryItem<TKey, TValue>(ListItemChangeMode.Replace, Key, Value)); }
			return rt;
		}

		public TValue this[TKey key]
		{
			get { return il[key]; }
			set { TValue ov = il[key]; il[key] = value; if (Updated != null) Updated(key, ov, value); }
		}

		public KeyValuePair<TKey, TValue>[] ToArray()
		{
			return il.ToArray();
		}

		public void SetEvents(AddRemoveEventHandler Added, AddRemoveEventHandler Removed, ClearedEventHandler Cleared, UpdatedRemoveEventHandler Updated, SendChangesHandler SendChanges)
		{
			this.Added = Added;
			this.Removed = Removed;
			this.Cleared = Cleared;
			this.Updated = Updated;
			this.SendChanges = SendChanges;
		}

		public void ClearEvents()
		{
			Added = null;
			Removed = null;
			Cleared = null;
			Updated = null;
			SendChanges = null;
		}

		#region - Interface Implementation -

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

		#endregion
	}
}