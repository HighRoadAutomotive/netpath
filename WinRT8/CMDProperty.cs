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
using Windows.UI.Xaml;

namespace System
{
	public abstract class CMDPropertyBase
	{
		protected static readonly ConcurrentDictionary<HashID, CMDPropertyBase> registered;

		static CMDPropertyBase()
		{
			registered = new ConcurrentDictionary<HashID, CMDPropertyBase>();
		}

		public HashID ID { get; protected set; }
		public string Name { get; protected set; }
		public HashID OwnerID { get; protected set; }
		public Type OwnerType { get; protected set; }
		public Type PropertyType { get; protected set; }
		internal object defaultValue { get; set; }
		public DependencyProperty XAMLProperty { get; protected set; }

		internal static CMDPropertyBase FromID(HashID ID)
		{
			CMDPropertyBase ret;
			registered.TryGetValue(ID, out ret);
			return ret;
		}
	}

	public sealed class CMDProperty<T> : CMDPropertyBase
	{
		public T DefaultValue { get { return (T)defaultValue; } private set { defaultValue = value; } }

		internal Action<CMDObject, T, T> CMDPropertyChangedCallback { get; private set; }
		internal Func<CMDObject, T, bool> CMDValidateValueCallback { get; private set; }

		private CMDProperty(HashID ID, string Name, HashID OwnerID, Type OwnerType)
		{
			this.ID = ID;
			this.Name = Name;
			this.OwnerID = OwnerID;
			this.OwnerType = OwnerType;
			PropertyType = typeof(T);
			DefaultValue = default(T);
			CMDPropertyChangedCallback = null;
			CMDValidateValueCallback = null;
		}

		private CMDProperty(HashID ID, string Name, HashID OwnerID, Type OwnerType, T DefaultValue)
		{
			this.ID = ID;
			this.Name = Name;
			this.OwnerID = OwnerID;
			this.OwnerType = OwnerType;
			PropertyType = typeof(T);
			this.DefaultValue = DefaultValue;
			CMDPropertyChangedCallback = null;
			CMDValidateValueCallback = null;
		}

		private CMDProperty(HashID ID, string Name, HashID OwnerID, Type OwnerType, T DefaultValue, Action<CMDObject, T, T> CMDPropertyChangedCallback, Func<CMDObject, T, bool> CMDValidateValueCallback = null)
		{
			this.ID = ID;
			this.Name = Name;
			this.OwnerID = OwnerID;
			this.OwnerType = OwnerType;
			PropertyType = typeof (T);
			this.DefaultValue = DefaultValue;
			this.CMDPropertyChangedCallback = CMDPropertyChangedCallback;
			this.CMDValidateValueCallback = CMDValidateValueCallback;
		}

		public static CMDProperty<TType> Register<TType>(string Name, Type OwnerType)
		{
			var np = new CMDProperty<TType>(HashID.GenerateHashID(OwnerType.FullName + "." + Name), Name, HashID.GenerateHashID(OwnerType.FullName), OwnerType);
			if (!registered.TryAdd(np.ID, np))
				throw new ArgumentException(string.Format("Unable to register the CMDProperty '{0}' on type '{1}'. A CMDProperty with the same Name and OwnerType has already been registered.", Name, np.OwnerType));
			return np;
		}

		public static CMDProperty<TType> Register<TType>(string Name, Type OwnerType, TType defaultValue)
		{
			var np = new CMDProperty<TType>(HashID.GenerateHashID(OwnerType.FullName + "." + Name), Name, HashID.GenerateHashID(OwnerType.FullName), OwnerType, defaultValue);
			if (!registered.TryAdd(np.ID, np))
				throw new ArgumentException(string.Format("Unable to register the CMDProperty '{0}' on type '{1}'. A CMDProperty with the same Name and OwnerType has already been registered.", Name, np.OwnerType));
			return np;
		}

		public static CMDProperty<TType> Register<TType>(string Name, Type OwnerType, TType defaultValue, Action<CMDObject, TType, TType> CMDPropertyChangedCallback, Func<CMDObject, TType, bool> CMDValidateValueCallback = null)
		{
			var np = new CMDProperty<TType>(HashID.GenerateHashID(OwnerType.FullName + "." + Name), Name, HashID.GenerateHashID(OwnerType.FullName), OwnerType, defaultValue, CMDPropertyChangedCallback, CMDValidateValueCallback);
			if (!registered.TryAdd(np.ID, np))
				throw new ArgumentException(string.Format("Unable to register the CMDProperty '{0}' on type '{1}'. A CMDProperty with the same Name and OwnerType has already been registered.", Name, np.OwnerType));
			return np;
		}

		public override int GetHashCode()
		{
			return ID.GetHashCode() ^ OwnerType.GetHashCode();
		}
	}
}