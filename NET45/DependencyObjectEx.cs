using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace System.Windows
{
	public class DependencyObjectEx : DependencyObject
	{
		private ConcurrentDictionary<int, object> values;

		public DependencyObjectEx()
		{
			values = new ConcurrentDictionary<int, object>();
		}

		public object GetValueThreaded(DependencyProperty dp)
		{
			if (Application.Current.Dispatcher.CheckAccess()) return GetValue(dp);
			object ret = null;
			Application.Current.Dispatcher.Invoke(() => { ret = GetValue(dp); }, DispatcherPriority.Normal);
			return ret;
		}
		
		public void SetValueThreaded(DependencyProperty dp, object value)
		{
			if(Application.Current.Dispatcher.CheckAccess()) SetValue(dp, value);
			Application.Current.Dispatcher.Invoke(() => SetValue(dp, value), DispatcherPriority.Normal);
		}

		public void SetValueThreaded(DependencyPropertyKey dp, object value)
		{
			if (Application.Current.Dispatcher.CheckAccess()) SetValue(dp, value);
			Application.Current.Dispatcher.Invoke(() => SetValue(dp, value), DispatcherPriority.Normal);
		}

		public object GetValueExternal<T>(DependencyExternal<T> de)
		{
			if (de.IsUnset)
				throw new ArgumentException(string.Format("External value '{0}' in type '{1}' is unset. You must set a value before accessing the property.", de.Name, de.OwnerType));

			object value;
			return values.TryGetValue(de.GetHashCode(), out value) == false ? de.DefaultValue : value;
		}
		
		public void SetValueExternal<T>(DependencyExternal<T> de, T value)
		{
			if (de.ExternalValidateValueCallback != null && !de.ExternalValidateValueCallback(this, value)) return;

			if (EqualityComparer<T>.Default.Equals(value, de.DefaultValue))
			{
				object temp;
				values.TryRemove(de.GetHashCode(), out temp);
				if (de.ExternalPropertyChangedCallback != null) de.ExternalPropertyChangedCallback(this, (T)temp, de.DefaultValue);
			}
			else
			{
				object temp = values.AddOrUpdate(de.GetHashCode(), value, (p, v) => value);
				if (de.ExternalPropertyChangedCallback != null) de.ExternalPropertyChangedCallback(this, (T)temp, value);
			}

			de.IsUnset = false;
		}

		public void ClearExternalValue<T>(DependencyExternal<T> de)
		{
			de.IsUnset = true;

			object temp;
			if (values.TryRemove(de.GetHashCode(), out temp) && de.ExternalPropertyChangedCallback != null) de.ExternalPropertyChangedCallback(this, de.DefaultValue, (T)temp);
		}
	}

	public class DependencyExternalBase
	{
		protected static readonly ConcurrentDictionary<int, object> registered;

		static DependencyExternalBase()
		{
			registered = new ConcurrentDictionary<int, object>();
		}
	}

	public sealed class DependencyExternal<T> : DependencyExternalBase
	{
		public string Name { get; private set; }
		public Type OwnerType { get; private set; }
		public Type PropertyType { get; private set; }
		public T DefaultValue { get; private set; }
		private int isUnset = 0;
		public bool IsUnset { get { if (isUnset == 0) { return false; } return true; } internal set { if (value) { Interlocked.CompareExchange(ref isUnset, 1, 0); } else {Interlocked.CompareExchange(ref isUnset, 0, 1);} } }

		internal Action<DependencyObjectEx, T, T> ExternalPropertyChangedCallback { get; private set; }
		internal Func<DependencyObjectEx, T, bool> ExternalValidateValueCallback { get; private set; }

		public DependencyExternal() { }

		private DependencyExternal(string Name, Type OwnerType)
		{
			this.Name = Name;
			this.OwnerType = OwnerType;
			PropertyType = typeof(T);
			DefaultValue = default(T);
			ExternalPropertyChangedCallback = null;
			ExternalValidateValueCallback = null;
		}

		private DependencyExternal(string Name, Type OwnerType, T DefaultValue)
		{
			this.Name = Name;
			this.OwnerType = OwnerType;
			PropertyType = typeof(T);
			this.DefaultValue = DefaultValue;
			ExternalPropertyChangedCallback = null;
			ExternalValidateValueCallback = null;
		}

		private DependencyExternal(string Name, Type OwnerType, T DefaultValue, Action<DependencyObjectEx, T, T> ExternalPropertyChangedCallback)
		{
			this.Name = Name;
			this.OwnerType = OwnerType;
			PropertyType = typeof(T);
			this.DefaultValue = DefaultValue;
			this.ExternalPropertyChangedCallback = ExternalPropertyChangedCallback;
			ExternalValidateValueCallback = null;
		}

		private DependencyExternal(string Name, Type OwnerType, T DefaultValue, Action<DependencyObjectEx, T, T> ExternalPropertyChangedCallback, Func<DependencyObjectEx, T, bool> ExternalValidateValueCallback)
		{
			this.Name = Name;
			this.OwnerType = OwnerType;
			PropertyType = typeof(T);
			this.DefaultValue = DefaultValue;
			this.ExternalPropertyChangedCallback = ExternalPropertyChangedCallback;
			this.ExternalValidateValueCallback = ExternalValidateValueCallback;
		}

		public static DependencyExternal<TType> RegisterExternal<TType>(string name, Type ownerType)
		{
			var np = new DependencyExternal<TType>(name, ownerType);
			if (!registered.TryAdd(np.GetHashCode(), np))
				throw new ArgumentException(string.Format("Unable to register the DependencyExternal '{0}' on type '{1}'. A DependencyExternal with the same Name and OwnerType has already been registered.", name, ownerType));
			return np;
		}

		public static DependencyExternal<TType> RegisterExternal<TType>(string name, Type ownerType, TType defaultValue)
		{
			var np = new DependencyExternal<TType>(name, ownerType, defaultValue);
			if (!registered.TryAdd(np.GetHashCode(), np))
				throw new ArgumentException(string.Format("Unable to register the DependencyExternal '{0}' on type '{1}'. A DependencyExternal with the same Name and OwnerType has already been registered.", name, ownerType));
			return np;
		}

		public static DependencyExternal<TType> RegisterExternal<TType>(string name, Type ownerType, TType defaultValue, Action<DependencyObjectEx, TType, TType> ExternalPropertyChangedCallback)
		{
			var np = new DependencyExternal<TType>(name, ownerType, defaultValue, ExternalPropertyChangedCallback);
			if (!registered.TryAdd(np.GetHashCode(), np))
				throw new ArgumentException(string.Format("Unable to register the DependencyExternal '{0}' on type '{1}'. A DependencyExternal with the same Name and OwnerType has already been registered.", name, ownerType));
			return np;
		}

		public static DependencyExternal<TType> RegisterExternal<TType>(string name, Type ownerType, TType defaultValue, Action<DependencyObjectEx, TType, TType> ExternalPropertyChangedCallback, Func<DependencyObjectEx, TType, bool> ExternalValidateValueCallback)
		{
			var np = new DependencyExternal<TType>(name, ownerType, defaultValue, ExternalPropertyChangedCallback, ExternalValidateValueCallback);
			if (!registered.TryAdd(np.GetHashCode(), np))
				throw new ArgumentException(string.Format("Unable to register the DependencyExternal '{0}' on type '{1}'. A DependencyExternal with the same Name and OwnerType has already been registered.", name, ownerType));
			return np;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode() ^ OwnerType.GetHashCode();
		}
	}
}