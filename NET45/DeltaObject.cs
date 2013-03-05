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
		[NonSerialized, IgnoreDataMember, XmlIgnore] private ConcurrentQueue<ChangeObjectItem> modifications;
		[NonSerialized, IgnoreDataMember, XmlIgnore] private long ChangeCount;
		[IgnoreDataMember, XmlIgnore] public long BatchInterval { get; protected set; }
		[NonSerialized, IgnoreDataMember, XmlIgnore] private DependencyObjectEx baseXAMLObject;
		[IgnoreDataMember, XmlIgnore] protected DependencyObjectEx BaseXAMLObject { get { return baseXAMLObject; } set { if (baseXAMLObject == null) baseXAMLObject = value; } }

		protected DeltaObject()
		{
			modifications = new ConcurrentQueue<ChangeObjectItem>();
			values = new ConcurrentDictionary<HashID, object>();
			ChangeCount = 0;
			BatchInterval = 0;
			baseXAMLObject = null;
		}

		protected DeltaObject(DependencyObjectEx baseXAMLObject)
		{
			modifications = new ConcurrentQueue<ChangeObjectItem>();
			values = new ConcurrentDictionary<HashID, object>();
			ChangeCount = 0;
			BatchInterval = 0;
			this.baseXAMLObject = baseXAMLObject;
		}

		protected DeltaObject(long BatchInterval)
		{
			modifications = new ConcurrentQueue<ChangeObjectItem>();
			values = new ConcurrentDictionary<HashID, object>();
			ChangeCount = 0;
			this.BatchInterval = BatchInterval;
			baseXAMLObject = null;
		}

		protected DeltaObject(DependencyObjectEx baseXAMLObject, long BatchInterval)
		{
			modifications = new ConcurrentQueue<ChangeObjectItem>();
			values = new ConcurrentDictionary<HashID, object>();
			ChangeCount = 0;
			this.BatchInterval = BatchInterval;
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
				values.TryRemove(de.ID, out temp);
				if (BatchInterval > 0)
				{
					modifications.Enqueue(new ChangeObjectItem(true, de.ID));
					IncrementChangeCount();
				}

				//Clear the changed event handlers
				var tt = value as DeltaCollectionBase;
				if (tt != null) tt.ClearChangedHandlers();

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
				if (BatchInterval > 0)
				{
					modifications.Enqueue(new ChangeObjectItem(false, de.ID, value));
					IncrementChangeCount();
				}

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
				values.TryRemove(de.ID, out temp);
				if (BatchInterval > 0)
				{
					modifications.Enqueue(new ChangeObjectItem(true, de.ID));
					IncrementChangeCount();
				}

				if (xamlProperty != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(xamlProperty, de.defaultValue);

				//Call the property changed callback
				if (temp != null && de.DeltaPropertyChangedCallback != null) de.DeltaPropertyChangedCallback(this, (T)temp, de.DefaultValue);
			}
			else
			{
				//Update the value
				object temp = values.AddOrUpdate(de.ID, value, (p, v) => value);
				if (BatchInterval > 0)
				{
					modifications.Enqueue(new ChangeObjectItem(false, de.ID, value));
					IncrementChangeCount();
				}

				if (xamlProperty != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(xamlProperty, value);

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
				values.TryRemove(de.ID, out temp);
				if (BatchInterval > 0)
				{
					modifications.Enqueue(new ChangeObjectItem(true, de.ID));
					IncrementChangeCount();
				}

				if (xamlProperty != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(xamlProperty, de.defaultValue);

				//Call the property changed callback
				if (temp != null && de.DeltaPropertyChangedCallback != null) de.DeltaPropertyChangedCallback(this, (T)temp, de.DefaultValue);
			}
			else
			{
				//Update the value
				object temp = values.AddOrUpdate(de.ID, value, (p, v) => value);
				if (BatchInterval > 0)
				{
					modifications.Enqueue(new ChangeObjectItem(false, de.ID, value));
					IncrementChangeCount();
				}

				if (xamlProperty != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(xamlProperty, value);

				//Call the property changed callback
				if (temp != null && de.DeltaPropertyChangedCallback != null) de.DeltaPropertyChangedCallback(this, (T)temp, value);
			}
		}

		public void UpdateValue<T>(DeltaPropertyBase de, T value)
		{
			//If the new value is the default value remove this from the modified values list, otherwise add/update it.
			if (Equals(value, de.defaultValue))
			{
				//Remove the value from the list, which sets it to the default value.
				object temp;
				values.TryRemove(de.ID, out temp);

				//Clear the changed event handlers
				var tt = value as DeltaCollectionBase;
				if (tt != null) tt.ClearChangedHandlers();
			}
			else
			{
				//Setup the change event handler
				var tt = value as DeltaCollectionBase;
				if (tt != null) tt.Changed += (Sender, Args) => IncrementChangeCount();

				//Update the values
				values.AddOrUpdate(de.ID, value, (p, v) => value);
			}
		}

		public void ClearValue<T>(DeltaProperty<T> de)
		{
			object temp;
			if (!values.TryRemove(de.ID, out temp))
			{
				modifications.Enqueue(new ChangeObjectItem(true, de.ID));
				IncrementChangeCount();
				var tt = temp as DeltaCollectionBase;
				if (tt != null) tt.ClearChangedHandlers();
			}
			if (de.DeltaPropertyChangedCallback != null)
				de.DeltaPropertyChangedCallback(this, (T) temp, de.DefaultValue);
		}

		public void ApplyDeltaValues(IEnumerable<ChangeObjectItem> values)
		{
			foreach (var v in values)
			{
				if (v.UseDefault)
				{
					object temp;
					this.values.TryRemove(v.Key, out temp);
				}
				else
					this.values.AddOrUpdate(v.Key, v.Value, (p, a) => v.Value);
			}
		}

		public Dictionary<HashID, object> GetNonDefaultValues()
		{
			return values.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		}

		public IEnumerable<ChangeObjectItem> GetDeltaValues()
		{
			return modifications.ToArray();
		}

		private void IncrementChangeCount()
		{
			//If the change notification interval is less than zero, do nothing.
			if (BatchInterval < 1) return;
			Threading.Interlocked.Increment(ref ChangeCount);

			//If the change count is greater than the interval run the batch updates.
			//Note that we don't need to use CompareExchange here because we only care if the value is greater-than-or-equal-to the batch interval, not what the exact overage is.
			if (ChangeCount <= BatchInterval) return;
			Threading.Interlocked.Exchange(ref ChangeCount, 0);
			BatchUpdates();
		}

		protected void OnDeserializingBase(StreamingContext context)
		{
			modifications = new ConcurrentQueue<ChangeObjectItem>();
			values = new ConcurrentDictionary<HashID, object>();
			ChangeCount = 0;
			BatchInterval = 0;
		}

		protected abstract void BatchUpdates();
	}
}