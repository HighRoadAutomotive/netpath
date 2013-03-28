using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using ProtoBuf;

namespace System
{
	[Serializable]
	public abstract class DREObject<T> : DeltaObject where T : DREObject<T>
	{
		[DataMember(Name = "_DREID")]
		[ProtoMember(1, AsReference = false, DataFormat = DataFormat.Default, IsRequired = true)]
		public Guid _DREID { get { return GetValue(_DREIDProperty); } protected set { SetValue(_DREIDProperty, value); } }
		[NonSerialized, IgnoreDataMember, XmlIgnore] public static readonly DeltaProperty<Guid> _DREIDProperty = DeltaProperty<Guid>.Register("_DREID", typeof(T), default(Guid), null);

		[NonSerialized, IgnoreDataMember, XmlIgnore] private static readonly System.Collections.Concurrent.ConcurrentDictionary<Guid, T> __dcm;
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

		public static bool HasData(Guid DataID)
		{
			return __dcm.ContainsKey(DataID);
		}

		public static T RegisterData(Guid ClientID, T Data)
		{
			lock (Data.__crllock)
			{
				Data.__crl.GetOrAdd(ClientID, Data._DREID);
				return __dcm.GetOrAdd(Data._DREID, Data);
			}
		}

		public static T RegisterData(Guid ClientID, Guid DataID)
		{
			T Data;
			__dcm.TryGetValue(DataID, out Data);
			if (Data == null) return null;
			lock (Data.__crllock)
			{
				Data.__crl.GetOrAdd(ClientID, Data._DREID);
				return __dcm.GetOrAdd(Data._DREID, Data);
			}
		}

		public static bool UnregisterData(Guid ClientID, Guid DataID)
		{
			T data;
			__dcm.TryGetValue(DataID, out data);
			if (data == null) return true;
			Guid dreid;
			data.__crl.TryRemove(ClientID, out dreid);
			lock (data.__crllock)
			{
				T t;
				if (data.__crl.IsEmpty) return __dcm.TryRemove(DataID, out t);
			}
			return true;
		}

		[IgnoreDataMember, XmlIgnore] public IEnumerable<Guid> ClientList { get { return __crl.Keys; } }
		[NonSerialized, IgnoreDataMember, XmlIgnore] private ConcurrentDictionary<Guid, Guid> __crl = new ConcurrentDictionary<Guid, Guid>();
		[NonSerialized, IgnoreDataMember, XmlIgnore] private object __crllock = new object();

		protected DREObject()
		{
			_DREID = Guid.NewGuid();
		}

		protected DREObject(DependencyObjectEx baseXAMLObject)
			: base(baseXAMLObject)
		{
			_DREID = Guid.NewGuid();
		}

		protected DREObject(long BatchInterval)
			: base(BatchInterval)
		{
			_DREID = Guid.NewGuid();
		}

		protected DREObject(DependencyObjectEx baseXAMLObject, long BatchInterval)
			: base(baseXAMLObject, BatchInterval)
		{
			_DREID = Guid.NewGuid();
		}

		protected abstract void BatchUpdates();

		protected sealed override void IncrementChangeCount()
		{
			base.IncrementChangeCount();
			BatchUpdates();
		}
	}
}