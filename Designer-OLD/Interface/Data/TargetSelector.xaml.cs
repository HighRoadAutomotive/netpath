using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
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
using NETPath.Projects;

namespace NETPath.Interface.Data
{
	internal partial class TargetSelector : Grid
	{
		public DataType OpenType { get { return (DataType)GetValue(OpenTypeProperty); } set { SetValue(OpenTypeProperty, value); } }
		public static readonly DependencyProperty OpenTypeProperty = DependencyProperty.Register("OpenType", typeof(DataType), typeof(TargetSelector), new PropertyMetadata(null));

		public bool IsServerType { get { return (bool)GetValue(IsServerTypeProperty); } set { SetValue(IsServerTypeProperty, value); } }
		public static readonly DependencyProperty IsServerTypeProperty = DependencyProperty.Register("IsServerType", typeof(bool), typeof(TargetSelector), new PropertyMetadata(false));

		public TargetSelector()
		{
			InitializeComponent();
			BG.DataContext = this;
		}

		private void TargetInclude_Checked(object Sender, RoutedEventArgs E)
		{
			if (Sender == null) return;
			var lbi = Globals.GetVisualParent<ListBoxItem>(Sender);
			if (lbi == null) return;
			var pgt = lbi.Content as ProjectGenerationTarget;
			if (pgt == null) return;

			if (pgt.TargetTypes.Contains(OpenType)) return;
			pgt.TargetTypes.Add(OpenType);
		}

		private void TargetInclude_Unchecked(object Sender, RoutedEventArgs E)
		{
			if (Sender == null) return;
			var lbi = Globals.GetVisualParent<ListBoxItem>(Sender);
			if (lbi == null) return;
			var pgt = lbi.Content as ProjectGenerationTarget;
			if (pgt == null) return;

			if (!pgt.TargetTypes.Contains(OpenType)) return;
			pgt.TargetTypes.Remove(OpenType);
		}
	}

	[ValueConversion(typeof(ProjectGenerationTarget), typeof(bool))]
	public class TargetHasTypeConverter : IMultiValueConverter
	{
		public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return false;
			var lt = value[0] as ProjectGenerationTarget;
			var te = value[1] as DataType;
			if (lt == null || te == null) return false;

			return lt.TargetTypes.Contains(te);
		}

		public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
		{
			return new object[] { };
		}
	}
}