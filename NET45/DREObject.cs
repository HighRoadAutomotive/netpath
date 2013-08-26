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
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Xml.Serialization;
using ProtoBuf;

namespace System
{
	[Serializable]
	[DataContract]
	[ProtoBuf.ProtoContract]
	public abstract class DREObjectBase
	{
		[NonSerialized, IgnoreDataMember, XmlIgnore] private ConcurrentDictionary<HashID, object> values;
		[NonSerialized, IgnoreDataMember, XmlIgnore] private ConcurrentQueue<CMDItemBase> modifications;
		[NonSerialized, IgnoreDataMember, XmlIgnore] private long changeCount;
		[IgnoreDataMember, XmlIgnore] protected long ChangeCount { get { return changeCount; } }
		[NonSerialized, IgnoreDataMember, XmlIgnore] private long batchInterval;
		[IgnoreDataMember, XmlIgnore] public long BatchInterval { get { return batchInterval; } protected set { batchInterval = value; } }
		[NonSerialized, IgnoreDataMember, XmlIgnore] private DependencyObjectEx baseXAMLObject; 
		[IgnoreDataMember, XmlIgnore] protected DependencyObjectEx BaseXAMLObject { get { return baseXAMLObject; } set { if (baseXAMLObject == null) baseXAMLObject = value; } }

		protected DREObjectBase()
		{
			modifications = new ConcurrentQueue<CMDItemBase>();
			values = new ConcurrentDictionary<HashID, object>();
			changeCount = 0;
			BatchInterval = 0;
			baseXAMLObject = null;
		}

		protected DREObjectBase(DependencyObjectEx baseXAMLObject)
		{
			modifications = new ConcurrentQueue<CMDItemBase>();
			values = new ConcurrentDictionary<HashID, object>();
			changeCount = 0;
			BatchInterval = 0;
			this.baseXAMLObject = baseXAMLObject;
		}

		protected DREObjectBase(long BatchInterval)
		{
			modifications = new ConcurrentQueue<CMDItemBase>();
			values = new ConcurrentDictionary<HashID, object>();
			changeCount = 0;
			this.BatchInterval = BatchInterval;
			baseXAMLObject = null;
		}

		protected DREObjectBase(DependencyObjectEx baseXAMLObject, long BatchInterval)
		{
			modifications = new ConcurrentQueue<CMDItemBase>();
			values = new ConcurrentDictionary<HashID, object>();
			changeCount = 0;
			this.BatchInterval = BatchInterval;
			this.baseXAMLObject = baseXAMLObject;
		}

		public T GetValue<T>(DREProperty<T> de)
		{
			object value;
			return values.TryGetValue(de.ID, out value) == false ? de.DefaultValue : (T)value;
		}

		internal object GetValue(CMDPropertyBase de)
		{
			object value;
			return values.TryGetValue(de.ID, out value) == false ? de.defaultValue : value;
		}

		public void SetValue<T>(DREProperty<T> de, T value)
		{
			//Call the validator to see if this value is acceptable
			if (de.DeltaValidateValueCallback != null && !de.DeltaValidateValueCallback(this, value)) return;

			//If the new value is the default value remove this from the modified values list, otherwise add/update it.
			if (EqualityComparer<T>.Default.Equals(value, de.DefaultValue))
			{
				//Remove the value from the list, which sets it to the default value.
				object temp;
				if (!values.TryRemove(de.ID, out temp)) return;
				if (de.EnableBatching && BatchInterval > 0)
				{
					modifications.Enqueue(new CMDItemValue<T>(true, de.ID));
					IncrementChangeCount();
				}

				//Clear the changed event handlers
				if (de.IsDictionary || de.IsList)
				{
					var tt = temp as DeltaCollectionBase;
					if (tt != null) tt.ClearChangedHandlers();
				}

				if (de.XAMLProperty != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(de.XAMLProperty, de.defaultValue);
				if (de.XAMLPropertyKey != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(de.XAMLPropertyKey, de.defaultValue);

				//Call the property updated callback
				if (temp != null && de.DREPropertyUpdatedCallback != null && baseXAMLObject != null) de.DREPropertyUpdatedCallback(this, (T)temp, de.DefaultValue);

				//Call the property changed callback
				if (temp != null && de.DREPropertyChangedCallback != null && baseXAMLObject != null) de.DREPropertyChangedCallback(this, (T)temp, de.DefaultValue);
			}
			else
			{
				//Setup the change event handler
				if (de.IsDictionary || de.IsList)
				{
					var tt = value as DeltaCollectionBase;
					if (tt != null) tt.Changed += (Sender, Args) => IncrementChangeCount();
				}

				//Update the value
				object temp = values.AddOrUpdate(de.ID, value, (p, v) => value);
				if (de.EnableBatching && BatchInterval > 0)
				{
					modifications.Enqueue(new CMDItemValue<T>(false, de.ID, value));
					IncrementChangeCount();
				}

				if (de.XAMLProperty != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(de.XAMLProperty, value);
				if (de.XAMLPropertyKey != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(de.XAMLPropertyKey, value);

				//Call the property updated callback
				if (temp != null && de.DREPropertyUpdatedCallback != null && baseXAMLObject != null) de.DREPropertyUpdatedCallback(this, (T)temp, value);

				//Call the property changed callback
				if (temp != null && de.DREPropertyChangedCallback != null && baseXAMLObject != null) de.DREPropertyChangedCallback(this, (T)temp, value);
			}
		}

		public void UpdateValueExternal<T>(DREProperty<T> de, T value)
		{
			//If the new value is the default value remove this from the modified values list, otherwise add/update it.
			if (Equals(value, de.DefaultValue))
			{
				//Remove the value from the list, which sets it to the default value.
				object temp;
				if (!values.TryRemove(de.ID, out temp)) return;

				//Clear the changed event handlers
				if (de.IsDictionary || de.IsList)
				{
					var tt = temp as DeltaCollectionBase;
					if (tt != null) tt.ClearChangedHandlers();
				}
			}
			else
			{
				//Setup the change event handler
				if (de.IsDictionary || de.IsList)
				{
					var tt = value as DeltaCollectionBase;
					if (tt != null) tt.Changed += (Sender, Args) => IncrementChangeCount();
				}

				//Update the values
				var temp = (T)values.AddOrUpdate(de.ID, value, (p, v) => value);
			}
			if (de.XAMLProperty != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(de.XAMLProperty, value);
			if (de.XAMLPropertyKey != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(de.XAMLPropertyKey, value);
		}

		public void UpdateValueInternal<T>(DREProperty<T> de, T value)
		{
			//If the new value is the default value remove this from the modified values list, otherwise add/update it.
			if (Equals(value, de.DefaultValue))
			{
				//Remove the value from the list, which sets it to the default value.
				object temp;
				if (!values.TryRemove(de.ID, out temp)) return;
				if (de.EnableBatching && BatchInterval > 0)
				{
					modifications.Enqueue(new CMDItemValue<T>(true, de.ID));
					IncrementChangeCount();
				}

				//Clear the changed event handlers
				if (de.IsDictionary || de.IsList)
				{
					var tt = temp as DeltaCollectionBase;
					if (tt != null) tt.ClearChangedHandlers();
				}

				//Call the property updated callback
				if (temp != null && de.DREPropertyUpdatedCallback != null && baseXAMLObject != null) de.DREPropertyUpdatedCallback(this, (T)temp, value);
			}
			else
			{
				//Setup the change event handler
				if (de.IsDictionary || de.IsList)
				{
					var tt = value as DeltaCollectionBase;
					if (tt != null) tt.Changed += (Sender, Args) => IncrementChangeCount();
				}

				//Update the values
				var temp = (T)values.AddOrUpdate(de.ID, value, (p, v) => value);
				if (de.EnableBatching && BatchInterval > 0)
				{
					modifications.Enqueue(new CMDItemValue<T>(false, de.ID, value));
					IncrementChangeCount();
				}

				//Call the property updated callback
				if (temp != null && de.DREPropertyUpdatedCallback != null && baseXAMLObject != null) de.DREPropertyUpdatedCallback(this, (T)temp, value);
			}
		}

		public void ClearValue<T>(DREProperty<T> de)
		{
			object temp;
			if (!values.TryRemove(de.ID, out temp))
			{
				if (de.EnableBatching && BatchInterval > 0)
				{
					modifications.Enqueue(new CMDItemValue<T>(false, de.ID));
					IncrementChangeCount();
				}
				//Clear the changed event handlers
				if (de.IsDictionary || de.IsList)
				{
					var tt = temp as DeltaCollectionBase;
					if (tt != null) tt.ClearChangedHandlers();
				}
			}
			if (de.DREPropertyChangedCallback != null)
				de.DREPropertyChangedCallback(this, (T) temp, de.DefaultValue);
		}

		public void ApplyDelta<T>(CMDItemValue<T> v)
		{
			if (v == null) return;
			if (v.UseDefault)
			{
				object temp;
				values.TryRemove(v.Key, out temp);
				var de = CMDPropertyBase.FromID(v.Key) as DREProperty<T>;
				if (de != null && de.XAMLProperty != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(de.XAMLProperty, de.DefaultValue);
				if (de != null && de.XAMLPropertyKey != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(de.XAMLPropertyKey, de.DefaultValue);
			}
			else
			{
				var temp = values.AddOrUpdate(v.Key, v.Value, (p, a) => v.Value);
				var de = CMDPropertyBase.FromID(v.Key) as DREProperty<T>;
				if (de != null && de.XAMLProperty != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(de.XAMLProperty, v.Value);
				if (de != null && de.XAMLPropertyKey != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(de.XAMLPropertyKey, v.Value);
			}
		}

		public Dictionary<HashID, object> GetNonDefaultValues()
		{
			return values.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		}

		public IEnumerable<CMDItemBase> GetDeltaValues()
		{
			var til = new List<CMDItemBase>();
			CMDItemBase t;
			while (modifications.TryDequeue(out t))
				if (t != null) til.Add(t);
			return til.GroupBy(a => a.Key).Select(a => a.Last()).ToArray();
		}

		protected virtual void IncrementChangeCount()
		{
			if (ChangeCount >= (BatchInterval - 1)) BatchUpdates();

			//If the change notification interval is less than zero, do nothing.
			if (BatchInterval < 1) return;
			Threading.Interlocked.Increment(ref changeCount);

			//If the change count is greater than the interval run the batch updates.
			//Note that we don't need to use CompareExchange here because we only care if the value is greater-than-or-equal-to the batch interval, not what the exact overage is.
			if (ChangeCount < BatchInterval) return;
			Threading.Interlocked.Exchange(ref changeCount, 0);
		}

		protected virtual void OnDeserializingBase(StreamingContext context)
		{
			modifications = new ConcurrentQueue<CMDItemBase>();
			values = new ConcurrentDictionary<HashID, object>();
			changeCount = 0;
			BatchInterval = 0;
		}

		[DataMember] [ProtoMember(1)] public Guid _DREID { get; set; }

		protected void SetDREID(string PrimaryKey) { if (_DREID == Guid.Empty) _DREID = HashID.GenerateHashID(PrimaryKey).ToGUID(); }
		protected void SetDREID(byte PrimaryKey) { if (_DREID == Guid.Empty) _DREID = HashID.GenerateHashID(new byte[] { PrimaryKey }).ToGUID(); }
		protected void SetDREID(sbyte PrimaryKey) { if (_DREID == Guid.Empty) _DREID = HashID.GenerateHashID(BitConverter.GetBytes(PrimaryKey)).ToGUID(); }
		protected void SetDREID(short PrimaryKey) { if (_DREID == Guid.Empty) _DREID = HashID.GenerateHashID(BitConverter.GetBytes(PrimaryKey)).ToGUID(); }
		protected void SetDREID(int PrimaryKey) { if (_DREID == Guid.Empty) _DREID = HashID.GenerateHashID(BitConverter.GetBytes(PrimaryKey)).ToGUID(); }
		protected void SetDREID(long PrimaryKey) { if (_DREID == Guid.Empty) _DREID = HashID.GenerateHashID(BitConverter.GetBytes(PrimaryKey)).ToGUID(); }
		protected void SetDREID(ushort PrimaryKey) { if (_DREID == Guid.Empty) _DREID = HashID.GenerateHashID(BitConverter.GetBytes(PrimaryKey)).ToGUID(); }
		protected void SetDREID(uint PrimaryKey) { if (_DREID == Guid.Empty) _DREID = HashID.GenerateHashID(BitConverter.GetBytes(PrimaryKey)).ToGUID(); }
		protected void SetDREID(ulong PrimaryKey) { if (_DREID == Guid.Empty) _DREID = HashID.GenerateHashID(BitConverter.GetBytes(PrimaryKey)).ToGUID(); }
		protected void SetDREID(Guid PrimaryKey) { if (_DREID == Guid.Empty) _DREID = HashID.GenerateHashID(PrimaryKey.ToByteArray()).ToGUID(); }

		protected abstract void BatchUpdates();
	}

	[DataContract]
	[ProtoBuf.ProtoContract]
	public abstract class DREObject<T> : DREObjectBase where T : DREObject<T>
	{
		[NonSerialized, IgnoreDataMember, XmlIgnore] private static readonly System.Collections.Concurrent.ConcurrentDictionary<Guid, T> __dcm;
		static DREObject()
		{
			__dcm = new System.Collections.Concurrent.ConcurrentDictionary<Guid, T>();
		}

		public static T GetDataFromID(Guid ID)
		{
			T t;
			__dcm.TryGetValue(ID, out t);
			return t;
		}

		public static void UpdateValue<TType>(Guid ID, DREProperty<TType> prop, TType value)
		{
			T t;
			__dcm.TryGetValue(ID, out t);
			if (t != null) t.UpdateValueExternal(prop, value);
		}

		public static bool HasData(Guid DataID)
		{
			return __dcm.ContainsKey(DataID);
		}

		public static T RegisterData(Guid ClientID, T Data)
		{
			Data.__crl.GetOrAdd(ClientID, Data._DREID);
			return __dcm.GetOrAdd(Data._DREID, Data);
		}

		public static T RegisterData(Guid ClientID, Guid DataID)
		{
			T Data;
			__dcm.TryGetValue(DataID, out Data);
			if (Data == null) return null;
			Data.__crl.GetOrAdd(ClientID, Data._DREID);
			return __dcm.GetOrAdd(Data._DREID, Data);
		}

		public static bool UnregisterData(Guid ClientID, Guid DataID)
		{
			T data;
			__dcm.TryGetValue(DataID, out data);
			if (data == null) return true;
			Guid dreid;
			data.__crl.TryRemove(ClientID, out dreid);
			return !data.__crl.IsEmpty || __dcm.TryRemove(DataID, out data);
		}

		//Constructors

		protected DREObject()
		{
			_DREID = Guid.Empty;
			__crl = new ConcurrentDictionary<Guid, Guid>();
		}

		protected DREObject(DependencyObjectEx baseXAMLObject)
			: base(baseXAMLObject)
		{
			_DREID = Guid.Empty;
			__crl = new ConcurrentDictionary<Guid, Guid>();
		}

		protected DREObject(long BatchInterval)
			: base(BatchInterval)
		{
			_DREID = Guid.Empty;
			__crl = new ConcurrentDictionary<Guid, Guid>();
		}

		protected DREObject(DependencyObjectEx baseXAMLObject, long BatchInterval)
			: base(baseXAMLObject, BatchInterval)
		{
			_DREID = Guid.Empty;
			__crl = new ConcurrentDictionary<Guid, Guid>();
		}

		protected override sealed void OnDeserializingBase(StreamingContext context)
		{
			base.OnDeserializingBase(context);
			__crl = new ConcurrentDictionary<Guid, Guid>();
		}

		[IgnoreDataMember, XmlIgnore] public IEnumerable<Guid> ClientList { get { return __crl.Keys; } }
		[NonSerialized, IgnoreDataMember, XmlIgnore] private ConcurrentDictionary<Guid, Guid> __crl = new ConcurrentDictionary<Guid, Guid>();
	}
}