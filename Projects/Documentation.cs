using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WCFArchitect.Projects
{
	public class Documentation : DependencyObject
	{
		public bool IsClass { get { return (bool)GetValue(IsClassProperty); } set { SetValue(IsClassProperty, value); } }
		public static readonly DependencyProperty IsClassProperty = DependencyProperty.Register("IsClass", typeof(bool), typeof(Documentation), new PropertyMetadata(false));

		public bool IsProperty { get { return (bool)GetValue(IsPropertyProperty); } set { SetValue(IsPropertyProperty, value); } }
		public static readonly DependencyProperty IsPropertyProperty = DependencyProperty.Register("IsProperty", typeof(bool), typeof(Documentation), new PropertyMetadata(false));

		public bool IsMethod { get { return (bool)GetValue(IsMethodProperty); } set { SetValue(IsMethodProperty, value); } }
		public static readonly DependencyProperty IsMethodProperty = DependencyProperty.Register("IsMethod", typeof(bool), typeof(Documentation), new PropertyMetadata(false));

		public bool IsParameter { get { return (bool)GetValue(IsParameterProperty); } set { SetValue(IsParameterProperty, value); } }
		public static readonly DependencyProperty IsParameterProperty = DependencyProperty.Register("IsParameter", typeof(bool), typeof(Documentation), new PropertyMetadata(false));

		public string Summary { get { return (string)GetValue(SummaryProperty); } set { SetValue(SummaryProperty, value); } }
		public static readonly DependencyProperty SummaryProperty = DependencyProperty.Register("Summary", typeof(string), typeof(Documentation), new PropertyMetadata(""));

		public string Remarks { get { return (string)GetValue(RemarksProperty); } set { SetValue(RemarksProperty, value); } }
		public static readonly DependencyProperty RemarksProperty = DependencyProperty.Register("Remarks", typeof(string), typeof(Documentation), new PropertyMetadata(""));

		public string Returns { get { return (string)GetValue(ReturnsProperty); } set { SetValue(ReturnsProperty, value); } }
		public static readonly DependencyProperty ReturnsProperty = DependencyProperty.Register("Returns", typeof(string), typeof(Documentation), new PropertyMetadata(""));

		public string Example { get { return (string)GetValue(ExampleProperty); } set { SetValue(ExampleProperty, value); } }
		public static readonly DependencyProperty ExampleProperty = DependencyProperty.Register("Example", typeof(string), typeof(Documentation), new PropertyMetadata(""));

		public string Value { get { return (string)GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(Documentation), new PropertyMetadata(""));

	}
}