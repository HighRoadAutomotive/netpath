using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace DeltaV
{
	public abstract class DeltaObject
	{
		private static readonly ConcurrentDictionary<HashID, Type> types = new ConcurrentDictionary<HashID, Type>();
		private static readonly ConcurrentDictionary<HashID, DeltaObject> objects = new ConcurrentDictionary<HashID, DeltaObject>();

		public static bool RegisterType(Type type)
		{
			return types.TryAdd(HashID.GenerateHashID(type.FullName), type);
		}

		private readonly ConcurrentDictionary<HashID, object> values;
		private readonly ConcurrentDictionary<HashID, bool> modified;
		internal readonly HashID TypeID;
		internal readonly HashID ID;

		protected DeltaObject(Type TypeID)
		{
			values = new ConcurrentDictionary<HashID, object>();
			modified = new ConcurrentDictionary<HashID, bool>();
			this.TypeID = HashID.GenerateHashID(TypeID.FullName);
			ID = new HashID();
			objects.AddOrUpdate(ID, this, (p, v) => this);
		}

		internal DeltaObject(HashID TypeID, HashID ID)
		{
			values = new ConcurrentDictionary<HashID, object>();
			modified = new ConcurrentDictionary<HashID, bool>();
			this.TypeID = TypeID;
			this.ID = ID;
			objects.AddOrUpdate(ID, this, (p, v) => this);
		}

		~DeltaObject()
		{
			DeltaObject temp;
			objects.TryRemove(ID, out temp);
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
			//Check to see if the value is actually different
			object cmp;
			values.TryGetValue(de.ID, out cmp);
			if (EqualityComparer<T>.Default.Equals(value, (T)cmp)) return;

			//Call the validator to see if this value is acceptable
			if (de.DeltaValidateValueCallback != null && !de.DeltaValidateValueCallback(this, value)) return;

			//If the new value is the default value remove this from the modified values list, otherwise add or update it.
			if (EqualityComparer<T>.Default.Equals(value, de.DefaultValue))
			{
				object temp;
				values.TryRemove(de.ID, out temp);
				bool tm = modified.AddOrUpdate(de.ID, true, (p, v) => true);
				if (de.DeltaPropertyChangedCallback != null) de.DeltaPropertyChangedCallback(this, (T)temp, de.DefaultValue);
			}
			else
			{
				object temp = values.AddOrUpdate(de.ID, value, (p, v) => value);
				bool tm = modified.AddOrUpdate(de.ID, true, (p, v) => true);
				if (de.DeltaPropertyChangedCallback != null) de.DeltaPropertyChangedCallback(this, (T)temp, value);
			}
		}

		public void ClearValue<T>(DeltaProperty<T> de)
		{
			object temp;
			bool tm = modified.AddOrUpdate(de.ID, true, (p, v) => true);
			if (values.TryRemove(de.ID, out temp) && de.DeltaPropertyChangedCallback != null) de.DeltaPropertyChangedCallback(this, de.DefaultValue, (T)temp);
		}

		public bool IsModified<T>(DeltaProperty<T> de)
		{
			return modified.ContainsKey(de.ID);
		}

		internal bool IsModified(DeltaPropertyBase de)
		{
			return modified.ContainsKey(de.ID);		
		}

		protected bool IsModified(HashID PropertyID)
		{
			return modified.ContainsKey(PropertyID);
		}

		internal List<DeltaPropertyBase> GetModifiedProperties()
		{
			return values.Keys.Where(t => IsModified(DeltaPropertyBase.FromID(t))).Select(DeltaPropertyBase.FromID).ToList();
		}

		internal List<DeltaPropertyBase> GetNonDefaultProperties()
		{
			return values.Select(k => DeltaPropertyBase.FromID(k.Key)).ToList();
		}

		internal object GetValue(HashID de)
		{
			object value = null;
			bool tm;
			if (modified.TryRemove(de, out tm))
				return values.TryGetValue(de, out value) == false ? DeltaPropertyBase.FromID(ID).defaultValue : value;
			return value;
		}

		internal void SetValue<T>(HashID de, T value)
		{
			if (EqualityComparer<T>.Default.Equals(value, (T)DeltaPropertyBase.FromID(de).defaultValue))
			{
				object temp;
				values.TryRemove(de, out temp);
			}
			else
			{
				object temp = values.AddOrUpdate(de, value, (p, v) => { v = value; return v; });
			}
		}
	}
}