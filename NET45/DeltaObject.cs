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

namespace System
{
	[Serializable]
	public abstract class DeltaObject
	{
		[NonSerialized, IgnoreDataMember, XmlIgnore] private ConcurrentDictionary<HashID, object> values;
		[NonSerialized, IgnoreDataMember, XmlIgnore] private ConcurrentQueue<CMDItemBase> modifications;
		[NonSerialized, IgnoreDataMember, XmlIgnore] private long ChangeCount;
		[NonSerialized, IgnoreDataMember, XmlIgnore] private long batchInterval;
		[IgnoreDataMember, XmlIgnore] public long BatchInterval { get { return batchInterval; } protected set { batchInterval = value; } }
		[NonSerialized, IgnoreDataMember, XmlIgnore] private DependencyObjectEx baseXAMLObject;
		[IgnoreDataMember, XmlIgnore] protected DependencyObjectEx BaseXAMLObject { get { return baseXAMLObject; } set { if (baseXAMLObject == null) baseXAMLObject = value; } }
		[NonSerialized, IgnoreDataMember, XmlIgnore] private int enableBatching;
		[IgnoreDataMember, XmlIgnore] protected bool EnableBatching { get { return enableBatching != 0; } set { Threading.Interlocked.Exchange(ref enableBatching, value ? 1 : 0); } }

		protected DeltaObject()
		{
			modifications = new ConcurrentQueue<CMDItemBase>();
			values = new ConcurrentDictionary<HashID, object>();
			ChangeCount = 0;
			BatchInterval = 0;
			EnableBatching = false;
			baseXAMLObject = null;
		}

		protected DeltaObject(DependencyObjectEx baseXAMLObject)
		{
			modifications = new ConcurrentQueue<CMDItemBase>();
			values = new ConcurrentDictionary<HashID, object>();
			ChangeCount = 0;
			BatchInterval = 0;
			EnableBatching = false;
			this.baseXAMLObject = baseXAMLObject;
		}

		protected DeltaObject(long BatchInterval)
		{
			modifications = new ConcurrentQueue<CMDItemBase>();
			values = new ConcurrentDictionary<HashID, object>();
			ChangeCount = 0;
			this.BatchInterval = BatchInterval;
			EnableBatching = false;
			baseXAMLObject = null;
		}

		protected DeltaObject(DependencyObjectEx baseXAMLObject, long BatchInterval)
		{
			modifications = new ConcurrentQueue<CMDItemBase>();
			values = new ConcurrentDictionary<HashID, object>();
			ChangeCount = 0;
			this.BatchInterval = BatchInterval;
			EnableBatching = false;
			this.baseXAMLObject = baseXAMLObject;
		}

		public T GetValue<T>(DeltaProperty<T> de)
		{
			object value;
			return values.TryGetValue(de.ID, out value) == false ? de.DefaultValue : (T)value;
		}

		internal object GetValue(DeltaPropertyBase de)
		{
			object value;
			return values.TryGetValue(de.ID, out value) == false ? de.defaultValue : value;
		}

		public void SetValue<T>(DeltaProperty<T> de, T value)
		{
			//Call the validator to see if this value is acceptable
			if (de.DeltaValidateValueCallback != null && !de.DeltaValidateValueCallback(this, value)) return;

			//If the new value is the default value remove this from the modified values list, otherwise add/update it.
			if (EqualityComparer<T>.Default.Equals(value, de.DefaultValue))
			{
				//Remove the value from the list, which sets it to the default value.
				object temp;
				if (!values.TryRemove(de.ID, out temp)) return;

				if (EnableBatching && BatchInterval > 0)
				{
					modifications.Enqueue(new CMDItemValue<T>(true, de.ID));
					IncrementChangeCount();
				}

				//Clear the changed event handlers
				var tt = value as DeltaCollectionBase;
				if (tt != null) tt.ClearChangedHandlers();

				//Call the property updated callback
				if (temp != null && de.DeltaPropertyUpdatedCallback != null && baseXAMLObject != null) de.DeltaPropertyUpdatedCallback(this, (T)temp, de.DefaultValue);

				//Call the property changed callback
				if (temp != null && de.DeltaPropertyChangedCallback != null && baseXAMLObject != null) de.DeltaPropertyChangedCallback(this, (T)temp, de.DefaultValue);
			}
			else
			{
				//Setup the change event handler
				var tt = value as DeltaCollectionBase;
				if (tt != null) tt.Changed += (Sender, Args) => IncrementChangeCount();

				//Update the value
				object temp = values.AddOrUpdate(de.ID, value, (p, v) => value);
				if (EnableBatching && BatchInterval > 0)
				{
					modifications.Enqueue(new CMDItemValue<T>(false, de.ID, value));
					IncrementChangeCount();
				}

				//Call the property updated callback
				if (temp != null && de.DeltaPropertyUpdatedCallback != null && baseXAMLObject != null) de.DeltaPropertyUpdatedCallback(this, (T)temp, value);

				//Call the property changed callback
				if (temp != null && de.DeltaPropertyChangedCallback != null && baseXAMLObject != null) de.DeltaPropertyChangedCallback(this, (T)temp, value);
			}
		}

		public void SetValue<T>(DeltaProperty<T> de, T value, DependencyProperty xamlProperty)
		{
			//Call the validator to see if this value is acceptable
			if (de.DeltaValidateValueCallback != null && !de.DeltaValidateValueCallback(this, value)) return;

			//If the new value is the default value remove this from the modified values list, otherwise add/update it.
			if (EqualityComparer<T>.Default.Equals(value, de.DefaultValue))
			{
				//Remove the value from the list, which sets it to the default value.
				object temp;
				if (!values.TryRemove(de.ID, out temp)) return;

				if (EnableBatching && BatchInterval > 0)
				{
					modifications.Enqueue(new CMDItemValue<T>(true, de.ID));
					IncrementChangeCount();
				}

				if (xamlProperty != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(xamlProperty, de.defaultValue);

				//Call the property updated callback
				if (temp != null && de.DeltaPropertyUpdatedCallback != null) de.DeltaPropertyUpdatedCallback(this, (T)temp, de.DefaultValue);

				//Call the property changed callback
				if (temp != null && de.DeltaPropertyChangedCallback != null) de.DeltaPropertyChangedCallback(this, (T)temp, de.DefaultValue);
			}
			else
			{
				//Update the value
				object temp = values.AddOrUpdate(de.ID, value, (p, v) => value);
				if (EnableBatching && BatchInterval > 0)
				{
					modifications.Enqueue(new CMDItemValue<T>(false, de.ID, value));
					IncrementChangeCount();
				}

				if (xamlProperty != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(xamlProperty, value);

				//Call the property updated callback
				if (temp != null && de.DeltaPropertyUpdatedCallback != null) de.DeltaPropertyUpdatedCallback(this, (T)temp, value);

				//Call the property changed callback
				if (temp != null && de.DeltaPropertyChangedCallback != null) de.DeltaPropertyChangedCallback(this, (T)temp, value);
			}
		}

		public void SetValue<T>(DeltaProperty<T> de, T value, DependencyPropertyKey xamlProperty)
		{
			//Call the validator to see if this value is acceptable
			if (de.DeltaValidateValueCallback != null && !de.DeltaValidateValueCallback(this, value)) return;

			//If the new value is the default value remove this from the modified values list, otherwise add/update it.
			if (EqualityComparer<T>.Default.Equals(value, de.DefaultValue))
			{
				//Remove the value from the list, which sets it to the default value.
				object temp;
				if (!values.TryRemove(de.ID, out temp)) return;

				if (EnableBatching && BatchInterval > 0)
				{
					modifications.Enqueue(new CMDItemValue<T>(true, de.ID));
					IncrementChangeCount();
				}

				if (xamlProperty != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(xamlProperty, de.defaultValue);

				//Call the property updated callback
				if (temp != null && de.DeltaPropertyUpdatedCallback != null) de.DeltaPropertyUpdatedCallback(this, (T)temp, de.DefaultValue);

				//Call the property changed callback
				if (temp != null && de.DeltaPropertyChangedCallback != null) de.DeltaPropertyChangedCallback(this, (T)temp, de.DefaultValue);
			}
			else
			{
				//Update the value
				object temp = values.AddOrUpdate(de.ID, value, (p, v) => value);
				if (EnableBatching && BatchInterval > 0)
				{
					modifications.Enqueue(new CMDItemValue<T>(false, de.ID, value));
					IncrementChangeCount();
				}

				if (xamlProperty != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(xamlProperty, value);

				//Call the property updated callback
				if (temp != null && de.DeltaPropertyUpdatedCallback != null) de.DeltaPropertyUpdatedCallback(this, (T)temp, value);

				//Call the property changed callback
				if (temp != null && de.DeltaPropertyChangedCallback != null) de.DeltaPropertyChangedCallback(this, (T)temp, value);
			}
		}

		public void UpdateValue<T>(DeltaProperty<T> de, T value)
		{
			//If the new value is the default value remove this from the modified values list, otherwise add/update it.
			if (Equals(value, de.DefaultValue))
			{
				//Remove the value from the list, which sets it to the default value.
				object temp;
				if (!values.TryRemove(de.ID, out temp)) return;

				//Clear the changed event handlers
				var tt = temp as DeltaCollectionBase;
				if (tt != null) tt.ClearChangedHandlers();

				//Trigger batch updates if needed
				if (EnableBatching && tt == null && BatchInterval > 0)
				{
					modifications.Enqueue(new CMDItemValue<T>(true, de.ID));
					IncrementChangeCount();
				}

				//Call the property updated callback
				if (temp != null && de.DeltaPropertyUpdatedCallback != null) de.DeltaPropertyUpdatedCallback(this, (T)temp, de.DefaultValue);
			}
			else
			{
				//Setup the change event handler
				var tt = value as DeltaCollectionBase;
				if (tt != null) tt.Changed += (Sender, Args) => IncrementChangeCount();

				//Update the values
				var temp = (T)values.AddOrUpdate(de.ID, value, (p, v) => value);

				//Trigger batch updates if needed
				if (EnableBatching && tt == null && BatchInterval > 0)
				{
					modifications.Enqueue(new CMDItemValue<T>(false, de.ID, value));
					IncrementChangeCount();
				}

				//Call the property updated callback
				if (de.DeltaPropertyUpdatedCallback != null) de.DeltaPropertyUpdatedCallback(this, temp, value);
			}
		}

		public void ClearValue<T>(DeltaProperty<T> de)
		{
			object temp;
			if (!values.TryRemove(de.ID, out temp))
			{
				modifications.Enqueue(new CMDItemValue<T>(true, de.ID));
				IncrementChangeCount();
				var tt = temp as DeltaCollectionBase;
				if (tt != null) tt.ClearChangedHandlers();
			}
			if (de.DeltaPropertyChangedCallback != null)
				de.DeltaPropertyChangedCallback(this, (T) temp, de.DefaultValue);
		}

		public void ApplyDelta<T>(CMDItemValue<T> v)
		{
			if (v == null) return;
			if (v.UseDefault)
			{
				object temp;
				values.TryRemove(v.Key, out temp);
				var de = DeltaPropertyBase.FromID(v.Key) as DeltaProperty<T>;
				if (de != null && de.DeltaPropertyUpdatedCallback != null) de.DeltaPropertyUpdatedCallback(this, (T)temp, de.DefaultValue);
			}
			else
			{
				var temp = values.AddOrUpdate(v.Key, v.Value, (p, a) => v.Value);
				var de = DeltaPropertyBase.FromID(v.Key) as DeltaProperty<T>;
				if (de != null && de.DeltaPropertyUpdatedCallback != null) de.DeltaPropertyUpdatedCallback(this, (T)temp, v.Value);
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
			//If the change notification interval is less than zero, do nothing.
			if (BatchInterval < 1) return;
			Threading.Interlocked.Increment(ref ChangeCount);

			//If the change count is greater than the interval run the batch updates.
			//Note that we don't need to use CompareExchange here because we only care if the value is greater-than-or-equal-to the batch interval, not what the exact overage is.
			if (ChangeCount <= BatchInterval) return;
			Threading.Interlocked.Exchange(ref ChangeCount, 0);
		}

		protected virtual void OnDeserializingBase(StreamingContext context)
		{
			modifications = new ConcurrentQueue<CMDItemBase>();
			values = new ConcurrentDictionary<HashID, object>();
			ChangeCount = 0;
			BatchInterval = 0;
		}
	}
}