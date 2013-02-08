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
	public class ChangeListItem<T>
	{
		[DataMember(Order = 0)] public ListItemChangeMode Mode { get; set; }
		[DataMember(Order = 1)] public T Item { get; set; }
		[DataMember(Order = 2)] public T OldItem { get; set; }
		[DataMember(Order = 3)] public int Index { get; set; }

		public ChangeListItem(ListItemChangeMode Mode, T Item, int Index = -1)
		{
			this.Mode = Mode;
			this.Item = Item;
			this.Index = Index;
		}

		public ChangeListItem(ListItemChangeMode Mode, T Item, T OldItem)
		{
			this.Mode = Mode;
			this.Item = Item;
			this.OldItem = OldItem;
		}
	}

	[DataContract(Namespace = "http://www.prospectivesoftware.com/")]
	public class ChangeDictionaryItem<TKey, T>
	{
		[DataMember(Order = 0)] public ListItemChangeMode Mode { get; set; }
		[DataMember(Order = 1)] public TKey Key { get; set; }
		[DataMember(Order = 2)] public T Value { get; set; }

		public ChangeDictionaryItem(ListItemChangeMode Mode, TKey Key, T Value)
		{
			this.Mode = Mode;
			this.Key = Key;
			this.Value = Value;
		}
	}
}