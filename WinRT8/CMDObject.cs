using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace System
{
	public abstract class CMDObject
	{
		[IgnoreDataMember, XmlIgnore] private ConcurrentDictionary<HashID, object> values;

		protected CMDObject()
		{
			values = new ConcurrentDictionary<HashID, object>();
		}

		public T GetValue<T>(CMDProperty<T> de)
		{
			object value;
			return values.TryGetValue(de.ID, out value) == false ? de.DefaultValue : (T)value;
		}

		internal object GetValue(CMDPropertyBase de)
		{
			object value;
			return values.TryGetValue(de.ID, out value) == false ? de.defaultValue : value;
		}

		public void SetValue<T>(CMDProperty<T> de, T value)
		{
			//Call the validator to see if this value is acceptable
			if (de.DeltaValidateValueCallback != null && !de.DeltaValidateValueCallback(this, value)) return;

			//If the new value is the default value remove this from the modified values list, otherwise add/update it.
			if (EqualityComparer<T>.Default.Equals(value, de.DefaultValue))
			{
				//Remove the value from the list, which sets it to the default value.
				object temp;
				if (!values.TryRemove(de.ID, out temp)) return;

				//Call the property updated callback
				if (temp != null && de.CMDPropertyUpdatedCallback != null) de.CMDPropertyUpdatedCallback(this, (T)temp, de.DefaultValue);

				//Call the property changed callback
				if (temp != null && de.CMDPropertyChangedCallback != null) de.CMDPropertyChangedCallback(this, (T)temp, de.DefaultValue);
			}
			else
			{
				//Update the value
				object temp = values.AddOrUpdate(de.ID, value, (p, v) => value);

				//Call the property updated callback
				if (temp != null && de.CMDPropertyUpdatedCallback != null) de.CMDPropertyUpdatedCallback(this, (T)temp, value);

				//Call the property changed callback
				if (temp != null && de.CMDPropertyChangedCallback != null) de.CMDPropertyChangedCallback(this, (T)temp, value);
			}
		}

		public void UpdateValue<T>(CMDProperty<T> de, T value)
		{
			//If the new value is the default value remove this from the modified values list, otherwise add/update it.
			if (Equals(value, de.DefaultValue))
			{
				//Remove the value from the list, which sets it to the default value.
				object temp;
				if (!values.TryRemove(de.ID, out temp)) return;

				//Call the property updated callback
				if (temp != null && de.CMDPropertyUpdatedCallback != null) de.CMDPropertyUpdatedCallback(this, (T)temp, de.DefaultValue);
			}
			else
			{
				//Update the values
				var temp = (T)values.AddOrUpdate(de.ID, value, (p, v) => value);

				//Call the property updated callback
				if (de.CMDPropertyUpdatedCallback != null) de.CMDPropertyUpdatedCallback(this, temp, value);
			}
		}

		public void ClearValue<T>(CMDProperty<T> de)
		{
			object temp;
			values.TryRemove(de.ID, out temp);
			if (de.CMDPropertyChangedCallback != null)
				de.CMDPropertyChangedCallback(this, (T) temp, de.DefaultValue);
		}

		protected virtual void OnDeserializingBase(StreamingContext context)
		{
			values = new ConcurrentDictionary<HashID, object>();
		}
	}
}