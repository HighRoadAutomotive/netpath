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
}