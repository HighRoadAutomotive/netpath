using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
	public abstract class DeltaCollectionBase
	{
		internal EventHandler Changed;

		internal void ClearChangedHandlers()
		{
			Changed = null;
		}
	}
}