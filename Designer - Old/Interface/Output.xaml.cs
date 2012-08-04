using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WCFArchitect.Interface
{
	public partial class Output : Grid
	{
		public Output(FlowDocument Output)
		{
			InitializeComponent();

			OutputBox.Document = Output;
		}

		private void OutputBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			try
			{
				OutputBox.ScrollToEnd();
			}
			catch
			{ return; }
		}
	}
}