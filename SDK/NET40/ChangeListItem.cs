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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace System.Collections.Generic
{
	public enum ListItemChangeMode
	{
		Add,
		Replace,
		Remove,
		Insert,
		Move
	}

	[DataContract(Namespace = "http://www.prospectivesoftware.com/")]
	public struct ChangeListItem<T>
	{
		[DataMember(Order = 0)] public ListItemChangeMode Mode { get; private set; }
		[DataMember(Order = 1)] public T Item { get; private set; }
		[DataMember(Order = 2)] public T OldItem { get; private set; }
		[DataMember(Order = 3)] public int Index { get; private set; }

		public ChangeListItem(ListItemChangeMode Mode, T Item, int Index = -1) : this()
		{
			this.Mode = Mode;
			this.Item = Item;
			this.Index = Index;
		}

		public ChangeListItem(ListItemChangeMode Mode, T Item, T OldItem) : this()
		{
			this.Mode = Mode;
			this.Item = Item;
			this.OldItem = OldItem;
		}
	}

	[DataContract(Namespace = "http://www.prospectivesoftware.com/")]
	public struct ChangeDictionaryItem<TKey, T>
	{
		[DataMember(Order = 0)] public ListItemChangeMode Mode { get; private set; }
		[DataMember(Order = 1)] public TKey Key { get; private set; }
		[DataMember(Order = 2)] public T Value { get; private set; }

		public ChangeDictionaryItem(ListItemChangeMode Mode, TKey Key, T Value) : this()
		{
			this.Mode = Mode;
			this.Key = Key;
			this.Value = Value;
		}
	}

	[DataContract(Namespace = "http://www.prospectivesoftware.com/")]
	public abstract class CMDItemBase
	{
		[DataMember(Order = 0)] public bool UseDefault { get; protected set; }
		[DataMember(Order = 1)] public HashID Key { get; protected set; }

		public abstract object GetValue();
	}

	[DataContract(Namespace = "http://www.prospectivesoftware.com/")]
	public class CMDItemValue<T> : CMDItemBase
	{
		[DataMember(Order = 3)] public T Value { get; private set; }

		public CMDItemValue(bool UseDefault, HashID Key, T Value = default(T))
		{
			this.UseDefault = UseDefault;
			this.Key = Key;
			this.Value = Value;
		}

		public override object GetValue()
		{
			return Value;
		}
	}
}