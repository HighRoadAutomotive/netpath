using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace System.ServiceModel
{
	public enum ListItemChangeMode
	{
		Add,
		Remove,
		Insert
	}

	[DataContract(Namespace = "http://www.prospectivesoftware.com/")]
	public class ChangeListItem<T>
	{
		[DataMember(Order = 0)] public ListItemChangeMode Mode { get; set; }
		[DataMember(Order = 1)] public T Item { get; set; }
		[DataMember(Order = 2)] public int Index { get; set; }

		public ChangeListItem(ListItemChangeMode Mode, T Item, int Index = -1)
		{
			this.Mode = Mode;
			this.Item = Item;
			this.Index = Index;
		}
	}

	[DataContract(Namespace = "http://www.prospectivesoftware.com/")]
	public class ChangeDictionaryItem<T, TKey>
	{
		[DataMember(Order = 0)] public ListItemChangeMode Mode { get; set; }
		[DataMember(Order = 1)] public TKey Index { get; set; }

		public ChangeDictionaryItem(ListItemChangeMode Mode, TKey Index)
		{
			this.Mode = Mode;
			this.Index = Index;
		}
	}
}