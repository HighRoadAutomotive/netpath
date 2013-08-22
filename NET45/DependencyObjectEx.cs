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
	public abstract class DependencyObjectEx : DependencyObject
	{
		private readonly ConcurrentDictionary<int, object> values;
		private DREObjectBase baseDataObject;
		internal protected DREObjectBase BaseDataObject { get { return baseDataObject; } set { if (baseDataObject == null) baseDataObject = value; } }
		protected bool IsExternalUpdate { get; set; }

		protected DependencyObjectEx()
		{
			values = new ConcurrentDictionary<int, object>();
			baseDataObject = null;
		}

		protected DependencyObjectEx(DREObjectBase baseDataObject)
		{
			values = new ConcurrentDictionary<int, object>();
			this.baseDataObject = baseDataObject;
		}

		public T GetValueThreaded<T>(DependencyProperty dp)
		{
			if (Application.Current.Dispatcher.CheckAccess()) return (T)GetValue(dp);
			T ret = default(T);
			Application.Current.Dispatcher.Invoke(() => { ret = (T)GetValue(dp); }, DispatcherPriority.Normal);
			return ret;
		}

		public void SetValueThreaded<T>(DependencyProperty dp, T value)
		{
			if (Application.Current.Dispatcher.CheckAccess()) { SetValue(dp, value); }
			else Application.Current.Dispatcher.Invoke(() => { SetValue(dp, value); }, DispatcherPriority.Normal);
		}

		public void SetValueThreaded<T>(DependencyPropertyKey dp, T value)
		{
			if (Application.Current.Dispatcher.CheckAccess()) { SetValue(dp, value); }
			else Application.Current.Dispatcher.Invoke(() => { SetValue(dp, value); }, DispatcherPriority.Normal);
		}

		public void UpdateValueThreaded<T>(DependencyProperty dp, T value)
		{
			if (Application.Current.Dispatcher.CheckAccess()) { IsExternalUpdate = true; T a = (T)GetValue(dp); SetValue(dp, value); T b = (T)GetValue(dp); IsExternalUpdate = false; }
			else Application.Current.Dispatcher.Invoke(() => { IsExternalUpdate = true; SetValue(dp, value); IsExternalUpdate = false; }, DispatcherPriority.Normal);
		}

		public void UpdateValueThreaded<T>(DependencyPropertyKey dp, T value)
		{
			if (Application.Current.Dispatcher.CheckAccess()) { IsExternalUpdate = true; SetValue(dp, value); IsExternalUpdate = false; }
			else Application.Current.Dispatcher.Invoke(() => { IsExternalUpdate = true; SetValue(dp, value); IsExternalUpdate = false; }, DispatcherPriority.Normal);
		}

		public T GetValueExternal<T>(DependencyExternal<T> de)
		{
			if (de.IsUnset)
				throw new ArgumentException(string.Format("External value '{0}' in type '{1}' is unset. You must set a value before accessing the property.", de.Name, de.OwnerType));

			object value;
			return values.TryGetValue(de.GetHashCode(), out value) == false ? de.DefaultValue : (T)value;
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