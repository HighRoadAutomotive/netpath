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
using System.Text;
using System.Windows;

namespace System
{
	public sealed class DREProperty<T> : CMDPropertyBase
	{
		public T DefaultValue { get { return (T)defaultValue; } private set { defaultValue = value; } }

		internal Action<DREObjectBase, T, T> DREPropertyChangedCallback { get; private set; }
		internal Action<DREObjectBase, T, T> DREPropertyUpdatedCallback { get; private set; }
		internal Func<DREObjectBase, T, bool> DeltaValidateValueCallback { get; private set; }

		internal bool EnableBatching { get; private set; }
		internal bool IsList { get; private set; }
		internal bool IsDictionary { get; private set; }

		private DREProperty(HashID ID, HashID OwnerID, Type OwnerType, bool EnableBatching, DependencyProperty XAMLProperty = null, DependencyPropertyKey XAMLPropertyKey = null)
		{
			this.ID = ID;
			this.OwnerID = OwnerID;
			this.OwnerType = OwnerType;
			PropertyType = typeof(T);
			DefaultValue = default(T);
			DREPropertyChangedCallback = null;
			DeltaValidateValueCallback = null;
			this.XAMLProperty = XAMLProperty;
			this.XAMLPropertyKey = XAMLPropertyKey;
			this.EnableBatching = EnableBatching;
		}

		private DREProperty(HashID ID, HashID OwnerID, Type OwnerType, bool EnableBatching, T DefaultValue, DependencyProperty XAMLProperty = null, DependencyPropertyKey XAMLPropertyKey = null)
		{
			this.ID = ID;
			this.OwnerID = OwnerID;
			this.OwnerType = OwnerType;
			PropertyType = typeof(T);
			this.DefaultValue = DefaultValue;
			DREPropertyChangedCallback = null;
			DeltaValidateValueCallback = null;
			this.XAMLProperty = XAMLProperty;
			this.XAMLPropertyKey = XAMLPropertyKey;
			this.EnableBatching = EnableBatching;
		}

		private DREProperty(HashID ID, HashID OwnerID, Type OwnerType, bool EnableBatching, T DefaultValue, Action<DREObjectBase, T, T> DREPropertyChangedCallback, Action<DREObjectBase, T, T> DREPropertyUpdatedCallback = null, Func<DREObjectBase, T, bool> DeltaValidateValueCallback = null, DependencyProperty XAMLProperty = null, DependencyPropertyKey XAMLPropertyKey = null)
		{
			this.ID = ID;
			this.OwnerID = OwnerID;
			this.OwnerType = OwnerType;
			PropertyType = typeof(T);
			this.DefaultValue = DefaultValue;
			this.DREPropertyChangedCallback = DREPropertyChangedCallback;
			this.DREPropertyUpdatedCallback = DREPropertyUpdatedCallback;
			this.DeltaValidateValueCallback = DeltaValidateValueCallback;
			this.XAMLProperty = XAMLProperty;
			this.XAMLPropertyKey = XAMLPropertyKey;
			this.EnableBatching = EnableBatching;
		}

		private DREProperty(HashID ID, HashID OwnerID, Type OwnerType, bool EnableBatching, Action<DREObjectBase, T, T> DREPropertyChangedCallback)
		{
			this.ID = ID;
			this.OwnerID = OwnerID;
			this.OwnerType = OwnerType;
			PropertyType = typeof(T);
			DefaultValue = default(T);
			this.DREPropertyChangedCallback = DREPropertyChangedCallback;
			DeltaValidateValueCallback = null;
			this.EnableBatching = EnableBatching;
		}

		public static DREProperty<TType> Register<TType>(string Name, Type OwnerType, bool EnableBatching, DependencyProperty XAMLProperty = null, DependencyPropertyKey XAMLPropertyKey = null)
		{
			var np = new DREProperty<TType>(HashID.GenerateHashID(OwnerType.FullName + "." + Name), HashID.GenerateHashID(OwnerType.FullName), OwnerType, EnableBatching, XAMLProperty, XAMLPropertyKey);
			if (!registered.TryAdd(np.ID, np))
				throw new ArgumentException(string.Format("Unable to register the DREProperty '{0}' on type '{1}'. A DREProperty with the same Name and OwnerType has already been registered.", Name, np.OwnerType));
			return np;
		}

		public static DREProperty<TType> Register<TType>(string Name, Type OwnerType, bool EnableBatching, TType DefaultValue, DependencyProperty XAMLProperty = null, DependencyPropertyKey XAMLPropertyKey = null)
		{
			var np = new DREProperty<TType>(HashID.GenerateHashID(OwnerType.FullName + "." + Name), HashID.GenerateHashID(OwnerType.FullName), OwnerType, EnableBatching, DefaultValue, XAMLProperty, XAMLPropertyKey);
			if (!registered.TryAdd(np.ID, np))
				throw new ArgumentException(string.Format("Unable to register the DREProperty '{0}' on type '{1}'. A DREProperty with the same Name and OwnerType has already been registered.", Name, np.OwnerType));
			return np;
		}

		public static DREProperty<TType> Register<TType>(string Name, Type OwnerType, bool EnableBatching, TType DefaultValue, Action<DREObjectBase, TType, TType> DREPropertyChangedCallback, Action<DREObjectBase, TType, TType> DREPropertyUpdatedCallback = null, Func<DREObjectBase, TType, bool> DeltaValidateValueCallback = null, DependencyProperty XAMLProperty = null, DependencyPropertyKey XAMLPropertyKey = null)
		{
			var np = new DREProperty<TType>(HashID.GenerateHashID(OwnerType.FullName + "." + Name), HashID.GenerateHashID(OwnerType.FullName), OwnerType, EnableBatching, DefaultValue, DREPropertyChangedCallback, DREPropertyUpdatedCallback, DeltaValidateValueCallback, XAMLProperty, XAMLPropertyKey);
			if (!registered.TryAdd(np.ID, np))
				throw new ArgumentException(string.Format("Unable to register the DREProperty '{0}' on type '{1}'. A DREProperty with the same Name and OwnerType has already been registered.", Name, np.OwnerType));
			return np;
		}

		public static DREProperty<DeltaDictionary<TKey, TType>> RegisterDictionary<TKey, TType>(string Name, Type OwnerType, Action<DREObjectBase, DeltaDictionary<TKey, TType>, DeltaDictionary<TKey, TType>> DREPropertyChangedCallback)
		{
			var np = new DREProperty<DeltaDictionary<TKey, TType>>(HashID.GenerateHashID(OwnerType.FullName + "." + Name), HashID.GenerateHashID(OwnerType.FullName), OwnerType, true, DREPropertyChangedCallback) { IsDictionary = true };
			if (!registered.TryAdd(np.ID, np))
				throw new ArgumentException(string.Format("Unable to register the DREProperty '{0}' on type '{1}'. A DREProperty with the same Name and OwnerType has already been registered.", Name, np.OwnerType));
			return np;
		}

		public static DREProperty<DeltaList<TType>> RegisterList<TType>(string Name, Type OwnerType, Action<DREObjectBase, DeltaList<TType>, DeltaList<TType>> DREPropertyChangedCallback)
		{
			var np = new DREProperty<DeltaList<TType>>(HashID.GenerateHashID(OwnerType.FullName + "." + Name), HashID.GenerateHashID(OwnerType.FullName), OwnerType, true, DREPropertyChangedCallback) { IsList = true };
			if (!registered.TryAdd(np.ID, np))
				throw new ArgumentException(string.Format("Unable to register the DREProperty '{0}' on type '{1}'. A DREProperty with the same Name and OwnerType has already been registered.", Name, np.OwnerType));
			return np;
		}

		public override int GetHashCode()
		{
			return ID.GetHashCode() ^ OwnerType.GetHashCode();
		}
	}
}