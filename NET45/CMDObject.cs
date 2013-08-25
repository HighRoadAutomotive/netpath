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
		[NonSerialized, IgnoreDataMember, XmlIgnore] private ConcurrentDictionary<HashID, object> values;
		[NonSerialized, IgnoreDataMember, XmlIgnore] private DependencyObjectEx baseXAMLObject;
		[IgnoreDataMember, XmlIgnore] public DependencyObjectEx BaseXAMLObject { get { return baseXAMLObject; } set { if (baseXAMLObject == null) baseXAMLObject = value; } }

		protected CMDObject()
		{
			values = new ConcurrentDictionary<HashID, object>();
			BaseXAMLObject = null;
		}

		protected CMDObject(DependencyObjectEx baseXAMLObject)
		{
			values = new ConcurrentDictionary<HashID, object>();
			BaseXAMLObject = baseXAMLObject;
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
			if (de.CMDValidateValueCallback != null && !de.CMDValidateValueCallback(this, value)) return;

			//If the new value is the default value remove this from the modified values list, otherwise add/update it.
			if (EqualityComparer<T>.Default.Equals(value, de.DefaultValue))
			{
				//Remove the value from the list, which sets it to the default value.
				object temp;
				if (!values.TryRemove(de.ID, out temp)) return;

				if (de.XAMLProperty != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(de.XAMLProperty, value);
				if (de.XAMLPropertyKey != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(de.XAMLPropertyKey, value);

				//Call the property changed callback
				if (temp != null && de.CMDPropertyChangedCallback != null) de.CMDPropertyChangedCallback(this, (T)temp, de.DefaultValue);
			}
			else
			{
				//Update the value
				object temp = values.AddOrUpdate(de.ID, value, (p, v) => value);

				if (de.XAMLProperty != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(de.XAMLProperty, value);
				if (de.XAMLPropertyKey != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(de.XAMLPropertyKey, value);

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

				if (de.XAMLProperty != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(de.XAMLProperty, value);
				if (de.XAMLPropertyKey != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(de.XAMLPropertyKey, value);
			}
			else
			{
				//Update the values
				var temp = (T)values.AddOrUpdate(de.ID, value, (p, v) => value);

				if (de.XAMLProperty != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(de.XAMLProperty, value);
				if (de.XAMLPropertyKey != null && baseXAMLObject != null) baseXAMLObject.UpdateValueThreaded(de.XAMLPropertyKey, value);
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