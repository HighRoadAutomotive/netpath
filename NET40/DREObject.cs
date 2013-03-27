using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace System
{
	public abstract class DREObject<T> : DeltaObject where T : DREObject<T>
	{
		[DataMember(Name = "_DREID")]
		[ProtoMember(1, AsReference = false, DataFormat = DataFormat.Default, IsRequired = true)]
		public Guid _DREID { get { return GetValue(_DREIDProperty); } protected set { SetValue(_DREIDProperty, value); } }
		public static readonly DeltaProperty<Guid> _DREIDProperty = DeltaProperty<Guid>.Register("_DREID", typeof(T), default(Guid), null);

		private static readonly System.Collections.Concurrent.ConcurrentDictionary<Guid, T> __dcm;
		static DREObject()
		{
			__dcm = new System.Collections.Concurrent.ConcurrentDictionary<Guid, T>();
		}

		public static T GetDataFromID(Guid ID)
		{
			T t;
			__dcm.TryGetValue(ID, out t);
			return t;
		}

		public static void UpdateValue<TType>(Guid ID, DeltaProperty<TType> prop, TType value)
		{
			T t;
			__dcm.TryGetValue(ID, out t);
			if (t != null) t.SetValue(prop, value);
		}

		public static bool HasData(T data)
		{
			return __dcm.ContainsKey(data._DREID); 
		}

		public static T RegisterData(Guid ClientID, T Data)
		{
			lock(Data.__crllock)
			{
				Data.__crl.GetOrAdd(ClientID, Data._DREID);
				return __dcm.GetOrAdd(Data._DREID, Data);
			}
		}

		public static bool UnregisterData(Guid DataID, Guid ClientID)
		{
			T data;
			__dcm.TryGetValue(DataID, out data);
			if(data == null) return true;
			Guid dreid;
			data.__crl.TryRemove(ClientID, out dreid);
			lock(data.__crllock) { T t; if(data.__crl.IsEmpty) return __dcm.TryRemove(DataID, out t); }
			return true;
		}

		public IEnumerable<Guid> ClientList { get { return __crl.Keys; } }
		private ConcurrentDictionary<Guid, Guid> __crl = new ConcurrentDictionary<Guid, Guid>();
		private object __crllock = new object();

		protected abstract void BatchUpdates();

		protected sealed override void IncrementChangeCount()
		{
			base.IncrementChangeCount();
			BatchUpdates();
		}
	}
}