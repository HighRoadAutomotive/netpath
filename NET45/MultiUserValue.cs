using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ProtoBuf;

namespace System
{
	[DataContract]
	[ProtoContract]
	public class MultiUserValueData<TID, TValue> where TID : struct
	{
		private readonly DateTime timestamp;
		private readonly TID userID;
		private readonly TValue value;

		public DateTime TimeStamp { get { return timestamp; } }
		public TID UserID { get { return userID; } }
		public TValue Value { get { return Value; } }

		public MultiUserValueData(TID UserID, TValue Value)
		{
			timestamp = DateTime.UtcNow;
			userID = UserID;
			value = Value;
		}
	}

	[DataContract]
	[ProtoContract]
	public struct MultiUserValue<TID, TValue> where TID : struct
	{
		[DataMember] [ProtoMember(1, AsReference = true, OverwriteList = true)] private ConcurrentDictionary<TID, MultiUserValueData<TID, TValue>> Values;
		[DataMember] [ProtoMember(2)] private TValue Current;

		public TValue Value { get { return Current; } set { lock (Values) Current = value; } }

		public MultiUserValue(TValue value)
		{
			Current = value;
			Values = new ConcurrentDictionary<TID, MultiUserValueData<TID, TValue>>();
		}

		public MultiUserValueData<TID, TValue> GetUserValue(TID UserID)
		{
			MultiUserValueData<TID, TValue> muuv = null;
			Values.TryGetValue(UserID, out muuv);
			return muuv;
		}

		public IEnumerable<MultiUserValueData<TID, TValue>> GetUserValueList()
		{
			return Values.ToArray().Select(a => a.Value);
		}

		public void SetUserValue(TID UserID, TValue NewValue)
		{
			var nv = new MultiUserValueData<TID, TValue>(UserID, Value);
			Values.AddOrUpdate(UserID, nv, (p, v) => nv);
		}

		public bool SelectUserValue(TID UserID)
		{
			MultiUserValueData<TID, TValue> muuv = null;
			Values.TryGetValue(UserID, out muuv);
			if (muuv == null) return false;
			Values.Clear();
			lock (Values) Current = muuv.Value;
			return true;
		}

		#region - Operators -

		public bool Equals(MultiUserValue<TID, TValue> other)
		{
			return EqualityComparer<TValue>.Default.Equals(Current, other.Current);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is MultiUserValue<TID, TValue> && Equals((MultiUserValue<TID, TValue>) obj);
		}

		public override int GetHashCode()
		{
			return EqualityComparer<TValue>.Default.GetHashCode(Current);
		}

		public static bool operator ==(MultiUserValue<TID, TValue> x, MultiUserValue<TID, TValue> y)
		{
			return x.Current.Equals(y.Current);
		}

		public static bool operator !=(MultiUserValue<TID, TValue> x, MultiUserValue<TID, TValue> y)
		{
			return !x.Current.Equals(y.Current);
		}

		public static bool operator ==(MultiUserValue<TID, TValue> x,  TValue y)
		{
			return x.Current.Equals(y);
		}

		public static bool operator !=(MultiUserValue<TID, TValue> x, TValue y)
		{
			return !x.Current.Equals(y);
		}

		public static bool operator ==(TValue x, MultiUserValue<TID, TValue> y)
		{
			return x.Equals(y.Current);
		}

		public static bool operator !=(TValue x, MultiUserValue<TID, TValue> y)
		{
			return !x.Equals(y.Current);
		}

		public static implicit operator TValue(MultiUserValue<TID, TValue> x)
		{
			return x.Current;
		}

		public static implicit operator MultiUserValue<TID, TValue>(TValue x)
		{
			return new MultiUserValue<TID, TValue>(x);
		}

		#endregion
	}
}