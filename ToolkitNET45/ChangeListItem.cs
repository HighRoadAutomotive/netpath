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
		[DataMember]public ListItemChangeMode Mode { get; set; }
		[DataMember]public T Item { get; set; }
		[DataMember]public int Index { get; set; }

		public ChangeListItem(ListItemChangeMode Mode, T Item, int Index = 0)
		{
			this.Mode = Mode;
			this.Item = Item;
			this.Index = Index;
		}
	}
}