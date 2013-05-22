using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDTest
{
	class Program
	{
		static void Main(string[] args)
		{
			var t = new Customer();
			var tl = new Task[8];
			tl[0] = Task.Factory.StartNew(() => { for (int i = 0; i < 20; i++) t.Name = "Thread 1"; });
			tl[1] = Task.Factory.StartNew(() => { for (int i = 0; i < 20; i++) t.Name = "Thread 2"; });
			tl[2] = Task.Factory.StartNew(() => { for (int i = 0; i < 20; i++) t.Name = "Thread 3"; });
			tl[3] = Task.Factory.StartNew(() => { for (int i = 0; i < 20; i++) t.Name = "Thread 4"; });
			tl[4] = Task.Factory.StartNew(() => { for (int i = 0; i < 20; i++) Console.WriteLine(t.Name); });
			tl[5] = Task.Factory.StartNew(() => { for (int i = 0; i < 20; i++) Console.WriteLine(t.Name); });
			tl[6] = Task.Factory.StartNew(() => { for (int i = 0; i < 20; i++) Console.WriteLine(t.Name); });
			tl[7] = Task.Factory.StartNew(() => { for (int i = 0; i < 20; i++) Console.WriteLine(t.Name); });

			Task.WaitAll(tl);
			Console.ReadLine();
		}
	}

	public partial class Customer : DeltaObject
	{
		public string Name { get { return GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DeltaProperty<string> NameProperty = DeltaProperty<string>.Register("Name", typeof(Customer), default(string), (s, o, n) => { var t = s as Customer; if (t == null) return; t.NamePropertyChanged(o, n); }, null);
		public void NamePropertyChanged(string OldValue, string NewValue) { return; }
	}
}