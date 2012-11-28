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
using System.Windows;
using System.Windows.Threading;

namespace WCFArchitect.Toolkit.NET45
{
	public class DependencyList<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		private List<T> il;
		private ReaderWriterLockSlim ocl;
		private Action<IEnumerable<T>> Added { get; set; }
		private Action<int, IEnumerable<T>> Inserted { get; set; }
		private Action<int, int, T> Moved { get; set; }
		private Action<IEnumerable<T>> Removed { get; set; }
		private Action<int, IEnumerable<T>> RemovedAt { get; set; }

		public DependencyList()
		{
			il = new List<T>();
			ocl = new ReaderWriterLockSlim();
		}

		public int IndexOf(T item)
		{
			ocl.EnterReadLock();
			try
			{
				return il.IndexOf(item);
			}
			finally 
			{
				ocl.ExitReadLock();
			}
		}

		public void Insert(int index, T item)
		{
			ocl.EnterWriteLock();
			try
			{
				il.Insert(index, item);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
		}

		public void RemoveAt(int index)
		{
			ocl.EnterWriteLock();
			try
			{
				il.RemoveAt(index);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
		}

		public T this[int index]
		{
			get
			{
				ocl.EnterReadLock();
				try
				{
					return il[index];
				}
				finally
				{
					ocl.ExitReadLock();
				}
			}
			set
			{
				ocl.EnterWriteLock();
				try
				{
					il[index] = value;
				}
				finally
				{
					ocl.ExitWriteLock();
				}
			}
		}

		public void Add(T item)
		{
			ocl.EnterWriteLock();
			try
			{
				il.Add(item);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
		}

		public void Clear()
		{
			ocl.EnterWriteLock();
			try
			{
				il.Clear();
			}
			finally
			{
				ocl.ExitWriteLock();
			}
		}

		public bool Contains(T item)
		{
			ocl.EnterReadLock();
			try
			{
				return il.Contains(item);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			ocl.EnterReadLock();
			try
			{
				il.CopyTo(array, arrayIndex);
			}
			finally
			{
				ocl.ExitReadLock();
			}
		}

		public int Count
		{
			get
			{
				ocl.EnterReadLock();
				try
				{
					return il.Count;
				}
				finally
				{
					ocl.ExitReadLock();
				}
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(T item)
		{
			ocl.EnterWriteLock();
			try
			{
				return il.Remove(item);
			}
			finally
			{
				ocl.ExitWriteLock();
			}
		}

		private void CallInserted(int index, T item)
		{
			if (Application.Current.Dispatcher == null) { Inserted(index, new List<T>() { item }); return; }
			if (Application.Current.Dispatcher.CheckAccess()) Inserted(index, new List<T>() { item });
			else Application.Current.Dispatcher.Invoke(() => Inserted(index, new List<T>() { item }), DispatcherPriority.Normal);
		}

		private void CallInserted(int index, IEnumerable<T> items)
		{
			if (Application.Current.Dispatcher == null) { Inserted(index, items); return; }
			if (Application.Current.Dispatcher.CheckAccess()) Inserted(index, items);
			else Application.Current.Dispatcher.Invoke(() => Inserted(index, items), DispatcherPriority.Normal);
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