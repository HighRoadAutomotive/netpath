using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace DeltaV
{
	[Serializable]
	public abstract class DeltaObject
	{
		[NonSerialized] private readonly ConcurrentDictionary<HashID, Modifiable> values;
		[NonSerialized] internal readonly HashID ID;

		private class Modifiable
		{
			private int isModified;
			public bool IsModified { get { return System.Threading.Interlocked.CompareExchange(ref isModified, 0, 0) != 0; } set { System.Threading.Interlocked.Exchange(ref isModified, value ? 1 : 0); } }
			private object data;
			public object Data { get { return data; } }

			public Modifiable(object Data)
			{
				isModified = 1;
				data = Data;
			}
		}

		protected DeltaObject()
		{
			values = new ConcurrentDictionary<HashID, Modifiable>();
			ID = new HashID();
		}

		internal DeltaObject(HashID ID)
		{
			values = new ConcurrentDictionary<HashID, Modifiable>();
			this.ID = ID;
		}

		internal DeltaObject(Guid ID)
		{
			values = new ConcurrentDictionary<HashID, Modifiable>();
			this.ID = HashID.GenerateHashID(ID.ToByteArray());
		}

		public T GetValue<T>(DeltaProperty<T> de)
		{
			Modifiable value;
			return values.TryGetValue(de.ID, out value) == false ? de.DefaultValue : (T)value.Data;
		}

		internal object GetValue(DeltaPropertyBase de)
		{
			Modifiable value;
			return values.TryGetValue(de.ID, out value) == false ? de.defaultValue : value.Data;
		}

		public void SetValue<T>(DeltaProperty<T> de, T value)
		{
			//Call the validator to see if this value is acceptable
			if (de.DeltaValidateValueCallback != null && !de.DeltaValidateValueCallback(this, value)) return;

			//If the new value is the default value remove this from the modified values list, otherwise add/update it.
			if (EqualityComparer<T>.Default.Equals(value, de.DefaultValue))
			{
				Modifiable temp;
				values.TryRemove(de.ID, out temp);
				if (de.DeltaPropertyChangedCallback != null) de.DeltaPropertyChangedCallback(this, (T)temp.Data, de.DefaultValue);
			}
			else
			{
				Modifiable temp = values.AddOrUpdate(de.ID, new Modifiable(value), (p, v) => new Modifiable(value));
				if (de.DeltaPropertyChangedCallback != null) de.DeltaPropertyChangedCallback(this, (T)temp.Data, value);
			}
		}

		public void UpdateValue<T>(DeltaProperty<T> de, T value)
		{
			//Call the validator to see if this value is acceptable
			if (de.DeltaValidateValueCallback != null && !de.DeltaValidateValueCallback(this, value)) return;

			//If the new value is the default value remove this from the modified values list, otherwise add/update it.
			if (EqualityComparer<T>.Default.Equals(value, de.DefaultValue))
			{
				Modifiable temp;
				values.TryRemove(de.ID, out temp);
			}
			else
			{
				values.AddOrUpdate(de.ID, new Modifiable(value), (p, v) => new Modifiable(value));
			}
		}

		public void ClearValue<T>(DeltaProperty<T> de)
		{
			Modifiable temp;
			if (values.TryRemove(de.ID, out temp) && de.DeltaPropertyChangedCallback != null) de.DeltaPropertyChangedCallback(this, de.DefaultValue, (T)temp.Data);
		}

		public bool IsModified<T>(DeltaProperty<T> de)
		{
			return values[de.ID].IsModified;
		}
		public Dictionary<HashID, object> GetNonDefaultValues()
		{
			return values.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Data);
		}

		public Dictionary<HashID, object> GetDeltaValues()
		{
			KeyValuePair<HashID, Modifiable>[] il = values.ToArray();
			Dictionary<HashID, object> dl =  il.Where(kvp => kvp.Value.IsModified).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Data);
			foreach (KeyValuePair<HashID, Modifiable> kvp in il)
				kvp.Value.IsModified = false;
			return dl;
		}
	}
}