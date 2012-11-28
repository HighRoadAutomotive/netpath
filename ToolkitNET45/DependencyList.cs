using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace WCFArchitect.Toolkit.NET45
{
	public class DependencyList<T> : ObservableCollection<T>,  IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		private ReaderWriterLockSlim ocl;
		private Action<T> Added { get; set; }
		private Action<int, T> Inserted { get; set; }
		private Action<int, int, T> Moved { get; set; }
		private Action<T> Removed { get; set; }
		private Action<int, T> RemovedAt { get; set; }

		public DependencyList()
		{
			ocl = new ReaderWriterLockSlim();
		}

		public new int IndexOf(T item)
		{
			ocl.EnterReadLock();
			try
			{
				return base.IndexOf(item);
			}
			finally 
			{
				ocl.ExitReadLock();
			}
		}

		public new void Insert(int index, T item)
		{
			ocl.EnterWriteLock();
			try
			{
				base.Insert(index, item);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		public T this[int index]
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public void Add(T item)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(T item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}

		public bool Remove(T item)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<T> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(PropertyName));
		}
	}
}