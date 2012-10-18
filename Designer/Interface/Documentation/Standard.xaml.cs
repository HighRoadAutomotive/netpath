using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WCFArchitect.Projects;

namespace WCFArchitect.Interface.Documentation
{
	public partial class Standard : Grid
	{
		public bool IsProperty { get { return (bool)GetValue(IsPropertyProperty); } set { SetValue(IsPropertyProperty, value); } }
		public static readonly DependencyProperty IsPropertyProperty = DependencyProperty.Register("IsProperty", typeof(bool), typeof(Standard), new PropertyMetadata(false, ModeChangedCallback));

		public bool IsMethod { get { return (bool)GetValue(IsMethodProperty); } set { SetValue(IsMethodProperty, value); } }
		public static readonly DependencyProperty IsMethodProperty = DependencyProperty.Register("IsMethod", typeof(bool), typeof(Standard), new PropertyMetadata(false, ModeChangedCallback));

		public Projects.Documentation Documentation { get { return (Projects.Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Projects.Documentation), typeof(Standard));

		private static void ModeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var t = d as Standard;
			if (t == null) return;

			t.SummaryBlock.Visibility = Visibility.Visible;
			t.RemarksBlock.Visibility = Visibility.Visible;
			t.ReturnsBlock.Visibility = Visibility.Visible;
			t.ExampleBlock.Visibility = Visibility.Visible;
			t.ValueBlock.Visibility = Visibility.Visible;

			if (t.IsProperty)
				t.ReturnsBlock.Visibility = Visibility.Collapsed;
			if (t.IsMethod)
				t.ValueBlock.Visibility = Visibility.Collapsed;
		}

		public Standard()
		{
			InitializeComponent();

			SummaryBlock.DataContext = this;
			RemarksBlock.DataContext = this;
			ReturnsBlock.DataContext = this;
			ExampleBlock.DataContext = this;
			ValueBlock.DataContext = this;
		}
	}
}